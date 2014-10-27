using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using DotPay.Common;

namespace DotPay.MainDomain
{
    public abstract class Account : DomainBase, IAggregateRoot,
                         IEventHandler<AccountCreated>,
                         IEventHandler<AccountBalanceIncrease>,
                         IEventHandler<AccountBalanceDecrease>,
                         IEventHandler<AccountLockedIncrease>,
                         IEventHandler<AccountLockedDecrease>
    {
        #region ctor
        protected Account() { }

        public Account(int userID)
        {
            this.RaiseEvent(new AccountCreated(userID));
        }
        #endregion

        #region propertis
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual decimal Balance { get; protected set; }
        public virtual decimal Locked { get; protected set; }

        public virtual int CreateAt { get; protected set; }
        public virtual int UpdateAt { get; protected set; }
        #endregion

        #region public method
        public virtual void BalanceIncrease(decimal amount)
        {
            this.RaiseEvent(new AccountBalanceIncrease(this.ID, amount));
        }

        public virtual void BalanceDecrease(decimal amount)
        {
            if (this.Balance < amount)
                throw new AccountBalanceNotEnoughException();

            this.RaiseEvent(new AccountBalanceDecrease(this.ID, amount));
        }

        public virtual void LockedIncrease(decimal amount)
        {
            this.RaiseEvent(new AccountLockedIncrease(this.ID, amount));
        }

        public virtual void LockedDecrease(decimal amount)
        {
            if (this.Locked < amount)
                throw new AccountLockedNotEnoughException();

            this.RaiseEvent(new AccountLockedDecrease(this.ID, amount));
        }
        #endregion

        #region inner event handlers
        void IEventHandler<AccountCreated>.Handle(AccountCreated @event)
        {
            this.UserID = @event.UserID;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.Balance = 0;
            this.Locked = 0;
        }

        void IEventHandler<AccountBalanceDecrease>.Handle(AccountBalanceDecrease @event)
        {
            this.Balance -= @event.AmountDecrease;
            if (this.Balance < 0.000001M) this.Balance = 0;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<AccountBalanceIncrease>.Handle(AccountBalanceIncrease @event)
        {
            this.Balance += @event.AmountIncrease;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<AccountLockedDecrease>.Handle(AccountLockedDecrease @event)
        {
            this.Locked -= @event.LockedDecrease;
            if (this.Locked < 0.000001M) this.Locked = 0;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<AccountLockedIncrease>.Handle(AccountLockedIncrease @event)
        {
            this.Locked += @event.LockedIncrease;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
