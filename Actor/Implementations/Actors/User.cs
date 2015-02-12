using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Actor.Events;
using Dotpay.Actor.Interfaces;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class User : EventSourcingGrain<User, IUserState>, IUser
    {
        #region IUser
        async Task<ErrorCode> IUser.PreRegister(string email)
        {
            if (this.State.IsVerified)
                return ErrorCode.EmailAlreadyRegisted;
            else
            {
                var token = this.GenerteEmailValidateToken(email);
                //邮件服务考虑使用Message Queue完成发送,建立独立的邮件发送服务
                await this.ApplyEvent(new UserPreRegister(email, token));
            }
            return ErrorCode.None;
        }

        Task IUser.Initialize(string userAccount, string loginPassword, string paymentPassword)
        {
            return this.State.IsVerified ? this.ApplyEvent(new UserInitialized(userAccount, loginPassword, paymentPassword)) : TaskDone.Done;
        }

        Task IUser.Lock(Guid operatorId, string reason)
        {
            if (this.State.IsVerified && !this.State.IsLocked)
                return this.ApplyEvent(new UserLocked(operatorId, reason));

            return TaskDone.Done;
        }

        Task IUser.Unlock(Guid operatorId, string reason)
        {
            if (this.State.IsVerified && this.State.IsLocked)
                return this.ApplyEvent(new UserUnlocked(operatorId, reason));

            return TaskDone.Done;
        }

        Task IUser.SetMobile(string mobile, string otpKey, string otp)
        {
            if (this.State.IsVerified && this.State.MobileSetting == null && Utilities.GenerateSmsOTP(otpKey, 1) == otp)
                return this.ApplyEvent(new UserSetMobile(mobile, otpKey, otp));

            return TaskDone.Done;
        }

        Task IUser.VeirfyIdentity(string fullName, IdNoType idNoType, string idNo)
        {
            if (this.State.IsVerified && this.State.IdentityInfo == null)
                return this.ApplyEvent(new UserIdentityVerified(fullName, idNo, idNoType));

            return TaskDone.Done;
        }
        async Task<ErrorCode> IUser.Login(string loginPassword, string ip)
        {
            if (this.State.IsLocked)
                return ErrorCode.UserAccountIsLocked;
            else if (this.State.LoginPassword == loginPassword)
            {
                await this.ApplyEvent(new UserLoginSuccessed(ip));
                return ErrorCode.None;
            }
            else
            {
                await this.ApplyEvent(new UserLoginFailed(loginPassword, ip));
                return ErrorCode.LoginNameOrPasswordError;
            }
        }

        Task<bool> IUser.CheckLoginPassword(string loginPassword)
        {
            return Task.FromResult(this.State.LoginPassword == loginPassword);
        }

        Task<bool> IUser.CheckPaymentPassword(string paymentPassword)
        {
            return Task.FromResult(this.State.PaymentPassword == paymentPassword);
        }

        async Task<ErrorCode> IUser.ChangeLoginPassword(string oldLoginPassword, string newLoginPassword)
        {
            if (this.State.LoginPassword == oldLoginPassword)
            {
                await this.ApplyEvent(new UserLoginPasswordChanged(oldLoginPassword, newLoginPassword));
                return ErrorCode.None;
            }
            else
                return ErrorCode.OldLoginPasswordError;
        }

        async Task<ErrorCode> IUser.ChangePaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode)
        {
            if (!this.CheckSmsOtp(smsVerifyCode))
                return ErrorCode.SMSPasswordError;

            if (this.State.PaymentPassword != oldPaymentPassword)
                return ErrorCode.OldPaymentPasswordError;

            await this.ApplyEvent(new UserPaymentPasswordChanged(oldPaymentPassword, newPaymentPassword));
            return ErrorCode.None;
        }

        Task IUser.AssignRoles(Guid operatorId, IEnumerable<ManagerType> roles)
        {
            return this.ApplyEvent(new UserAssignedRoles(operatorId, roles));
        }
        #endregion

        #region Event Handlers
        private void Handle(UserPreRegister @event)
        {
            this.State.Email = @event.Email;
            this.State.EmailVerifyToken = @event.Token;
        }
        private void Handle(UserInitialized @event)
        {
            this.State.IsVerified = true;
            this.State.LoginName = @event.LoginName;
            this.State.LoginPassword = @event.LoginPassword;
            this.State.PaymentPassword = @event.PaymentPassword;

            this.State.WriteStateAsync();
        }
        private void Handle(UserLoginSuccessed @event)
        {
            this.State.LastLoginAt = @event.UTCTimestamp;
            this.State.LastLoginIp = @event.IP;
        }
        private void Handle(UserLoginFailed @event)
        {
            this.State.LastLoginFailedAt = @event.UTCTimestamp;
        }
        private void Handle(UserLocked @event)
        {
            this.State.IsLocked = true;
            this.State.LockedAt = @event.UTCTimestamp;
        }
        private void Handle(UserUnlocked @event)
        {
            this.State.IsLocked = false;
            this.State.LockedAt = null;
        }
        private void Handle(UserSetMobile @event)
        {
            this.State.MobileSetting = new MobileSetting()
            {
                Mobile = @event.Mobile,
                SmsKey = @event.OTPKey,
                SmsCounter = 1
            };
            this.State.WriteStateAsync();
        }
        private void Handle(UserIdentityVerified @event)
        {
            this.State.IdentityInfo = new IdentityInfo(@event.FullName, @event.IdNo, @event.IdType);
            this.State.WriteStateAsync();
        }
        private void Handle(UserLoginPasswordChanged @event)
        {
            this.State.LoginPassword = @event.NewLoginPassword;
            this.State.LastLoginPasswordChangeAt = @event.UTCTimestamp;
        }
        private void Handle(UserPaymentPasswordChanged @event)
        {
            this.State.PaymentPassword = @event.NewPaymentPassword;
            this.State.LastPaymentPasswordChangeAt = @event.UTCTimestamp;
        }
        private void Handle(UserAssignedRoles @event)
        {
            this.State.Roles = @event.Roles;
            this.State.WriteStateAsync();
        }
        #endregion

        #region private method
        private string GenerteEmailValidateToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            var targetBytes = Encoding.UTF8.GetBytes(email + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }
        private bool CheckSmsOtp(string otp)
        {
            var result = false;

            if (this.State.MobileSetting != null)
            {
                result = Utilities.GenerateSmsOTP(this.State.MobileSetting.SmsKey, this.State.MobileSetting.SmsCounter) == otp;
            }
            else
                result = true;

            return result;
        }
        #endregion
    }

    #region IUserState
    public interface IUserState : IEventSourcingState
    {
        string LoginName { get; set; }
        string Email { get; set; }
        string EmailVerifyToken { get; set; }
        bool IsVerified { get; set; }
        bool IsLocked { get; set; }
        DateTime? LockedAt { get; set; }
        IEnumerable<ManagerType> Roles { get; set; }
        IdentityInfo IdentityInfo { get; set; }
        MobileSetting MobileSetting { get; set; }
        string LoginPassword { get; set; }
        DateTime? LastLoginPasswordChangeAt { get; set; }
        string PaymentPassword { get; set; }
        DateTime? LastPaymentPasswordChangeAt { get; set; }
        string LastLoginIp { get; set; }
        DateTime? LastLoginAt { get; set; }
        DateTime? LastLoginFailedAt { get; set; }
    }
    #endregion

    #region Sub Class
    public class IdentityInfo
    {
        public IdentityInfo(string fullName, string idNo, IdNoType idType)
        {
            this.FullName = fullName;
            this.IdNo = idNo;
            this.IdType = IdType;
        }
        public string FullName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdType { get; set; }
    }

    public class MobileSetting
    {
        public string Mobile { get; set; }
        public string SmsKey { get; set; }
        public int SmsCounter { get; set; }
    }
    #endregion
}
