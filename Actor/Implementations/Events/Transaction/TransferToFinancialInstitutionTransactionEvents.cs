using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actors.Implementations.Events.Transaction
{
    [Immutable]
    [Serializable]
    public class TransferToFinancialInstitutionTransactionInitilized : GrainEvent
    {
        public TransferToFinancialInstitutionTransactionInitilized(TransferSourceInfo source, TransferTargetInfo target, string sequenceNo, decimal amount, string memo)
        {
            this.Source = source;
            this.Target = target;
            this.SequenceNo = sequenceNo;
            this.Amount = amount;
            this.Memo = memo;
        }

        public TransferSourceInfo Source { get; private set; }
        public TransferTargetInfo Target { get; private set; }
        public string SequenceNo { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferToFinancialInstitutionTransactionPreparationCompleted : GrainEvent
    {
        public TransferToFinancialInstitutionTransactionPreparationCompleted(TransferSourceInfo source, TransferTargetInfo target, decimal amount)
        {
            this.Source = source;
            this.Target = target;
            this.Amount = amount;
        }
        public TransferSourceInfo Source { get; private set; }
        public TransferTargetInfo Target { get; private set; }
        public decimal Amount { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferToFinancialInstitutionMarkProcessing : GrainEvent
    {
        public TransferToFinancialInstitutionMarkProcessing(Guid operatorId)
        {
            this.OperatorId = operatorId;
        }

        public Guid OperatorId { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferToFinancialInstitutionMarkCompleted : GrainEvent
    {
        public TransferToFinancialInstitutionMarkCompleted(Guid operatorId, string fiTransactionNo)
        {
            this.OperatorId = operatorId;
            this.FiTransactionNo = fiTransactionNo;
        }

        public Guid OperatorId { get; private set; }
        public string FiTransactionNo { get; private set; }
    }
}
