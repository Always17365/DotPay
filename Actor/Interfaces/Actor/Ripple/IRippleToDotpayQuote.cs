using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    public interface IRippleToDotpayQuote : IGrainWithIntegerKey
    {
        Task Initialize(Guid userId, string invoiceId, CurrencyType currency, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Complete(string invoiceId, string txId, decimal sendAmount);
        Task<RippleToDotpayQuoteInfo> GetQuoteInfo();
    }

    [Immutable]
    [Serializable]
    public class RippleToDotpayQuoteInfo
    {
        public RippleToDotpayQuoteInfo(Guid userId, string invoiceId, CurrencyType currency, decimal amount, decimal sendAmount, string memo)
        {
            this.UserId = userId;
            this.InvoiceId = invoiceId;
            this.Currency = currency;
            this.Amount = amount;
            this.SendAmount = sendAmount;
            this.Memo = memo;
        }

        public Guid UserId { get; set; }
        public string InvoiceId { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal SendAmount { get; set; }
        public string Memo { get; set; }
    }
}
