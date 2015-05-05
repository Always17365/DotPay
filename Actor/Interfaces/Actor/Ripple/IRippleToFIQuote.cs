using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    public interface IRippleToFIQuote : IGrainWithIntegerKey
    {
        Task Initialize(string invoiceId, Payway payway, string destination, string realName, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Complete(string invoiceId, string txId, decimal sendAmount);
        Task<RippleToFiQuoteInfo> GetQuoteInfo();
    }

    [Immutable]
    [Serializable]
    public class RippleToFiQuoteInfo
    {
        public RippleToFiQuoteInfo(string invoiceId,  Payway payway,string destination,string realName,CurrencyType currency, decimal amount, decimal sendAmount, string memo)
        {
            this.InvoiceId = invoiceId;
            this.Payway = payway;
            this.Destination = destination;
            this.RealName = realName;
            this.Currency = currency;
            this.Amount = amount;
            this.SendAmount = sendAmount;
            this.Memo = memo;
        }

        public string InvoiceId { get; set; }
        public Payway Payway { get; set; }
        public string Destination { get; set; }
        public string RealName { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal SendAmount { get; set; }
        public string Memo { get; set; }
    }
}
