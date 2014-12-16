using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleCommand
{
    public class CreateRippleOutboundTx : FC.Framework.Command
    {
        public CreateRippleOutboundTx(string destination, int destinationTag, string targetCurrency, decimal targetAmount, decimal sendmax, List<List<object>> paths)
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

    public class SignRippleOutboundTx : FC.Framework.Command
    {
        public SignRippleOutboundTx(int outboundTxid, string txhash, string txblob)
        {
            this.OutboundTxId = outboundTxid;
            this.TxHash = txhash;
            this.TxBlob = txblob;
        }
        public int OutboundTxId { get; protected set; }
        public string TxHash { get; protected set; }
        public string TxBlob { get; protected set; }
    }

    public class SubmitRippleOutboundTxSuccess : FC.Framework.Command
    {
        public SubmitRippleOutboundTxSuccess(string txid)
        {
            this.TxId = txid; 
        }
        public string TxId { get; protected set; }
    }

    public class SubmitRippleOutboundTxFail: FC.Framework.Command
    {
        public SubmitRippleOutboundTxFail(string txid,string reason)
        {
            this.TxId = txid;
            this.Reason = reason;
        }
        public string TxId { get; protected set; }
        public string Reason { get; protected set; }
    }
}
