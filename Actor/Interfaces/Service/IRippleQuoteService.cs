using System;
using System.Threading.Tasks;
using Dotpay.Actor;
using Dotpay.Actor.Ripple;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service
{
    public interface IRippleQuoteService : IGrainWithIntegerKey
    {
        Task<QuoteResult> Quote(TransferTargetInfo transferTargetInfo, CurrencyType currency, decimal amount, string memo);
    }

    [Immutable]
    [Serializable]
    public class QuoteResult
    {
        public QuoteResult(ErrorCode errorCode, Quote quote = null)
        {
            this.ErrorCode = errorCode;
            this.Quote = quote;
        }

        public Quote Quote { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }

    [Immutable]
    [Serializable]
    public class Quote
    {
        public Quote(long destinationTag, string invoiceId, decimal sendAmount)
        {
            this.DestinationTag = destinationTag;
            this.InvoiceId = invoiceId;
            this.SendAmount = sendAmount;
        }

        public long DestinationTag { get; set; }
        public string InvoiceId { get; set; }
        public decimal SendAmount { get; set; }
    }
}