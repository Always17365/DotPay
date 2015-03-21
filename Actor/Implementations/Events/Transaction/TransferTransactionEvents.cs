using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Dotpay.Common.Enum;
using Orleans.Concurrency;
using Orleans.EventSourcing;

namespace Dotpay.Actor.Implementations.Events.Transaction
{
    [Immutable]
    [Serializable]
    public class TransferInitilizedEvent : GrainEvent
    {
        public TransferInitilizedEvent(string sequenceNo, TransferTransactionInfo transferTransactionInfo)
        {
            this.SequenceNo = sequenceNo;
            this.TransferTransactionInfo = transferTransactionInfo;
        }

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
        public TransferTransactionMarkedAsProcessingEvent(Guid operatorId)
        {
            this.OperatorId = operatorId;
        }

        public Guid OperatorId { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmCompletedEvent : GrainEvent
    {
        public TransferTransactionConfirmCompletedEvent(Guid operatorId, string fiTransactionNo)
        {
            this.OperatorId = operatorId;
            this.FiTransactionNo = fiTransactionNo;
        }

        public Guid OperatorId { get; private set; }
        public string FiTransactionNo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionConfirmedFailEvent : GrainEvent
    {
        public TransferTransactionConfirmedFailEvent(Guid operatorId, string reason)
        {
            this.OperatorId = operatorId;
            this.Reason = reason;
        }

        public Guid OperatorId { get; private set; }
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
