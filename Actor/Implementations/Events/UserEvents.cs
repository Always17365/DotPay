using System;
using System.Collections.Generic;
using Dotpay.Common;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class UserPreRegisterEvent : GrainEvent
    {
        public UserPreRegisterEvent(long userId,string email, string token)
        {
            this.UserId = userId;
            this.Email = email;
            this.Token = token;
        }

        public long UserId { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserInitializedEvent : GrainEvent
    {
        public UserInitializedEvent(string loginName, string loginPassword, string paymentPassword, Guid accountId, string salt)
        {
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.PaymentPassword = paymentPassword;
            this.AccountId = accountId;
            this.Salt = salt;
        }

        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
        public string PaymentPassword { get; private set; }
        public Guid AccountId { get; private set; }
        public string Salt { get; private set; }
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
        public UserLockedEvent(long operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public long OperationId { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserUnlockedEvent : GrainEvent
    {
        public UserUnlockedEvent(long operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public long OperationId { get; private set; }
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
    public class UserLoginPasswordChangedEvent
        : GrainEvent
    {
        public UserLoginPasswordChangedEvent(string oldLoginPassword, string newLoginPassword)
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
    public class UserPaymentPasswordChangedEvent : GrainEvent
    {
        public UserPaymentPasswordChangedEvent(string oldPaymentPassword, string newPaymentPassword)
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
