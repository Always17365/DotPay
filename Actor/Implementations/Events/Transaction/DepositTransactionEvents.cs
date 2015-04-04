using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common.Enum;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class DepositTransactionInitializedEvent : GrainEvent
    {
        public DepositTransactionInitializedEvent(Guid transactionId, string sequenceNo, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            this.TransactionId = transactionId;
            this.SequenceNo = sequenceNo;
            this.AccountId = accountId;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.Payway = payway;
        }

        public Guid TransactionId { get; private set; }
        public string SequenceNo { get; private set; }
        public Guid AccountId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public Payway Payway { get; private set; }
        public string Memo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class DepositTransactionPreparationCompletedEvent : GrainEvent
    {
        public DepositTransactionPreparationCompletedEvent(Guid transactionId, Guid accountId, decimal amount)
        {
            this.TransactionId = transactionId;
            this.AccountId = accountId;
            this.Amount = amount;
        }

        public Guid TransactionId { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class DepositTransactionConfirmedEvent : GrainEvent
    {
        public DepositTransactionConfirmedEvent(Guid? mannagerId, string transactionNo)
        {
            this.ManagerId = mannagerId;
            this.TransactionNo = transactionNo;
        }

        public Guid? ManagerId { get; private set; }
        public string TransactionNo { get; private set; }
    }
    [Immutable]
    [Serializable]
    public class DepositTransactionFailedEvent : GrainEvent
    {
        public DepositTransactionFailedEvent(Guid managerId, Guid accountId, string reason)
        {
            this.ManagerId = managerId;
            this.AccountId = accountId;
            this.Reason = reason;
        }

        public Guid ManagerId { get; private set; }
        public Guid AccountId { get; private set; }
        public string Reason { get; private set; }
    }
}
