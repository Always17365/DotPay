using System;
using System.Collections.Generic;
using Dotpay.Common;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class UserPreRegister : GrainEvent
    {
        public UserPreRegister(string email, string token)
        {
            this.Email = email;
            this.Token = token;
        }

        public string Email { get; private set; }
        public string Token { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserInitialized : GrainEvent
    {
        public UserInitialized(string loginName, string loginPassword, string paymentPassword)
        {
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.PaymentPassword = paymentPassword;
        }

        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
        public string PaymentPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginSuccessed : GrainEvent
    {
        public UserLoginSuccessed(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginFailed : GrainEvent
    {
        public UserLoginFailed(string loginPassword, string ip)
        {
            this.LoginPassword = loginPassword;
            this.IP = ip;
        }
        public string LoginPassword { get; private set; }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLocked : GrainEvent
    {
        public UserLocked(Guid operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public Guid OperationId { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserUnlocked : GrainEvent
    {
        public UserUnlocked(Guid operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public Guid OperationId { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserSetMobile : GrainEvent
    {
        public UserSetMobile(string mobile, string otpKey, string otp)
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
    public class SmsCounterIncreased : GrainEvent
    {
        public SmsCounterIncreased(int smsCounter)
        {
            this.SmsCounter = smsCounter;
        }

        public int SmsCounter { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserIdentityVerified : GrainEvent
    {
        public UserIdentityVerified(string fullName, string idNo, IdNoType idType)
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
    public class UserLoginPasswordChanged : GrainEvent
    {
        public UserLoginPasswordChanged(string oldLoginPassword, string newLoginPassword)
        {
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }
        public string OldLoginPassword { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginPasswordForget : GrainEvent
    {
        public UserLoginPasswordForget(string resetToken)
        {
            this.ResetToken = resetToken;
        }

        public string ResetToken { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserLoginPasswordReset : GrainEvent
    {
        public UserLoginPasswordReset(string resetToken, string newLoginPassword)
        {
            this.ResetToken = resetToken;
            this.NewLoginPassword = newLoginPassword;
        }

        public string ResetToken { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordChanged : GrainEvent
    {
        public UserPaymentPasswordChanged(string oldPaymentPassword, string newPaymentPassword)
        {
            this.OldPaymentPassword = oldPaymentPassword;
            this.NewPaymentPassword = newPaymentPassword;
        }
        public string OldPaymentPassword { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordForget : GrainEvent
    {
        public UserPaymentPasswordForget(string resetToken)
        {
            this.ResetToken = resetToken;
        }

        public string ResetToken { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserPaymentPasswordReset : GrainEvent
    {
        public UserPaymentPasswordReset(string resetToken, string newPaymentPassword)
        {
            this.ResetToken = resetToken;
            this.NewPaymentPassword = newPaymentPassword;
        }

        public string ResetToken { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class UserAssignedRoles : GrainEvent
    {
        public UserAssignedRoles(Guid operatorId, IEnumerable<ManagerType> roles)
        {
            this.OperatorId = operatorId;
            this.Roles = roles;
        }
        public Guid OperatorId { get; private set; }
        public IEnumerable<ManagerType> Roles { get; private set; }
    }
}
