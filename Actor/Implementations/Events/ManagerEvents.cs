using System;
using System.Collections.Generic;
using Dotpay.Common;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class ManagerInitializedEvent : GrainEvent
    {
        public ManagerInitializedEvent(string loginName, string loginPassword, string twofactorKey, Guid operatorId, string salt)
        {
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.TwofactorKey = twofactorKey;
            this.OperatorId = operatorId;
            this.Salt = salt;
        }

        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
        public string TwofactorKey { get; private set; }
        public Guid OperatorId { get; private set; }
        public string Salt { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerLoginSuccessedEvent : GrainEvent
    {
        public ManagerLoginSuccessedEvent(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerLoginFailedEvent : GrainEvent
    {
        public ManagerLoginFailedEvent(string ip)
        {
            this.IP = ip;
        }
        public string IP { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerLockedEvent : GrainEvent
    {
        public ManagerLockedEvent(Guid operationId, string reason)
        {
            this.OperationId = operationId;
            this.Reason = reason;
        }
        public Guid OperationId { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerUnlockedEvent : GrainEvent
    {
        public ManagerUnlockedEvent(Guid operationId)
        {
            this.OperationId = operationId;
        }
        public Guid OperationId { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerSetMobileEvent : GrainEvent
    {
        public ManagerSetMobileEvent(string mobile, string otpKey, string otp)
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
    public class ManagerIdentityVerifiedEvent : GrainEvent
    {
        public ManagerIdentityVerifiedEvent(string fullName, string idNo, IdNoType idType)
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
    public class ManagerLoginPasswordChangedEvent : GrainEvent
    {
        public ManagerLoginPasswordChangedEvent(string oldLoginPassword, string newLoginPassword)
        {
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }
        public string OldLoginPassword { get; private set; }
        public string NewLoginPassword { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class ManagerLoginPasswordResetEvent : GrainEvent
    {
        public ManagerLoginPasswordResetEvent(string newLoginPassword, Guid operatorId)
        {
            this.NewLoginPassword = newLoginPassword;
            this.OperatorId = operatorId;
        }

        public string NewLoginPassword { get; private set; }
        public Guid OperatorId { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerPaymentPasswordChangedEvent : GrainEvent
    {
        public ManagerPaymentPasswordChangedEvent(string oldPaymentPassword, string newPaymentPassword)
        {
            this.OldPaymentPassword = oldPaymentPassword;
            this.NewPaymentPassword = newPaymentPassword;
        }
        public string OldPaymentPassword { get; private set; }
        public string NewPaymentPassword { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class ManagerAssignedRolesEvent : GrainEvent
    {
        public ManagerAssignedRolesEvent(Guid operatorId, IEnumerable<ManagerType> roles)
        {
            this.OperatorId = operatorId;
            this.Roles = roles;
        }
        public Guid OperatorId { get; private set; }
        public IEnumerable<ManagerType> Roles { get; private set; }
    }
}
