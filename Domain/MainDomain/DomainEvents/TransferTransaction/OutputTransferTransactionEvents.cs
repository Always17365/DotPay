using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class OutputTransferTransactionCreated : DomainEvent
    {
        public OutputTransferTransactionCreated(int fromUserID, string destination, CurrencyType sourceCurrency, decimal sourceAmount,
                                                CurrencyType targetCurrency, decimal targetAmount, PayWay payway)
        {
            this.FromUserID = fromUserID;
            this.Destination = destination;
            this.SourceCurrency = sourceCurrency;
            this.SourceAmount = sourceAmount;
            this.TargetCurrency = targetCurrency;
            this.TargetAmount = targetAmount;
            this.PayWay = payway;
        }
        public int FromUserID { get; private set; }
        public string Destination { get; private set; }
        public CurrencyType SourceCurrency { get; private set; }
        public decimal SourceAmount { get; private set; }
        public CurrencyType TargetCurrency { get; private set; }
        public decimal TargetAmount { get; private set; }
        public PayWay PayWay { get; private set; }
    }

    public class OutputTransferTransactionComplete : DomainEvent
    {
        public OutputTransferTransactionComplete(int OutputTransferID)
        {
            this.OutputTransferID = OutputTransferID;

        }
        public int OutputTransferID { get; private set; }
    }
    public class OutputTransferTransactionFailed : DomainEvent
    {
        public OutputTransferTransactionFailed(int OutputTransferID, string reason)
        {
            this.OutputTransferID = OutputTransferID;
            this.Reason = reason;
        }
        public int OutputTransferID { get; private set; }
        public string Reason { get; private set; }
    }
}
