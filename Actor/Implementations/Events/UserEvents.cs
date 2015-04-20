using System;
using System.Collections.Generic;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class UserRegisterEvent : GrainEvent
    {
        public UserRegisterEvent(Guid userId, string loginName, string email, string loginPassword, string salt, Lang lang, string token)
        {
            this.UserId = userId;
            this.LoginName = loginName;
            this.Email = email;
            this.LoginPassword = loginPassword;
            this.Salt = salt;
            this.Lang = lang;
            this.Token = token;
        }

        public Guid UserId { get; private set; }
        public string LoginName { get; private set; }
        public string Email { get; private set; }
        public string LoginPassword { get; private set; }
        public string Salt { get; private set; }
        public Lang Lang { get; private set; }
        public string Token { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserActiveTokenResetEvent : GrainEvent
    {
        public UserActiveTokenResetEvent(string token)
        {
            this.Token = token;
        }
        public string Token { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordInitalizedEvent : GrainEvent
    {
        public UserPaymentPasswordInitalizedEvent(string paymentPassword)
        {
            this.PaymentPassword = paymentPassword;
        }
        public string PaymentPassword { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class UserActivedEvent : GrainEvent
    {
        public UserActivedEvent(string emailVerifyToken)
        {
            this.EmailVerifyToken = emailVerifyToken;
        }
        public string EmailVerifyToken { get; private set; }
    }
   
    [Immutable]
    [Serializable]
    public class UserLoginSuccessedEvent : GrainEvent
    {
        public UserLoginSuccessedEvent(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginFailedEvent : GrainEvent
    {
        public UserLoginFailedEvent(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLockedEvent : GrainEvent
    {
        public UserLockedEvent(Guid lockBy, string reason)
        {
            this.LockBy = lockBy;
            this.Reason = reason;
        }
        public Guid LockBy { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserUnlockedEvent : GrainEvent
    {
        public UserUnlockedEvent(Guid lockBy, string reason)
        {
            this.LockBy = lockBy;
            this.Reason = reason;
        }
        public Guid LockBy { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserSetMobileEvent : GrainEvent
    {
        public UserSetMobileEvent(string mobile, string otpKey, string otp)
        {
            this.Mobile = mobile;
            this.OTPKey = otpKey;
            this.OTP = otp;
        }

        public string Mobile { get; private set; }
        public string OTPKey { get; private set; }
        public string OTP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class SmsCounterIncreasedEvent : GrainEvent
    {
        public SmsCounterIncreasedEvent(int smsCounter)
        {
            this.SmsCounter = smsCounter;
        }

        public int SmsCounter { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserIdentityVerifiedEvent : GrainEvent
    {
        public UserIdentityVerifiedEvent(string fullName, string idNo, IdNoType idType)
        {
            this.FullName = fullName;
            this.IdNo = idNo;
            this.IdType = idType;
        }
        public string FullName { get; private set; }
        public string IdNo { get; private set; }
        public IdNoType IdType { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginPasswordModifiedEvent
        : GrainEvent
    {
        public UserLoginPasswordModifiedEvent(string oldLoginPassword, string newLoginPassword)
        {
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }
        public string OldLoginPassword { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginPasswordForgetEvent : GrainEvent
    {
        public UserLoginPasswordForgetEvent(string resetToken)
        {
            this.ResetToken = resetToken;
        }

        public string ResetToken { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginPasswordResetEvent : GrainEvent
    {
        public UserLoginPasswordResetEvent(string resetToken, string newLoginPassword)
        {
            this.ResetToken = resetToken;
            this.NewLoginPassword = newLoginPassword;
        }

        public string ResetToken { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordModifiedEvent : GrainEvent
    {
        public UserPaymentPasswordModifiedEvent(string oldPaymentPassword, string newPaymentPassword)
        {
            this.OldPaymentPassword = oldPaymentPassword;
            this.NewPaymentPassword = newPaymentPassword;
        }
        public string OldPaymentPassword { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordForgetEvent : GrainEvent
    {
        public UserPaymentPasswordForgetEvent(string resetToken)
        {
            this.ResetToken = resetToken;
        }

        public string ResetToken { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordResetEvent : GrainEvent
    {
        public UserPaymentPasswordResetEvent(string resetToken, string newPaymentPassword)
        {
            this.ResetToken = resetToken;
            this.NewPaymentPassword = newPaymentPassword;
        }

        public string ResetToken { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }
}
