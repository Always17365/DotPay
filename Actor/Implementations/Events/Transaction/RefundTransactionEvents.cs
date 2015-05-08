using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class RefundTransactionInitializedEvent : GrainEvent
    {
        public RefundTransactionInitializedEvent(Guid transactionId, Guid sourceTransactionId, Guid accountId, RefundTransactionType refundTransactionType, CurrencyType currency, decimal amount)
        {
            this.TransactionId = transactionId;
            this.SourceTransactionId = sourceTransactionId;
            this.AccountId = accountId;
            this.RefundTransactionType = refundTransactionType;
            this.Currency = currency;
            this.Amount = amount;
        }

        public Guid TransactionId { get; private set; }
        public Guid SourceTransactionId { get; private set; }
        public Guid AccountId { get; private set; }
        public RefundTransactionType RefundTransactionType { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class RefundTransactionPreparationCompletedEvent : GrainEvent
    {
    }
    [Immutable]
    [Serializable]
    public class RefundTransactionConfirmedEvent : GrainEvent
    {
        public RefundTransactionConfirmedEvent()
        {
        }
    }
}
