using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.EventSourcing;
using Dotpay.Common;
using Dotpay.Actor;
using Orleans.Concurrency;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RippleToFinancialInstitutionInitialized : GrainEvent
    {
        public RippleToFinancialInstitutionInitialized(long transactionId, string invoiceId, TransferTargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo)
        {
            this.TransactionId = transactionId;
            this.InvoiceId = invoiceId;
            this.TransferTargetInfo = transferTargetInfo;
            this.Amount = amount;
            this.SendAmount = sendAmount;
            this.Memo = memo;
        }
        public long TransactionId { get; private set; }
        public string InvoiceId { get; private set; }
        public TransferTargetInfo TransferTargetInfo { get; private set; }
        public decimal Amount { get; private set; }
        public decimal SendAmount { get; private set; }
        public string Memo { get; private set; }
    }

    public class RippleToFinancialInstitutionCompleted : GrainEvent
    {
        public RippleToFinancialInstitutionCompleted(string invoiceId, string txId, decimal sendAmount)
        {
            this.InvoiceId = invoiceId;
            this.TxId = txId;
            this.SendAmount = sendAmount;
        }

        public string InvoiceId { get; private set; }
        public string TxId { get; private set; }
        public decimal SendAmount { get; private set; }
    }

}
