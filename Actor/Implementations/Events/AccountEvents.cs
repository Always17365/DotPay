using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actors.Implementations.Events
{
    #region Events

    [Immutable]
    [Serializable]
    public class AccountInitializeEvent : GrainEvent
    {
        public AccountInitializeEvent(Guid ownerId)
        {
            this.OwnerId = ownerId;
        }
        public Guid OwnerId { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransactionPreparationAddedEvent : GrainEvent
    {
        public TransactionPreparationAddedEvent(TransactionPreparation transferPreparation)
        {
            this.TransferTransactionPreparation = transferPreparation;
        }
        public TransactionPreparation TransferTransactionPreparation { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransactionPreparationCommittedEvent : GrainEvent
    {
        public TransactionPreparationCommittedEvent(TransactionPreparation transactionPreparation, decimal currentBalance)
        {
            this.TransactionPreparation = transactionPreparation;
            this.CurrentBalance = currentBalance;
        }
        public TransactionPreparation TransactionPreparation { get; private set; }
        public decimal CurrentBalance { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransactionPreparationCanceledEvent : GrainEvent
    {
        public TransactionPreparationCanceledEvent(TransactionPreparation transactionPreparation)
        {
            this.TransactionPreparation = transactionPreparation;
        }
        public TransactionPreparation TransactionPreparation { get; private set; }
    }
    #endregion
}
