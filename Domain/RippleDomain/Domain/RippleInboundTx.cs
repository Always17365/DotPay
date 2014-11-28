using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.RippleDomain.Events;

namespace DotPay.RippleDomain
{
    public class RippleInboundTx : DomainBase, IAggregateRoot, IEventHandler<RippleInboundTxCreated>
    {
        #region ctor
        protected RippleInboundTx() { }

        public RippleInboundTx(string txid, int destinationTag, decimal amount)
        {
            this.RaiseEvent(new RippleInboundTxCreated(txid, destinationTag, amount));
        }

        #endregion

        public virtual int ID { get; protected set; }
        public virtual string TxID { get; protected set; }
        public virtual int DestinationTag { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual int CreateAt { get; protected set; }

        #region Event handler
        void IEventHandler<RippleInboundTxCreated>.Handle(RippleInboundTxCreated @event)
        {
            this.TxID = @event.RippleTxID;
            this.DestinationTag = @event.DestinationTag;
            this.Amount = @event.Amount; 
        }
        #endregion
    }
}
