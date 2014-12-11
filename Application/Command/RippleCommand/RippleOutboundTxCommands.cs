using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleCommand
{
    public class CreateOutboundTx : FC.Framework.Command
    {
        public CreateOutboundTx(string destination, int destinationTag, string targetCurrency, decimal targetAmount, decimal sendmax, List<object> paths)
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
        public List<object> RipplePaths { get; protected set; }
    }

    public class SignOutboundTx : FC.Framework.Command
    {
        public SignOutboundTx(int outboundTxid, string txhash, string txblob)
        {
            this.OutboundTxId = outboundTxid;
            this.TxHash = txhash;
            this.TxBlob = txblob;
        }
        public int OutboundTxId { get; protected set; }
        public string TxHash { get; protected set; }
        public string TxBlob { get; protected set; }
    }

    public class CompleteOutboundTx : FC.Framework.Command
    {
        public CompleteOutboundTx(string destination, int destinationTag, string targetCurrency, decimal targetAmount, decimal sendmax, List<object> paths)
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
        public List<object> RipplePaths { get; protected set; }
    }
}
