using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using DFramework;
﻿using DFramework.Utilities;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Actor;
﻿using Dotpay.Common;
﻿using Orleans;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations.Actor
{
    /// <summary>
    /// Orleans grain implementation class Manager
    /// </summary>
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class Manager : EventSourcingGrain<Manager, IManagerState>, IManager
    {
        #region IManager
        Task IManager.Initialize(string loginName, string loginPassword, string twofactorKey, Guid operatorId)
        {
            if (!string.IsNullOrEmpty(this.State.LoginName))
            {
                var salt = Guid.NewGuid().Shrink().Substring(0, 10);
                loginPassword = PasswordHelper.EncryptMD5(loginPassword + salt);
                return this.ApplyEvent(new ManagerInitializedEvent(this.GetPrimaryKey(), loginName, loginPassword, twofactorKey, operatorId, salt));
            }

            return TaskDone.Done;
        }

        async Task<ErrorCode> IManager.Login(string loginPassword, string ip)
        {
            if (this.State.IsLocked)
                return ErrorCode.UserAccountIsLocked;
            else if (this.State.LoginPassword == PasswordHelper.EncryptMD5(loginPassword + this.State.Salt))
            {
                await this.ApplyEvent(new ManagerLoginSuccessedEvent(ip));
                return ErrorCode.None;
            }
            else
            {
                await this.ApplyEvent(new ManagerLoginFailedEvent(ip));
                return ErrorCode.LoginNameOrPasswordError;
            }
        }

        Task IManager.Lock(Guid operatorId, string reason)
        {
            if (!string.IsNullOrEmpty(this.State.LoginName) && !this.State.IsLocked)
                return this.ApplyEvent(new ManagerLockedEvent(operatorId, reason));

            return TaskDone.Done;
        }

        Task IManager.Unlock(Guid operatorId)
        {
            if (!string.IsNullOrEmpty(this.State.LoginName) && this.State.IsLocked)
                return this.ApplyEvent(new ManagerUnlockedEvent(operatorId));

            return TaskDone.Done;
        }

        Task<bool> IManager.CheckLoginPassword(string loginPassword)
        {
            return Task.FromResult(this.State.LoginPassword == loginPassword);
        }

        Task<bool> IManager.CheckTwofactor(string tfPassword)
        {
            var answer = Utilities.GenerateGoogleAuthOTP(this.State.TwofactorKey);

            return Task.FromResult(tfPassword == answer);
        }

        async Task<ErrorCode> IManager.ChangeLoginPassword(string oldLoginPassword, string newLoginPassword)
        {
            oldLoginPassword = PasswordHelper.EncryptMD5(oldLoginPassword + this.State.Salt);
            if (this.State.LoginPassword == oldLoginPassword)
            {
                newLoginPassword = PasswordHelper.EncryptMD5(oldLoginPassword + this.State.Salt);
                await this.ApplyEvent(new ManagerLoginPasswordChangedEvent(oldLoginPassword, newLoginPassword));
                return ErrorCode.None;
            }
            else
                return ErrorCode.OldLoginPasswordError;
        }

        Task IManager.ResetLoginPassword(string newLoginPassword, Guid operatorId)
        {
            return this.ApplyEvent(new ManagerLoginPasswordResetEvent(newLoginPassword, operatorId));
        }
        Task IManager.AssignRoles(Guid operatorId, IEnumerable<ManagerType> roles)
        {
            return this.ApplyEvent(new ManagerAssignedRolesEvent(operatorId, roles));
        }

        public Task<bool> HasRole(ManagerType role)
        {
            return Task.FromResult(this.State.Roles != null && this.State.Roles.Contains(role));
        }

        #endregion

        #region Events Handler
        private void Handle(ManagerInitializedEvent @event)
        {
            this.State.Id = @event.ManagerId;
            this.State.LoginName = @event.LoginName;
            this.State.LoginPassword = @event.LoginPassword;
            this.State.TwofactorKey = @event.TwofactorKey;
            this.State.Salt = @event.Salt;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerLockedEvent @event)
        {
            this.State.IsLocked = true;
            this.State.Reason = @event.Reason;
            this.State.LockedAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerUnlockedEvent @event)
        {
            this.State.IsLocked = false;
            this.State.LockedAt = null;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerLoginPasswordChangedEvent @event)
        {
            this.State.LoginPassword = @event.NewLoginPassword;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerLoginPasswordResetEvent @event)
        {
            this.State.LoginPassword = @event.NewLoginPassword;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerAssignedRolesEvent @event)
        {
            this.State.Roles = @event.Roles;
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerLoginSuccessedEvent @event)
        {
            this.State.LastLoginAt = @event.UTCTimestamp;
            this.State.LastLoginIp = @event.IP;
        }
        private void Handle(ManagerLoginFailedEvent @event)
        {
            this.State.LastLoginFailedAt = @event.UTCTimestamp;
        }
        #endregion
    }

    public interface IManagerState : IEventSourcingState
    {
        Guid Id { get; set; }
        string LoginName { get; set; }
        string TwofactorKey { get; set; }
        string LoginPassword { get; set; }
        string Salt { get; set; }
        bool IsLocked { get; set; }
        DateTime? LockedAt { get; set; }
        DateTime? LastLoginAt { get; set; }
        string LastLoginIp { get; set; }
        DateTime? LastLoginFailedAt { get; set; }
        string Reason { get; set; }
        IEnumerable<ManagerType> Roles { get; set; }
    }
}
