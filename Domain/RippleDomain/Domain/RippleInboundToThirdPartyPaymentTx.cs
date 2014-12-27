using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.RippleDomain.Events;
using DotPay.RippleDomain.Exceptions;

namespace DotPay.RippleDomain
{
    public class RippleInboundToThirdPartyPaymentTx : DomainBase, IAggregateRoot,
                                                     IEventHandler<RippleInboundTxToThirdPartyPaymentCreated>,
                                                     IEventHandler<RippleInboundTxToThirdPartyPaymentCompelted>
    {
        #region ctor
        protected RippleInboundToThirdPartyPaymentTx() { }
        public RippleInboundToThirdPartyPaymentTx(PayWay payway, string destination, string realName = "", decimal amount = 0M, string memo = "")
        {
            this.RaiseEvent(new RippleInboundTxToThirdPartyPaymentCreated(payway, destination, realName, amount, memo));
        }

        #endregion
        public virtual int ID { get; protected set; }
        public virtual string InvoiceID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual RippleTransactionState State { get; protected set; }
        public virtual string Destination { get; protected set; }
        public virtual string RealName { get; protected set; }
        public virtual string Memo { get; protected set; }
        public virtual decimal Amount { get; protected set; }

        public virtual int CreateAt { get; protected set; }
        public virtual int DoneAt { get; protected set; }

        public virtual void Complete(string txid, decimal amount)
        {
            if (this.State != RippleTransactionState.Pending)
                throw new RippleTransactionNotPendingException();
            if (this.Amount != 0 && this.Amount != amount)
                throw new RippleTransactionAmountNotMatchException();
            else
                this.RaiseEvent(new RippleInboundTxToThirdPartyPaymentCompelted(txid, this.PayWay, amount));
        }

        #region Event handler
        void IEventHandler<RippleInboundTxToThirdPartyPaymentCreated>.Handle(RippleInboundTxToThirdPartyPaymentCreated @event)
        {
            this.TxID = Guid.NewGuid().ToString();
            this.PayWay = @event.PayWay;
            this.State = RippleTransactionState.Pending;
            this.Destination = @event.Destination;
            this.RealName = @event.RealName;
            this.Amount = @event.Amount;
            this.InvoiceID = Utilities.SHA256Sign(@event.Destination + @event.Amount + @event.UTCTimestamp.ToUnixTimestamp());
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.Memo = @event.Memo;
        }
        #endregion

        void IEventHandler<RippleInboundTxToThirdPartyPaymentCompelted>.Handle(RippleInboundTxToThirdPartyPaymentCompelted @event)
        {
            this.TxID = @event.RippleTxID;
            this.State = RippleTransactionState.Success;
            this.Amount = @event.Amount;
        }
    }
}
