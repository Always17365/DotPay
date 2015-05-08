using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Events
{
    [Immutable]
    [Serializable]
    public class TransferInitilizedEvent : GrainEvent
    {
        public TransferInitilizedEvent(Guid transactionId, string sequenceNo, TransferTransactionInfo transferTransactionInfo)
        {
            this.TransactionId = transactionId;
            this.SequenceNo = sequenceNo;
            this.TransferTransactionInfo = transferTransactionInfo;
        }

        public Guid TransactionId { get; private set; }
        public string SequenceNo { get; private set; }
        public TransferTransactionInfo TransferTransactionInfo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class ConfirmedTransferTransactionPreparationEvent : GrainEvent
    {
        public ConfirmedTransferTransactionPreparationEvent(TransferTransactionInfo transferTransactionInfo)
        {
            this.TransferTransactionInfo = transferTransactionInfo;
        }

        public TransferTransactionInfo TransferTransactionInfo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionCanceldEvent : GrainEvent
    {
        public TransferTransactionCanceldEvent(TransferTransactionCancelReason cancelReason)
        {
            this.TransferTransactionCancelReason = cancelReason;
        }

        public TransferTransactionCancelReason TransferTransactionCancelReason { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionMarkedAsProcessingEvent : GrainEvent
    {
        public TransferTransactionMarkedAsProcessingEvent(Guid managerId,string managerName)
        {
            this.ManagerId = managerId;
            this.ManagerName = managerName;
        }

        public Guid ManagerId { get; private set; }
        public string ManagerName { get; set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmCompletedEvent : GrainEvent
    {
        public TransferTransactionConfirmCompletedEvent(Guid managerId, string fiTransactionNo)
        {
            this.ManagerId = managerId;
            this.FiTransactionNo = fiTransactionNo;
        }

        public Guid ManagerId { get; private set; }
        public string FiTransactionNo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmedFailEvent : GrainEvent
    {
        public TransferTransactionConfirmedFailEvent(Guid managerId, string reason, string manager)
        {
            this.ManagerId = managerId;
            this.Reason = reason;
            this.Manager = manager;
        }

        public Guid ManagerId { get; private set; }
        public string Manager { get; private set; }
        public string Reason { get; private set; }
    }
    #region Transfer To Ripple Events

    [Immutable]
    [Serializable]
    public class TransferTransactionSubmitedToRippleEvent : GrainEvent
    {
        public TransferTransactionSubmitedToRippleEvent() { }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionResubmitedToRippleEvent : GrainEvent
    {
        public TransferTransactionResubmitedToRippleEvent(string failedRippleTxId)
        {
            this.FailedRippleTxId = failedRippleTxId;
        }

        public string FailedRippleTxId { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmedRipplePresubmitEvent : GrainEvent
    {
        public TransferTransactionConfirmedRipplePresubmitEvent(RippleTransactionInfo rippleTransactionInfo)
        {
            this.RippleTransactionInfo = rippleTransactionInfo;
        }

        public RippleTransactionInfo RippleTransactionInfo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmedRippleTxCompleteEvent : GrainEvent
    {
        public TransferTransactionConfirmedRippleTxCompleteEvent(string rippleTxId)
        {
            this.RippleTxId = rippleTxId;
        }

        public string RippleTxId { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmedRippleTxFailEvent : GrainEvent
    {
        public TransferTransactionConfirmedRippleTxFailEvent(string rippleTxid, RippleTransactionFailedType failedReason)
        {
            this.RippleTxId = rippleTxid;
            this.FailedReason = failedReason;
        }

        public string RippleTxId { get; private set; }
        public RippleTransactionFailedType FailedReason { get; private set; }
    }
    #endregion
}
