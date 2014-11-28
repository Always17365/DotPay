using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    public class RippleInboundTxToThirdPartyPaymentCreated : DomainEvent
    {
        #region ctor
        public RippleInboundTxToThirdPartyPaymentCreated(PayWay payway, string destination)
        {
            this.PayWay = payway;
            this.Destination = destination;
        }
        #endregion
        public PayWay PayWay { get; protected set; }
        public string Destination { get; protected set; }
    }

    public class RippleInboundTxToThirdPartyPaymentCompelted : DomainEvent
    {
        #region ctor
        public RippleInboundTxToThirdPartyPaymentCompelted(string txid, PayWay payway, decimal amount)
        {
            this.RippleTxID = txid;
            this.PayWay = payway;
            this.Amount = amount;
        }
        #endregion

        public string RippleTxID { get; protected set; }
        public PayWay PayWay { get; protected set; }
        public decimal Amount { get; protected set; }
    }
    //public class RippleInboundTxToThirdPartyPaymentFailed : DomainEvent
    //{
    //    #region ctor
    //    public RippleInboundTxToThirdPartyPaymentFailed(int rippleActiveTxID, string reason)
    //    {
    //        this.RippleActiveTxID = rippleActiveTxID;
    //        this.Reason = reason;
    //    }
    //    #endregion

    //    public int RippleActiveTxID { get; protected set; }
    //    public string Reason { get; protected set; }
    //}
}
