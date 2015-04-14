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
        public ManagerInitializedEvent(Guid managerId, string loginName, string loginPassword, string twofactorKey, Guid createBy, string salt)
        {
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.TwofactorKey = twofactorKey;
            this.CreateBy = createBy;
            this.Salt = salt;
            this.ManagerId = managerId;
        }

        public Guid ManagerId { get; private set; }
        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
        public string TwofactorKey { get; private set; }
        public Guid CreateBy { get; private set; }
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
        public ManagerLockedEvent(Guid lockBy, string reason)
        {
            this.LockBy = lockBy;
            this.Reason = reason;
        }

        public Guid LockBy { get; private set; }
        public string Reason { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerUnlockedEvent : GrainEvent
    {
        public ManagerUnlockedEvent(Guid unlockBy)
        {
            this.UnlockBy = unlockBy;
        }

        public Guid UnlockBy { get; private set; }
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
        public ManagerLoginPasswordResetEvent(string newLoginPassword, Guid resetBy)
        {
            this.NewLoginPassword = newLoginPassword;
            this.ResetBy = resetBy;
        }

        public string NewLoginPassword { get; private set; }
        public Guid ResetBy { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class ManagerTwofactorKeyResetEvent : GrainEvent
    {
        public ManagerTwofactorKeyResetEvent(Guid resetBy, string otpKey)
        {
            this.ResetBy = resetBy;
            this.OtpKey = otpKey;
        }

        public Guid ResetBy { get; private set; }
        public string OtpKey { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class ManagerAssignedRolesEvent : GrainEvent
    {
        public ManagerAssignedRolesEvent(Guid assignBy, IEnumerable<ManagerType> roles)
        {
            this.AssignBy = assignBy;
            this.Roles = roles;
        }
        public Guid AssignBy { get; private set; }
        public IEnumerable<ManagerType> Roles { get; private set; }
    }
}
