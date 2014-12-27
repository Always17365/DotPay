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
    public class InboundTransferToBankPaymentTx : DomainBase, IAggregateRoot,
                              IEventHandler<InboundTransferToThirdPartyPaymentTxCreated>,
                              IEventHandler<InboundTransferToThirdPartyPaymentTxComplete>,
                              IEventHandler<InboundTransferToThirdPartyPaymentTxMarkProcessing>,
                              IEventHandler<InboundTransferToThirdPartyPaymentTxFailed>
    {
        #region ctor
        protected InboundTransferToBankPaymentTx() { }

        public InboundTransferToBankPaymentTx(string txid, string account, decimal amount, PayWay payway, PayWay sourcePayway, string realName, string memo)
        {
            this.RaiseEvent(new InboundTransferToThirdPartyPaymentTxCreated(txid, account, payway, amount, sourcePayway, realName, memo));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string SequenceNo { get; protected set; }
        public virtual string TxId { get; protected set; }
        public virtual PayWay SourcePayWay { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual string Account { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual TransactionState State { get; protected set; }
        public virtual string TransferNo { get; protected set; }
        public virtual int OperatorID { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }
        public virtual string RealName { get; protected set; }
        public virtual string Memo { get; protected set; }
        public virtual string Reason { get; protected set; }
        #endregion

        #region public method

        public virtual void MarkProcessing(int byUserID)
        {
            if (this.State != TransactionState.Init)
                throw new TransferTransactionNotInitException();
            else
                this.RaiseEvent(new InboundTransferToThirdPartyPaymentTxMarkProcessing(this.ID, this.PayWay, byUserID));
        }

        public virtual void Complete(string transferNo, int byUserID)
        {
            if (this.State != TransactionState.Pending && this.State != TransactionState.Fail)
                throw new TransferTransactionNotPendingException();
            else if (this.OperatorID != byUserID)
                throw new TransferTransactionIsProccessByOtherException();
            else
                this.RaiseEvent(new InboundTransferToThirdPartyPaymentTxComplete(this.ID, this.PayWay, transferNo, byUserID));
        }

        public virtual void Fail(string reason, int byUserID)
        {
            if (this.State != TransactionState.Pending)
                throw new TransferTransactionNotPendingException();
            else if (this.OperatorID != byUserID)
                throw new TransferTransactionIsProccessByOtherException();
            else
                this.RaiseEvent(new InboundTransferToThirdPartyPaymentTxFailed(this.ID, this.PayWay, reason, byUserID));
        }
        #endregion

        #region inner event handlers

        void IEventHandler<InboundTransferToThirdPartyPaymentTxCreated>.Handle(InboundTransferToThirdPartyPaymentTxCreated @event)
        {
            this.TxId = @event.TxId;
            this.Account = @event.Account;
            this.Amount = @event.Amount;
            this.PayWay = @event.PayWay;
            this.RealName = @event.RealName;
            this.SourcePayWay = @event.SourcePayway;
            this.SequenceNo = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
            this.State = TransactionState.Init;
            this.TransferNo = string.Empty;
            this.Reason = string.Empty;
            this.OperatorID = 0;
            this.DoneAt = 0;
            this.Memo = @event.Memo;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<InboundTransferToThirdPartyPaymentTxMarkProcessing>.Handle(InboundTransferToThirdPartyPaymentTxMarkProcessing @event)
        {
            this.State = TransactionState.Pending;
            this.OperatorID = @event.ByUserID;
        }

        void IEventHandler<InboundTransferToThirdPartyPaymentTxComplete>.Handle(InboundTransferToThirdPartyPaymentTxComplete @event)
        {
            this.State = TransactionState.Success;
            this.TransferNo = @event.TransferNo;
            this.DoneAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<InboundTransferToThirdPartyPaymentTxFailed>.Handle(InboundTransferToThirdPartyPaymentTxFailed @event)
        {
            this.State = TransactionState.Fail;
            this.DoneAt = 0;
            this.Reason = @event.Reason;
        }
        #endregion
    }
}
