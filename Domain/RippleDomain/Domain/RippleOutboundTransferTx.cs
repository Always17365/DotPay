using DotPay.RippleDomain.Events;
using FC.Framework.Domain;
using FC.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using DotPay.RippleDomain.Exceptions;

namespace DotPay.RippleDomain
{
    public class RippleOutboundTransferTx : DomainBase, IAggregateRoot,
                                          IEventHandler<RippleOutboundTransferTxCreated>,
                                          IEventHandler<RippleOutboundTransferSigned>,
                                          IEventHandler<RippleOutboundTransferSubmitSuccess>,
                                          IEventHandler<RippleOutboundTransferSubmitFail>
    {
        #region ctor
        protected RippleOutboundTransferTx() { }

        public RippleOutboundTransferTx(string destination, int destinationTag, string targetCurrency, decimal targetAmount, decimal tourceSendMaxAmount, List<List<object>> ripplePaths)
        {
            this.RaiseEvent(new RippleOutboundTransferTxCreated(destination, destinationTag, targetCurrency, targetAmount, tourceSendMaxAmount, ripplePaths));
        }

        #endregion

        public virtual int ID { get; protected set; }
        public virtual string TxId { get; protected set; }
        public virtual string Destination { get; protected set; }
        public virtual int DestinationTag { get; protected set; }
        public virtual RippleTransactionState State { get; protected set; }
        public virtual string TargetCurrency { get; protected set; }
        public virtual string TxBlob { get; protected set; }
        public virtual decimal TargetAmount { get; protected set; }
        public virtual decimal SourceAmount { get; protected set; }
        public virtual decimal Fee { get; protected set; }
        public virtual decimal SourceSendMaxAmount { get; protected set; }
        public virtual string Reason { get; protected set; }

        public virtual void MarkSigned(string txid, string txblob)
        {
            if (this.State != RippleTransactionState.Init)
                throw new RippleTransactionNotPendingException();
            else
                this.RaiseEvent(new RippleOutboundTransferSigned(this.ID, txid, txblob));
        }
        public virtual void MarkSubmitedSuccess(string txid)
        {
            if (this.State != RippleTransactionState.Submit)
                throw new RippleTransactionNotSubmitException();
            else
                this.RaiseEvent(new RippleOutboundTransferSubmitSuccess(this.ID, txid));
        }
        public virtual void MarkSubmitedFail(string txid, string reason)
        {
            if (this.State != RippleTransactionState.Submit)
                throw new RippleTransactionNotSubmitException();
            else
                this.RaiseEvent(new RippleOutboundTransferSubmitFail(this.ID, txid, reason));
        }


        void IEventHandler<RippleOutboundTransferTxCreated>.Handle(RippleOutboundTransferTxCreated @event)
        {
            this.Destination = @event.Destination;
            this.DestinationTag = @event.DestinationTag;
            this.TargetCurrency = @event.TargetCurrency;
            this.TargetAmount = @event.TargetAmount;
            this.State = RippleTransactionState.Init;
            this.TxId = string.Empty;
            this.TxBlob = string.Empty;
            this.Reason = string.Empty;
            this.SourceSendMaxAmount = @event.SourceSendMaxAmount;
        }

        void IEventHandler<RippleOutboundTransferSigned>.Handle(RippleOutboundTransferSigned @event)
        {
            this.State = RippleTransactionState.Submit;
            this.TxId = @event.Txhash;
            this.TxBlob = @event.Txblob;
        }

        void IEventHandler<RippleOutboundTransferSubmitSuccess>.Handle(RippleOutboundTransferSubmitSuccess @event)
        {
            this.State = RippleTransactionState.Success;
        }

        void IEventHandler<RippleOutboundTransferSubmitFail>.Handle(RippleOutboundTransferSubmitFail @event)
        {
            this.State = RippleTransactionState.Fail;
            this.Reason = @event.Reason;
        }
    }
}
