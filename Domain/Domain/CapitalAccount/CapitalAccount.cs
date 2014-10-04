using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;
using DotPay.Common;

namespace DotPay.Domain
{
    public class CapitalAccount : DomainBase, IAggregateRoot,
                                  IEventHandler<CapitalAccountCreated>,
                                  IEventHandler<CapitalAccountEnabled>,
                                  IEventHandler<CapitalAccountDisabled>
    {
        #region ctor
        protected CapitalAccount() { }

        public CapitalAccount(Bank bank, string bankAccount, string ownerName, int createBy)
        {
            this.RaiseEvent(new CapitalAccountCreated(bank, bankAccount, ownerName, createBy, this));
        }
        #endregion

        #region propertis
        public virtual int ID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual Bank Bank { get; protected set; }
        public virtual string OwnerName { get; protected set; }
        public virtual string BankAccount { get; protected set; }
        public virtual bool IsEnable { get; protected set; }

        public virtual int CreateAt { get; protected set; }
        public virtual int CreateBy { get; protected set; }
        #endregion

        #region public methods
        public virtual void Enable(int byUserID)
        {
            if (this.IsEnable) return;

            this.RaiseEvent(new CapitalAccountEnabled(this.ID, this.Bank, this.BankAccount, this.OwnerName, byUserID));
        }

        public virtual void Disable(int byUserID)
        {
            if (!this.IsEnable) return;

            this.RaiseEvent(new CapitalAccountDisabled(this.ID, this.Bank, this.BankAccount, this.OwnerName, byUserID));
        }
        #endregion

        #region inner event handlers
        void IEventHandler<CapitalAccountCreated>.Handle(CapitalAccountCreated @event)
        {
            this.UniqueID = Guid.NewGuid().Shrink();
            this.Bank = @event.Bank;
            this.BankAccount = @event.BankAccount;
            this.IsEnable = true;
            this.OwnerName = @event.OwnerName;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.CreateBy = @event.CreateBy;
        }

        void IEventHandler<CapitalAccountEnabled>.Handle(CapitalAccountEnabled @event)
        {
            this.IsEnable = true;
        }

        void IEventHandler<CapitalAccountDisabled>.Handle(CapitalAccountDisabled @event)
        {
            this.IsEnable = false;
        }
        #endregion
    }
}
