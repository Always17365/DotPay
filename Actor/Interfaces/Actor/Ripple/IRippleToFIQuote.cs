using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    public interface IRippleToFIQuote : IGrainWithIntegerKey
    {
        Task Initialize(string invoiceId, TransferToFITargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Complete(string invoiceId, string txId, decimal sendAmount);
        Task<RippleToFiQuoteInfo> GetQuoteInfo();
    }

    [Immutable]
    [Serializable]
    public class RippleToFiQuoteInfo
    {
        public RippleToFiQuoteInfo(string invoiceId, TransferToFITargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo)
        {
            this.InvoiceId = invoiceId;
            this.TransferTargetInfo = transferTargetInfo;
            this.Amount = amount;
            this.SendAmount = sendAmount;
            this.Memo = memo;
        }

        public string InvoiceId { get; set; }
        public TransferToFITargetInfo TransferTargetInfo { get; set; }
        public decimal Amount { get; set; }
        public decimal SendAmount { get; set; }
        public string Memo { get; set; }
    }
}
