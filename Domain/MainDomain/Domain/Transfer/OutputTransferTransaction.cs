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
    public class OutputTransferTransaction : DomainBase, IAggregateRoot,
                              IEventHandler<OutputTransferTransactionCreated>,
                              IEventHandler<OutputTransferTransactionComplete>,
                              IEventHandler<OutputTransferTransactionFailed>
    {
        #region ctor
        protected OutputTransferTransaction() { }

        public OutputTransferTransaction(int fromUserID, string destination,
                                         CurrencyType sourceCurrency, decimal sourceAmount,
                                         CurrencyType targetCurrency, decimal targetAmount,
                                         PayWay payway)
        {
            this.RaiseEvent(new OutputTransferTransactionCreated(fromUserID, destination, sourceCurrency,
                                                     sourceAmount, targetCurrency, targetAmount, payway));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual int FromUserID { get; protected set; }
        public virtual string Destination { get; protected set; }
        public virtual CurrencyType SourceCurrency { get; protected set; }
        public virtual decimal SourceAmount { get; protected set; }
        public virtual CurrencyType TargetCurrency { get; protected set; }
        public virtual decimal TargetAmount { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string Memo { get; protected set; }
        #endregion

        #region IMPL ICNYDeposit

        public virtual void Complete()
        {
            if (this.State != TransactionState.Pending)
                throw new TransferTransactionNotPendingException();
            else
                this.RaiseEvent(new OutputTransferTransactionComplete(this.ID));
        }

        public virtual void Fail(string reason)
        {
            if (this.State != TransactionState.Pending)
                throw new TransferTransactionNotPendingException();
            else
                this.RaiseEvent(new OutputTransferTransactionFailed(this.ID, reason));
        }
        #endregion

        #region inner event handlers

        void IEventHandler<OutputTransferTransactionCreated>.Handle(OutputTransferTransactionCreated @event)
        {
            this.FromUserID = @event.FromUserID;
            this.Destination = @event.Destination;
            this.SequenceNo = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().Shrink().Replace("-", string.Empty).Replace("_", string.Empty);
            this.SourceCurrency = @event.SourceCurrency;
            this.SourceAmount = @event.SourceAmount;
            this.TargetCurrency = @event.TargetCurrency;
            this.TargetAmount = @event.TargetAmount;
            this.PayWay = @event.PayWay;
            this.State = TransactionState.Pending;
            this.Memo = string.Empty;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<OutputTransferTransactionComplete>.Handle(OutputTransferTransactionComplete @event)
        {
            this.State = TransactionState.Success;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        void IEventHandler<OutputTransferTransactionFailed>.Handle(OutputTransferTransactionFailed @event)
        {
            this.State = TransactionState.Success;
            this.DoneAt = 0;
            this.Memo = @event.Reason;
        }
        #endregion
    }
}
