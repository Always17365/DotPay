using System;
using System.Collections.Generic;
using Dotpay.Common;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    public class UserPreRegister : GrainEvent
    {
        public UserPreRegister(string email, string token)
        {
            this.Email = email;
            this.Token = token;
        }

        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class UserInitialized : GrainEvent
    {
        public UserInitialized(string loginName, string loginPassword, string paymentPassword)
        {
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.PaymentPassword = paymentPassword;
        }

        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public string PaymentPassword { get; set; }
    }
    public class UserLoginSuccessed : GrainEvent
    {
        public UserLoginSuccessed(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; set; }
    }
    public class UserLoginFailed : GrainEvent
    {
        public UserLoginFailed(string loginPassword, string ip)
        {
            this.LoginPassword = loginPassword;
            this.IP = ip;
        }
        public string LoginPassword { get; set; }
        public string IP { get; set; }
    }

    public class UserLocked : GrainEvent
    {
        public UserLocked(Guid operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public Guid OperationId { get; set; }
        public string Reason { get; set; }
    }

    public class UserUnlocked : GrainEvent
    {
        public UserUnlocked(Guid operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public Guid OperationId { get; set; }
        public string Reason { get; set; }
    }

    public class UserSetMobile : GrainEvent
    {
        public UserSetMobile(string mobile, string otpKey, string otp)
        {
            this.Mobile = mobile;
            this.OTPKey = otpKey;
            this.OTP = otp;
        }

        public string Mobile { get; set; }
        public string OTPKey { get; set; }
        public string OTP { get; set; }
    }

    public class SmsCounterIncreased : GrainEvent
    {
        public SmsCounterIncreased(int smsCounter)
        {
            this.SmsCounter = smsCounter;
        }

        public int SmsCounter { get; set; }
    }

    public class UserIdentityVerified : GrainEvent
    {
        public UserIdentityVerified(string fullName, string idNo, IdNoType idType)
        {
            this.FullName = fullName;
            this.IdNo = idNo;
            this.IdType = idType;
        }
        public string FullName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdType { get; set; }
    }

    public class UserLoginPasswordChanged : GrainEvent
    {
        public UserLoginPasswordChanged(string oldLoginPassword, string newLoginPassword)
        {
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }
        public string OldLoginPassword { get; set; }
        public string NewLoginPassword { get; set; }
    }

    public class UserPaymentPasswordChanged : GrainEvent
    {
        public UserPaymentPasswordChanged(string oldPaymentPassword, string newPaymentPassword)
        {
            this.OldPaymentPassword = oldPaymentPassword;
            this.NewPaymentPassword = newPaymentPassword;
        }
        public string OldPaymentPassword { get; set; }
        public string NewPaymentPassword { get; set; }
    }

    public class UserAssignedRoles : GrainEvent
    {
        public UserAssignedRoles(Guid operatorId, IEnumerable<ManagerType> roles)
        {
            this.OperatorId = operatorId;
            this.Roles = roles;
        }
        public Guid OperatorId { get; set; }
        public IEnumerable<ManagerType> Roles { get; set; }
    }
}
