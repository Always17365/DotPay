using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public abstract class InsideTransferTransaction : DomainBase, IAggregateRoot,
                              IEventHandler<InsideTransferTransactionCreated>,
                              IEventHandler<InsideTransferTransactionComplete>
    {
        #region ctor
        protected InsideTransferTransaction() { }

        public InsideTransferTransaction(int fromUserID, int toUserID, CurrencyType currency, decimal amount, PayWay payway, string description)
        {
            this.RaiseEvent(new InsideTransferTransactionCreated(fromUserID, toUserID, currency, amount, payway, description, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual int FromUserID { get; protected set; }
        public virtual int ToUserID { get; protected set; }
        public virtual CurrencyType Currency { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region IMPL ICNYDeposit
        public virtual void Complete()
        {
            if (this.State != TransactionState.Pending)
                throw new TransferTransactionNotPendingException();
            else
                this.RaiseEvent(new InsideTransferTransactionComplete(this.ID, this.Currency));
        }
        #endregion

        #region inner event handlers

        void IEventHandler<InsideTransferTransactionCreated>.Handle(InsideTransferTransactionCreated @event)
        {
            this.FromUserID = @event.FromUserID;
            this.ToUserID = @event.ToUserID;
            this.SequenceNo = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().Shrink().Replace("-", string.Empty).Replace("_", string.Empty);
            this.Currency = @event.Currency;
            this.Amount = @event.Amount;
            this.PayWay = @event.PayWay;
            this.Description = @event.Description;
            this.State = TransactionState.Pending;
            this.Memo = string.Empty;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<InsideTransferTransactionComplete>.Handle(InsideTransferTransactionComplete @event)
        {
            this.State = TransactionState.Success;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
