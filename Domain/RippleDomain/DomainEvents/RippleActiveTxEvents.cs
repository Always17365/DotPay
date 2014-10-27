using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    public class RippleAciveTxCreated : DomainEvent
    {
        #region ctor
        public RippleAciveTxCreated(string txid, string txblob, string source, string destination, decimal amount, decimal fee)
        {
            this.TxID = txid;
            this.TxBlob = txblob;
            this.Source = source;
            this.Destination = destination;
            this.Amount = amount;
            this.Fee = fee;
        }
        #endregion
        public string TxID { get; protected set; }
        public string TxBlob { get; protected set; }
        public string Source { get; protected set; }
        public string Destination { get; protected set; }
        public decimal Amount { get; protected set; }
        public decimal Fee { get; protected set; }
    }

    public class RippleAciveTxCompelted : DomainEvent
    {
        #region ctor
        public RippleAciveTxCompelted(int rippleActiveTxID)
        {
            this.RippleActiveTxID = rippleActiveTxID;
        }
        #endregion

        public int RippleActiveTxID { get; protected set; }
    }
    public class RippleAciveTxFailed : DomainEvent
    {
        #region ctor
        public RippleAciveTxFailed(int rippleActiveTxID, string reason)
        {
            this.RippleActiveTxID = rippleActiveTxID;
            this.Reason = reason;
        }
        #endregion

        public int RippleActiveTxID { get; protected set; }
        public string Reason { get; protected set; }
    }
}
