using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    //因为不在支持类型adfsf:alipay@dotpay.co这种方式，所以这个事情去掉，暂时保留注释代码
    //public class RippleInboundTxToThirdPartyPaymentCreated : DomainEvent
    //{
    //    #region ctor
    //    public RippleInboundTxToThirdPartyPaymentCreated(PayWay payway,string destination)
    //    {
    //        this.PayWay = payway;
    //        this.Destination = destination;
    //    }
    //    #endregion
    //    public PayWay PayWay { get; protected set; }
    //    public string Destination { get; protected set; }
    //} 
    public class RippleInboundTxToThirdPartyPaymentCreated : DomainEvent
    {
        #region ctor
        public RippleInboundTxToThirdPartyPaymentCreated(PayWay payway, string destination, string realName, decimal amount, string memo)
        {
            this.PayWay = payway;
            this.Destination = destination;
            this.Amount = amount;
            this.Memo = memo;
            this.RealName = realName;
        }
        #endregion
        public PayWay PayWay { get; protected set; }
        public string RealName { get; protected set; }
        public string Destination { get; protected set; }
        public decimal Amount { get; protected set; }
        public string Memo { get; protected set; }
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
