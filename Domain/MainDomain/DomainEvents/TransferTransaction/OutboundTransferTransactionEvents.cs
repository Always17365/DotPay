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
    public class OutboundTransferTransactionCreated : DomainEvent
    {
        public OutboundTransferTransactionCreated(int fromUserID, string destination, decimal sourceAmount,
                                                CurrencyType targetCurrency, decimal targetAmount, PayWay payway, string description, OutboundTransferTransaction txEntity)
        {
            this.FromUserID = fromUserID;
            this.Destination = destination;
            this.SourceAmount = sourceAmount;
            this.TargetCurrency = targetCurrency;
            this.TargetAmount = targetAmount;
            this.PayWay = payway;
            this.Description = description;
            this.TxEntity = txEntity;
        }
        public int FromUserID { get; private set; }
        public string Destination { get; private set; }
        public decimal SourceAmount { get; private set; }
        public CurrencyType TargetCurrency { get; private set; }
        public decimal TargetAmount { get; private set; }
        public PayWay PayWay { get; private set; }
        public string Description { get; private set; }
        public OutboundTransferTransaction TxEntity { get; private set; }
    }

    public class OutboundTransferTransactionConfirmed : DomainEvent
    {
        public OutboundTransferTransactionConfirmed(int outboundTransferID)
        {
            this.OutboundTransferID = outboundTransferID;
        }
        public int OutboundTransferID { get; private set; }
    }

    public class OutboundTransferTransactionComplete : DomainEvent
    {
        public OutboundTransferTransactionComplete(int outboundTransferID)
        {
            this.OutboundTransferID = outboundTransferID;

        }
        public int OutboundTransferID { get; private set; }
    }
    public class OutboundTransferTransactionFailed : DomainEvent
    {
        public OutboundTransferTransactionFailed(int outboundTransferID, string reason)
        {
            this.OutboundTransferID = outboundTransferID;
            this.Reason = reason;
        }
        public int OutboundTransferID { get; private set; }
        public string Reason { get; private set; }
    }
}
