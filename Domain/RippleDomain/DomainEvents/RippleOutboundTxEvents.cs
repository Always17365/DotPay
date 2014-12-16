using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotPay.Common;
using FC.Framework;

namespace DotPay.RippleDomain.Events
{
    public class RippleOutboundTransferTxCreated : DomainEvent
    {
        public RippleOutboundTransferTxCreated(string destination, int destinationTag, string targetCurrency, decimal targetAmount, decimal sendmax, List<List<object>> paths)
        {
            this.Destination = destination;
            this.DestinationTag = destinationTag;
            this.TargetCurrency = targetCurrency;
            this.TargetAmount = targetAmount;
            this.SourceSendMaxAmount = sendmax;
            this.RipplePaths = paths;
        }
        public string Destination { get; protected set; }
        public int DestinationTag { get; protected set; }
        public string TargetCurrency { get; protected set; }
        public decimal TargetAmount { get; protected set; }
        public decimal SourceSendMaxAmount { get; protected set; }
        public List<List<object>> RipplePaths { get; protected set; }
    }

    public class RippleOutboundTransferSigned : DomainEvent
    {
        public RippleOutboundTransferSigned(int outboundTxId, string txhash, string txblob)
        {
            this.OutboundTxId = outboundTxId;
            this.Txhash = txhash;
            this.Txblob = txblob;
        }
        public int OutboundTxId { get; protected set; }
        public string Txhash { get; protected set; }
        public string Txblob { get; protected set; }
    }


    public class RippleOutboundTransferSubmitSuccess : DomainEvent
    {
        public RippleOutboundTransferSubmitSuccess(int outboundTxId, string txid)
        {
            this.OutboundTxId = outboundTxId;
            this.TxId = txid;
        }
        public int OutboundTxId { get; protected set; }
        public string TxId { get; protected set; } 
    } 
    public class RippleOutboundTransferSubmitFail : DomainEvent
    {
        public RippleOutboundTransferSubmitFail(int outboundTxId, string txid, string reason)
        {
            this.OutboundTxId = outboundTxId;
            this.TxId = txid;
            this.Reason = reason; 
        }
        public int OutboundTxId { get; protected set; }
        public string TxId { get; protected set; }
        public string Reason { get; protected set; }
    }

    //    public int RippleActiveTxID { get; protected set; }
    //}
    //public class RippleInboundTxFailed : DomainEvent
    //{
    //    #region ctor
    //    public RippleInboundTxFailed(int rippleActiveTxID, string reason)
    //    {
    //        this.RippleActiveTxID = rippleActiveTxID;
    //        this.Reason = reason;
    //    }
    //    #endregion

    //    public int RippleActiveTxID { get; protected set; }
    //    public string Reason { get; protected set; }
    //}
}