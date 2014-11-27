using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    public class RippleInboundTxCreated : DomainEvent
    {
        #region ctor
        public RippleInboundTxCreated(string txid, int destinationTag, decimal amount)
        {
            this.TxID = txid;
            this.DestinationTag = destinationTag;
            this.Amount = amount;
        }
        #endregion
        public string TxID { get; protected set; }
        public int DestinationTag { get; protected set; }
        public decimal Amount { get; protected set; }
    }

    public class RippleInboundTxCompelted : DomainEvent
    {
        #region ctor
        public RippleInboundTxCompelted(int rippleActiveTxID)
        {
            this.RippleActiveTxID = rippleActiveTxID;
        }
        #endregion

        public int RippleActiveTxID { get; protected set; }
    }
    public class RippleInboundTxFailed : DomainEvent
    {
        #region ctor
        public RippleInboundTxFailed(int rippleActiveTxID, string reason)
        {
            this.RippleActiveTxID = rippleActiveTxID;
            this.Reason = reason;
        }
        #endregion

        public int RippleActiveTxID { get; protected set; }
        public string Reason { get; protected set; }
    }
}
