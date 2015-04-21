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
using Dotpay.Common.Enum;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class User : EventSourcingGrain<User, IUserState>, IUser
    {
        private readonly List<DateTime> _loginFailCounter = new List<DateTime>();
        private readonly List<DateTime> _forgetLoginPasswordCounter = new List<DateTime>();
        private readonly List<DateTime> _forgetPaymentPasswordCounter = new List<DateTime>();
        private DateTime lastResetActiveTokenAt;
        private const int MAX_RETRY_LOGIN_TIMES = 5;
        private const int MAX_FORGET_LOGIN_PASSWORD_TIMES = 3;
        private readonly static TimeSpan LimitPeriod = TimeSpan.FromHours(1);

        #region IUser
        async Task<ErrorCode> IUser.Register(string email, string loginPassword, Lang lang, string activeToken)
        {
            if (!this.State.IsVerified)
            {
                var salt = Guid.NewGuid().Shrink().Substring(0, 8);
                loginPassword = PasswordHelper.EncryptMD5(loginPassword + salt);
                await this.ApplyEvent(new UserRegisterEvent(this.GetPrimaryKey(), email.ToLower(), loginPassword, salt, lang, activeToken));
            }
            return ErrorCode.None;
        }

        async Task<ErrorCode> IUser.ResetActiveToken(string activeToken)
        {
            if (this.State.IsVerified)
            {
                return ErrorCode.UserHasActived;
            }
            else
            {
                if (lastResetActiveTokenAt.AddMinutes(15) > DateTime.Now)
                    return ErrorCode.UserActiveEmailSendFrequently;
                else
                {
                    await this.ApplyEvent(new UserActiveTokenResetEvent(activeToken));
                    lastResetActiveTokenAt = DateTime.Now;
                    return ErrorCode.None;
                }
            }
        }

        Task IUser.InitializePaymentPassword(string paymentPassword)
        {
            if (this.State.IsVerified)
            {
                paymentPassword = PasswordHelper.EncryptMD5(paymentPassword + this.State.Salt);
                return this.ApplyEvent(new UserPaymentPasswordInitalizedEvent(paymentPassword));
            }
            else
                return TaskDone.Done;
        }
        async Task<ErrorCode> IUser.Active(string emailToken)
        {
            if (!this.State.IsVerified)
            {
                if (this.State.EmailVerifyToken.Equals(emailToken, StringComparison.OrdinalIgnoreCase))
                {
                    await this.ApplyEvent(new UserActivedEvent(emailToken));
                    return ErrorCode.None;
                }
                else
                    return ErrorCode.InvalidActiveToken;
            }
            else
                return ErrorCode.UserHasActived;
        }

        public Task SetLoginName(string loginName)
        {
            if (this.State.IsVerified)
            {
                return this.ApplyEvent(new UserLoginNameSetEvent(loginName));
            }
            else return TaskDone.Done;
        }

        Task IUser.Lock(Guid lockBy, string reason)
        {
            if (this.State.IsVerified && !this.State.IsLocked)
                return this.ApplyEvent(new UserLockedEvent(lockBy, reason));

            return TaskDone.Done;
        }

        Task IUser.Unlock(Guid lockBy, string reason)
        {
            if (this.State.IsVerified && this.State.IsLocked)
                return this.ApplyEvent(new UserUnlockedEvent(lockBy, reason));

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

        async Task<Tuple<ErrorCode, string>> IUser.ForgetLoginPassword(string token)
        {
            if (this.State.IsVerified)
            {
                if (_forgetLoginPasswordCounter.SkipWhile(d => d.AddMinutes(15) > DateTime.Now).Count() <
                    MAX_FORGET_LOGIN_PASSWORD_TIMES)
                {
                    await this.ApplyEvent(new UserLoginPasswordForgetEvent(token));
                    _forgetLoginPasswordCounter.Add(DateTime.Now);
                    return new Tuple<ErrorCode, string>(ErrorCode.None, token);
                }
                else
                {
                    return new Tuple<ErrorCode, string>(ErrorCode.ExceedMaxResetLoginPasswordRequestTime, string.Empty);
                }
            }

            return new Tuple<ErrorCode, string>(ErrorCode.InvalidUser, string.Empty);
        }

        async Task<ErrorCode> IUser.ResetLoginPassword(string newLoginPassword, string resetToken)
        {
            newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
            if (this.State.IsVerified)
            {
                if (this.State.LoginPasswordResetToken.Equals(resetToken, StringComparison.OrdinalIgnoreCase))
                {
                    await this.ApplyEvent(new UserLoginPasswordResetEvent(resetToken, newLoginPassword));
                    return ErrorCode.None;
                }
                else
                    return ErrorCode.InvalidResetLoginPasswordToken;
            }
            else
                return ErrorCode.InvalidUser;
        }

        async Task<Tuple<ErrorCode, string>> IUser.ForgetPaymentPassword(string token)
        {
            if (this.State.IsVerified)
            {
                if (_forgetLoginPasswordCounter.SkipWhile(d => d.AddMinutes(15) < DateTime.Now).Count() <
                    MAX_FORGET_LOGIN_PASSWORD_TIMES)
                {
                    await this.ApplyEvent(new UserPaymentPasswordForgetEvent(token));
                    _forgetLoginPasswordCounter.Add(DateTime.Now);
                    return new Tuple<ErrorCode, string>(ErrorCode.None, token);
                }
                else
                {
                    return new Tuple<ErrorCode, string>(ErrorCode.ExceedMaxResetLoginPasswordRequestTime, string.Empty);
                }
            }

            return new Tuple<ErrorCode, string>(ErrorCode.InvalidUser, string.Empty);
        }

        async Task<ErrorCode> IUser.ResetPaymentPassword(string newPaymentPassword, string resetToken)
        {
            if (this.State.IsVerified)
            {
                if (resetToken.Equals(this.State.PaymentPasswordResetToken, StringComparison.OrdinalIgnoreCase))
                {
                    newPaymentPassword = PasswordHelper.EncryptMD5(newPaymentPassword + this.State.Salt);
                    await this.ApplyEvent(new UserPaymentPasswordResetEvent(resetToken, newPaymentPassword));
                }
                else return ErrorCode.InvalidResetPaymentPasswordToken;
            }
            return ErrorCode.None;
        }
        async Task<Tuple<ErrorCode, int>> IUser.Login(string loginPassword, string ip)
        {
            var now = DateTime.Now;
            var remainRetryCounter = MAX_RETRY_LOGIN_TIMES - _loginFailCounter.Count();
            if (remainRetryCounter <= 0)
                return new Tuple<ErrorCode, int>(ErrorCode.ExceedMaxLoginFailTime, 0);
            else
                this._loginFailCounter.Add(DateTime.Now);

            if (this.State.IsLocked)
                return new Tuple<ErrorCode, int>(ErrorCode.UserAccountIsLocked, 0);
            else if (!this.State.IsVerified)
                return new Tuple<ErrorCode, int>(ErrorCode.UnactiveUser, 0);
            else if (this.State.LoginPassword == PasswordHelper.EncryptMD5(loginPassword + this.State.Salt))
            {
                await this.ApplyEvent(new UserLoginSuccessedEvent(ip));
                _loginFailCounter.Clear();
                return new Tuple<ErrorCode, int>(ErrorCode.None, 0); ;
            }
            else
            {
                await this.ApplyEvent(new UserLoginFailedEvent(ip));
                return new Tuple<ErrorCode, int>(ErrorCode.LoginNameOrPasswordError, remainRetryCounter);
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

        async Task<ErrorCode> IUser.ModifyLoginPassword(string oldLoginPassword, string newLoginPassword)
        {
            oldLoginPassword = PasswordHelper.EncryptMD5(oldLoginPassword + this.State.Salt);
            if (this.State.LoginPassword == oldLoginPassword)
            {
                newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
                await this.ApplyEvent(new UserLoginPasswordModifiedEvent(oldLoginPassword, newLoginPassword));
                return ErrorCode.None;
            }
            else
                return ErrorCode.OldLoginPasswordError;
        }

        async Task<ErrorCode> IUser.ModifyPaymentPassword(string oldPaymentPassword, string newPaymentPassword, string smsVerifyCode)
        {
            if (!this.CheckSmsOtp(smsVerifyCode))
                return ErrorCode.SmsPasswordError;

            oldPaymentPassword = PasswordHelper.EncryptMD5(oldPaymentPassword + this.State.Salt);
            if (this.State.PaymentPassword != oldPaymentPassword)
                return ErrorCode.OldPaymentPasswordError;

            newPaymentPassword = PasswordHelper.EncryptMD5(newPaymentPassword + this.State.Salt);
            await this.ApplyEvent(new UserPaymentPasswordModifiedEvent(oldPaymentPassword, newPaymentPassword));
            return ErrorCode.None;
        }


        public Task<Guid> GetAccountId()
        {
            return Task.FromResult(this.State.AccountId);
        }

        public Task<UserInfo> GetUserInfo()
        {
            return Task.FromResult(new UserInfo(this.State.LoginName, this.State.Email, this.State.Lang));
        }

        #endregion

        #region Event Handlers
        private void Handle(UserRegisterEvent @event)
        {
            this.State.Id = @event.UserId;
            this.State.Email = @event.Email;
            this.State.Salt = @event.Salt;
            this.State.LoginPassword = @event.LoginPassword;
            this.State.EmailVerifyToken = @event.Token;
            this.State.CreateAt = @event.UTCTimestamp;
            this.State.LoginName = string.Empty;
            this.State.Lang = @event.Lang;

            this.State.WriteStateAsync();
        }
        private void Handle(UserActiveTokenResetEvent @event)
        {
            this.State.EmailVerifyToken = @event.Token;

            this.State.WriteStateAsync();
        }
        private void Handle(UserPaymentPasswordInitalizedEvent @event)
        {
            this.State.PaymentPassword = @event.PaymentPassword;

            this.State.WriteStateAsync();
        }
        private void Handle(UserActivedEvent @event)
        {
            this.State.IsVerified = true;
            this.State.ActiveAt = @event.UTCTimestamp;

            this.State.WriteStateAsync();
        }
        private void Handle(UserLoginNameSetEvent @event)
        {
            this.State.LoginName = @event.LoginName;

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
        private void Handle(UserLoginPasswordModifiedEvent @event)
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
        private void Handle(UserPaymentPasswordModifiedEvent @event)
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
        Guid Id { get; set; }
        Lang Lang { get; set; }
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
        DateTime CreateAt { get; set; }
        DateTime? ActiveAt { get; set; }
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
