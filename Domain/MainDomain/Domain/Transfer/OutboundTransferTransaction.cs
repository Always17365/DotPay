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
    public class OutboundTransferTransaction : DomainBase, IAggregateRoot,
                              IEventHandler<OutboundTransferTransactionCreated>,
                              IEventHandler<OutboundTransferTransactionComplete>,
                              IEventHandler<OutboundTransferTransactionFailed>
    {
        #region ctor
        protected OutboundTransferTransaction() { }

        public OutboundTransferTransaction(int fromUserID, string destination, decimal sourceAmount,
                                           CurrencyType targetCurrency, decimal targetAmount, PayWay payway,string description)
        {
            this.RaiseEvent(new OutboundTransferTransactionCreated(fromUserID, destination, sourceAmount, targetCurrency, targetAmount, payway,description, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual int FromUserID { get; protected set; }
        public virtual string Destination { get; protected set; }
        public virtual decimal SourceAmount { get; protected set; }
        public virtual CurrencyType TargetCurrency { get; protected set; }
        public virtual decimal TargetAmount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual string TransferNo { get; protected set; }
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
                this.RaiseEvent(new OutboundTransferTransactionComplete(this.ID));
        }

        public virtual void Confirm(int byUserID)
        {
            if (this.FromUserID != byUserID)
                throw new TradePasswordErrorException();
            if (this.State != TransactionState.Init)
                throw new TransferTransactionHasConfirmedException();
            else
                this.RaiseEvent(new OutboundTransferTransactionConfirmed(this.ID));
        }

        public virtual void Fail(string reason)
        {
            if (this.State != TransactionState.Pending)
                throw new TransferTransactionNotPendingException();
            else
                this.RaiseEvent(new OutboundTransferTransactionFailed(this.ID, reason));
        }
        #endregion

        #region inner event handlers

        void IEventHandler<OutboundTransferTransactionCreated>.Handle(OutboundTransferTransactionCreated @event)
        {
            this.FromUserID = @event.FromUserID;
            this.Destination = @event.Destination;
            this.SequenceNo = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Replace("-", string.Empty);
            this.SourceAmount = @event.SourceAmount;
            this.TransferNo = string.Empty;
            this.TargetCurrency = @event.TargetCurrency;
            this.TargetAmount = @event.TargetAmount;
            this.PayWay = @event.PayWay;
            this.State = TransactionState.Init;
            this.Memo = string.Empty;
            this.Description = @event.Description;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<OutboundTransferTransactionComplete>.Handle(OutboundTransferTransactionComplete @event)
        {
            this.State = TransactionState.Success;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        void IEventHandler<OutboundTransferTransactionFailed>.Handle(OutboundTransferTransactionFailed @event)
        {
            this.State = TransactionState.Success;
            this.DoneAt = 0;
            this.Memo = @event.Reason;
        }
        #endregion
    }
}
