using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using DFramework.Utilities;
using Dotpay.Actor.Events;
using Dotpay.Common;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    /// <summary>
    /// Orleans grain implementation class Manager
    /// </summary>
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class Manager : EventSourcingGrain<Manager, IManagerState>, IManager
    {
        private readonly List<DateTime> _loginFailCounter = new List<DateTime>();
        private const int MAX_RETRY_LOGIN_TIMES = 5;
        private readonly static TimeSpan LimitPeriod = TimeSpan.FromHours(1);

        #region IManager
        Task IManager.Initialize(string loginName, string loginPassword, string twofactorKey, Guid createBy)
        {
            if (string.IsNullOrEmpty(this.State.LoginName))
            {
                var salt = Guid.NewGuid().Shrink().Substring(0, 10);
                loginPassword = PasswordHelper.EncryptMD5(loginPassword + salt);
                return this.ApplyEvent(new ManagerInitializedEvent(this.GetPrimaryKey(), loginName, loginPassword, twofactorKey, createBy, salt));
            }

            return TaskDone.Done;
        }

        async Task<ErrorCode> IManager.Login(string loginPassword, string ip)
        {
            var now = DateTime.Now;
            if (_loginFailCounter.SkipWhile(t => t.Add(LimitPeriod) < now).Count() > MAX_RETRY_LOGIN_TIMES)
                return ErrorCode.ExceedMaxLoginFailTime;

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
                _loginFailCounter.Add(DateTime.Now);
                return ErrorCode.LoginNameOrPasswordError;
            }
        }

        Task IManager.Lock(Guid lockBy, string reason)
        {
            if (!string.IsNullOrEmpty(this.State.LoginName) && !this.State.IsLocked)
                return this.ApplyEvent(new ManagerLockedEvent(lockBy, reason));

            return TaskDone.Done;
        }

        Task IManager.Unlock(Guid unlockBy)
        {
            if (!string.IsNullOrEmpty(this.State.LoginName) && this.State.IsLocked)
                return this.ApplyEvent(new ManagerUnlockedEvent(unlockBy));

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
                newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
                await this.ApplyEvent(new ManagerLoginPasswordChangedEvent(oldLoginPassword, newLoginPassword));
                return ErrorCode.None;
            }
            else
                return ErrorCode.OldLoginPasswordError;
        }

        Task IManager.ResetLoginPassword(string newLoginPassword, Guid resetBy)
        {
            var salt = Guid.NewGuid().Shrink().Substring(0, 10);
            newLoginPassword = PasswordHelper.EncryptMD5(newLoginPassword + this.State.Salt);
            return this.ApplyEvent(new ManagerLoginPasswordResetEvent(newLoginPassword, resetBy));
        }

        public Task ResetTwofactorKey(Guid resetBy)
        {
            var key = Utilities.GenerateOTPKey();

            return this.ApplyEvent(new ManagerTwofactorKeyResetEvent(resetBy, key));
        }

        Task IManager.AssignRoles(Guid assignBy, IEnumerable<ManagerType> roles)
        {
            return this.ApplyEvent(new ManagerAssignedRolesEvent(assignBy, roles));
        }

        public Task<bool> HasRole(ManagerType role)
        {
            return Task.FromResult(this.State.Roles != null && this.State.Roles.Contains(role));
        }

        public Task<bool> HasInitialized()
        {
            return Task.FromResult(!string.IsNullOrEmpty(this.State.Salt));
        }

        public Task<string> GetManagerLoginName()
        { 
            return Task.FromResult(this.State.LoginName);
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
            this.State.CreateAt = @event.UTCTimestamp;
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
        private void Handle(ManagerTwofactorKeyResetEvent @event)
        {
            this.State.TwofactorKey = @event.OtpKey;
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
            this.State.WriteStateAsync();
        }
        private void Handle(ManagerLoginFailedEvent @event)
        {
            this.State.LastLoginFailedAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
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
        DateTime CreateAt { get; set; }
        DateTime? LockedAt { get; set; }
        DateTime? LastLoginAt { get; set; }
        string LastLoginIp { get; set; }
        DateTime? LastLoginFailedAt { get; set; }
        string Reason { get; set; }
        IEnumerable<ManagerType> Roles { get; set; }
    }
}
