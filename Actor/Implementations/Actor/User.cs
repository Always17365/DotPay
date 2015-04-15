using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using DFramework.Utilities;
using Dotpay.Actor.Events;
using Dotpay.Actor;
using Dotpay.Common;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class User : EventSourcingGrain<User, IUserState>, IUser
    {
        private readonly List<DateTime> _loginFailCounter = new List<DateTime>();
        private const int MAX_RETRY_LOGIN_TIMES = 5;
        private readonly static TimeSpan LimitPeriod = TimeSpan.FromHours(1);

        #region IUser
        async Task<ErrorCode> IUser.PreRegister(string email)
        {
            if (this.State.IsVerified)
                return ErrorCode.EmailAlreadyRegisted;
            else
            {
                var token = this.GenerteEmailValidateToken(email);
                //邮件服务考虑使用Message Queue完成发送,建立独立的邮件发送服务
                await this.ApplyEvent(new UserPreRegisterEvent(this.GetPrimaryKeyLong(), email, token));
            }
            return ErrorCode.None;
        }

        async Task<Guid> IUser.Initialize(string userAccount, string loginPassword, string paymentPassword)
        {
            var accountId = Guid.NewGuid();
            if (!this.State.IsVerified)
            {
                var salt = Guid.NewGuid().Shrink().Substring(0, 8);
                loginPassword = PasswordHelper.EncryptMD5(loginPassword + salt);
                paymentPassword = PasswordHelper.EncryptMD5(paymentPassword + salt);
                await this.ApplyEvent(new UserInitializedEvent(userAccount, loginPassword, paymentPassword, accountId, salt));
            }
            else accountId = this.State.AccountId;

            return accountId;
        }

        Task IUser.Lock(long operatorId, string reason)
        {
            if (this.State.IsVerified && !this.State.IsLocked)
                return this.ApplyEvent(new UserLockedEvent(operatorId, reason));

            return TaskDone.Done;
        }

        Task IUser.Unlock(long operatorId, string reason)
        {
            if (this.State.IsVerified && this.State.IsLocked)
                return this.ApplyEvent(new UserUnlockedEvent(operatorId, reason));

            return TaskDone.Done;
        }

        Task IUser.SetMobile(string mobile, string otpKey, string otp)
        {
            if (this.State.IsVerified && this.State.MobileSetting == null && Utilities.GenerateSmsOTP(otpKey, 1) == otp)
                return this.ApplyEvent(new UserSetMobileEvent(mobile, otpKey, otp));

            return TaskDone.Done;
        }

        Task IUser.SmsCounterIncrease()
        {
            if (this.State.IsVerified && this.State.MobileSetting != null)
            {
                var currentSmsCounter = this.State.MobileSetting.SmsCounter + 1;
                return this.ApplyEvent(new SmsCounterIncreasedEvent(currentSmsCounter));
            }

            return TaskDone.Done;
        }

        Task IUser.VeirfyIdentity(string fullName, IdNoType idNoType, string idNo)
        {
            if (this.State.IsVerified && this.State.IdentityInfo == null)
                return this.ApplyEvent(new UserIdentityVerifiedEvent(fullName, idNo, idNoType));

            return TaskDone.Done;
        }

        async Task<string> IUser.ForgetLoginPassword()
        {
            if (this.State.IsVerified)
            {
                var token = GenerteResetLoginPasswordToken(this.State.Email);
                await this.ApplyEvent(new UserLoginPasswordForgetEvent(token));
                return token;
            }

            return string.Empty;
        }

        Task IUser.ResetLoginPassword(string newLoginPassword, string resetToken)
        {
            newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
            if (this.State.IsVerified)
                return this.ApplyEvent(new UserLoginPasswordResetEvent(resetToken, newLoginPassword));

            return TaskDone.Done;
        }

        async Task<string> IUser.ForgetPaymentPassword()
        {
            if (this.State.IsVerified)
            {
                var token = this.GenerteResetPaymentPasswordToken(this.State.Email);
                await this.ApplyEvent(new UserLoginPasswordForgetEvent(token));
                return token;
            }
            return string.Empty;
        }

        Task IUser.ResetPaymentPassword(string newPaymentPassword, string resetToken)
        {
            if (this.State.IsVerified)
            {
                newPaymentPassword = PasswordHelper.EncryptMD5(newPaymentPassword + this.State.Salt);
                return this.ApplyEvent(new UserPaymentPasswordResetEvent(resetToken, newPaymentPassword));
            }
            return TaskDone.Done;
        }

        async Task<ErrorCode> IUser.Login(string loginPassword, string ip)
        {
            var now = DateTime.Now;
            if (_loginFailCounter.SkipWhile(t => t.Add(LimitPeriod) < now).Count() > MAX_RETRY_LOGIN_TIMES)
                return ErrorCode.ExceedMaxLoginFailTime;

            if (this.State.IsLocked)
                return ErrorCode.UserAccountIsLocked;
            else if (this.State.LoginPassword == PasswordHelper.EncryptMD5(loginPassword + this.State.Salt))
            {
                await this.ApplyEvent(new UserLoginSuccessedEvent(ip));
                return ErrorCode.None;
            }
            else
            {
                await this.ApplyEvent(new UserLoginFailedEvent(ip));
                return ErrorCode.LoginNameOrPasswordError;
            }
        }

        Task<bool> IUser.CheckLoginPassword(string loginPassword)
        {
            return Task.FromResult(this.State.LoginPassword == PasswordHelper.EncryptMD5(loginPassword + this.State.Salt));
        }

        Task<bool> IUser.CheckPaymentPassword(string paymentPassword)
        {
            return Task.FromResult(this.State.PaymentPassword == PasswordHelper.EncryptMD5(paymentPassword + this.State.Salt));
        }

        async Task<ErrorCode> IUser.ChangeLoginPassword(string oldLoginPassword, string newLoginPassword)
        {
            oldLoginPassword = PasswordHelper.EncryptMD5(oldLoginPassword + this.State.Salt);
            if (this.State.LoginPassword == oldLoginPassword)
            {
                newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
                await this.ApplyEvent(new UserLoginPasswordChangedEvent(oldLoginPassword, newLoginPassword));
                return ErrorCode.None;
            }
            else
                return ErrorCode.OldLoginPasswordError;
        }

        async Task<ErrorCode> IUser.ChangePaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode)
        {
            if (!this.CheckSmsOtp(smsVerifyCode))
                return ErrorCode.SmsPasswordError;

            oldPaymentPassword = PasswordHelper.EncryptMD5(oldPaymentPassword + this.State.Salt);
            if (this.State.PaymentPassword != oldPaymentPassword)
                return ErrorCode.OldPaymentPasswordError;

            newPaymentPassword = PasswordHelper.EncryptMD5(newPaymentPassword + this.State.Salt);
            await this.ApplyEvent(new UserPaymentPasswordChangedEvent(oldPaymentPassword, newPaymentPassword));
            return ErrorCode.None;
        }


        public Task<Guid> GetAccountId()
        {
            return Task.FromResult(this.State.AccountId);
        }

        public Task<UserInfo> GetUserInfo()
        {
            return Task.FromResult(new UserInfo(this.State.LoginName, this.State.Email));
        }

        #endregion

        #region Event Handlers
        private void Handle(UserPreRegisterEvent @event)
        {
            this.State.Id = @event.UserId;
            this.State.Email = @event.Email;
            this.State.EmailVerifyToken = @event.Token;
        }
        private void Handle(UserInitializedEvent @event)
        {
            this.State.IsVerified = true;
            this.State.LoginName = @event.LoginName;
            this.State.LoginPassword = @event.LoginPassword;
            this.State.PaymentPassword = @event.PaymentPassword;
            this.State.Salt = @event.Salt;

            this.State.WriteStateAsync();
        }
        private void Handle(UserLoginSuccessedEvent @event)
        {
            this.State.LastLoginAt = @event.UTCTimestamp;
            this.State.LastLoginIp = @event.IP;
        }
        private void Handle(UserLoginFailedEvent @event)
        {
            this.State.LastLoginFailedAt = @event.UTCTimestamp;
        }
        private void Handle(UserLockedEvent @event)
        {
            this.State.IsLocked = true;
            this.State.LockedAt = @event.UTCTimestamp;
        }
        private void Handle(UserUnlockedEvent @event)
        {
            this.State.IsLocked = false;
            this.State.LockedAt = null;
        }
        private void Handle(UserSetMobileEvent @event)
        {
            this.State.MobileSetting = new MobileSetting()
            {
                Mobile = @event.Mobile,
                SmsKey = @event.OTPKey,
                SmsCounter = 1
            };
            this.State.WriteStateAsync();
        }
        private void Handle(SmsCounterIncreasedEvent @event)
        {
            this.State.MobileSetting.SmsCounter = @event.SmsCounter;
        }
        private void Handle(UserIdentityVerifiedEvent @event)
        {
            this.State.IdentityInfo = new IdentityInfo(@event.FullName, @event.IdNo, @event.IdType);
            this.State.WriteStateAsync();
        }
        private void Handle(UserLoginPasswordChangedEvent @event)
        {
            this.State.LoginPassword = @event.NewLoginPassword;
            this.State.LastLoginPasswordChangeAt = @event.UTCTimestamp;
        }
        private void Handle(UserLoginPasswordForgetEvent @event)
        {
            this.State.LoginPasswordResetToken = @event.ResetToken;
            this.State.LoginPasswordResetTokenGenerateAt = @event.UTCTimestamp;
        }
        private void Handle(UserLoginPasswordResetEvent @event)
        {
            this.State.LoginPassword = @event.NewLoginPassword;
            this.State.LastLoginPasswordChangeAt = @event.UTCTimestamp;
        }
        private void Handle(UserPaymentPasswordChangedEvent @event)
        {
            this.State.PaymentPassword = @event.NewPaymentPassword;
            this.State.LastPaymentPasswordChangeAt = @event.UTCTimestamp;
        }
        private void Handle(UserPaymentPasswordForgetEvent @event)
        {
            this.State.PaymentPasswordResetToken = @event.ResetToken;
            this.State.PaymentPasswordResetTokenGenerateAt = @event.UTCTimestamp;
        }
        private void Handle(UserPaymentPasswordResetEvent @event)
        {
            this.State.LoginPassword = @event.NewPaymentPassword;
            this.State.LastLoginPasswordChangeAt = @event.UTCTimestamp;
        }
        #endregion

        #region Private method
        private string GenerteEmailValidateToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            var targetBytes = Encoding.UTF8.GetBytes(email + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }
        private string GenerteResetLoginPasswordToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            Random randomNum = new Random();
            var targetBytes = Encoding.UTF8.GetBytes(email + randomNum.Next() + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }
        private string GenerteResetPaymentPasswordToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            var targetBytes = Encoding.UTF8.GetBytes(email + Guid.NewGuid() + DateTime.Now.ToString(CultureInfo.InvariantCulture));
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
        long Id { get; set; }
        Guid AccountId { get; set; }
        string LoginName { get; set; }
        string Email { get; set; }
        string EmailVerifyToken { get; set; }
        bool IsVerified { get; set; }
        bool IsLocked { get; set; }
        DateTime? LockedAt { get; set; }
        IdentityInfo IdentityInfo { get; set; }
        MobileSetting MobileSetting { get; set; }
        string LoginPassword { get; set; }
        string LoginPasswordResetToken { get; set; }
        DateTime? LoginPasswordResetTokenGenerateAt { get; set; }
        DateTime? LastLoginPasswordChangeAt { get; set; }
        string PaymentPassword { get; set; }
        string PaymentPasswordResetToken { get; set; }
        DateTime? PaymentPasswordResetTokenGenerateAt { get; set; }
        DateTime? LastPaymentPasswordChangeAt { get; set; }
        string LastLoginIp { get; set; }
        DateTime? LastLoginAt { get; set; }
        DateTime? LastLoginFailedAt { get; set; }
        string Salt { get; set; }
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
