using System;
using System.Threading.Tasks;
using Dotpay.Actor.Events;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    /// <summary>
    /// Orleans grain implementation class TransferTransaction
    /// </summary>
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class TransferTransaction : EventSourcingGrain<TransferTransaction, ITransferTransactionState>, ITransferTransaction
    {
        #region ITransferToFITransaction

        async Task ITransferTransaction.Initialize(string sequenceNo, TransferTransactionInfo transferTransactionInfo)
        {
            if (this.State.Status < TransferTransactionStatus.Submited)
                await this.ApplyEvent(new TransferInitilizedEvent(this.GetPrimaryKey(), sequenceNo, transferTransactionInfo));
        }

        async Task ITransferTransaction.ConfirmTransactionPreparation()
        {
            if (this.State.Status == TransferTransactionStatus.Submited)
            {
                await this.ApplyEvent(new ConfirmedTransferTransactionPreparationEvent(this.State.TransactionInfo));
            }
        }

        async Task ITransferTransaction.Cancel(TransferTransactionCancelReason cancelReason)
        {
            if (this.State.Status == TransferTransactionStatus.Submited)
            {
                await this.ApplyEvent(new TransferTransactionCanceldEvent(cancelReason));
            }
        }

        #region Common Transfer
        async Task<ErrorCode> ITransferTransaction.MarkAsProcessing(Guid managerId)
        {
            if (this.State.Status == TransferTransactionStatus.PreparationCompleted && this.State.TransactionInfo.Target.Payway != Payway.Ripple)
            {
                var manager = GrainFactory.GetGrain<IManager>(managerId);
                var loginName = await manager.GetManagerLoginName();
                await this.ApplyEvent(new TransferTransactionMarkedAsProcessingEvent(managerId, loginName));
                return ErrorCode.None;
            }
            else if (this.State.ManagerId != managerId)
                return ErrorCode.TranasferTransactionIsLockedByOther;

            return ErrorCode.None;
        }

        async Task<ErrorCode> ITransferTransaction.ConfirmComplete(Guid managerId, string transferTxNo)
        {
            if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.ManagerId == managerId && this.State.TransactionInfo.Target.Payway != Payway.Ripple)
            {
                await this.ApplyEvent(new TransferTransactionConfirmCompletedEvent(managerId, transferTxNo));
                return ErrorCode.None;
            }
            else if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.ManagerId != managerId)
                return ErrorCode.TranasferTransactionIsLockedByOther;
            else
                return ErrorCode.None;
        }

        async Task<ErrorCode> ITransferTransaction.ConfirmFail(Guid managerId, string reason)
        {
            if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.ManagerId == managerId && this.State.TransactionInfo.Target.Payway != Payway.Ripple)
            {
                var manager = GrainFactory.GetGrain<IManager>(managerId);
                var loginName = await manager.GetManagerLoginName();
                await this.ApplyEvent(new TransferTransactionConfirmedFailEvent(managerId, reason, loginName));
                return ErrorCode.None;
            }
            else if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.ManagerId != managerId)
                return ErrorCode.TranasferTransactionIsLockedByOther;
            else
                return ErrorCode.None;
        }
        #endregion

        #region Transfer To Ripple
        async Task<bool> ITransferTransaction.SubmitToRipple()
        {
            if (this.State.Status == TransferTransactionStatus.PreparationCompleted &&
                this.State.RippleTxStatus == RippleTransactionStatus.Initialized &&
                this.State.TransactionInfo.Target.Payway == Payway.Ripple)
            {
                await this.ApplyEvent(new TransferTransactionSubmitedToRippleEvent());
                return true;
            }

            return false;
        }

        async Task<bool> ITransferTransaction.ReSubmitToRipple()
        {
            if (this.State.Status == TransferTransactionStatus.PreparationCompleted &&
                this.State.RippleTxStatus == RippleTransactionStatus.Submited &&
                !string.IsNullOrEmpty(this.State.RippleTransactionInfo.RippleTxId) &&
                this.State.TransactionInfo.Target.Payway == Payway.Ripple)
            {
                await this.ApplyEvent(new TransferTransactionResubmitedToRippleEvent(this.State.RippleTransactionInfo.RippleTxId));
                return true;
            }

            return false;
        }

        Task ITransferTransaction.RippleTransactionPersubmit(string rippleTxId, long lastLedgerIndex)
        {
            if (this.State.Status == TransferTransactionStatus.PreparationCompleted &&
                this.State.RippleTxStatus.GetValueOrDefault() == RippleTransactionStatus.Submited &&
                this.State.TransactionInfo.Target.Payway == Payway.Ripple)
            {
                var retryCount = this.State.RippleTransactionInfo == null
                    ? 0
                    : this.State.RippleTransactionInfo.RetryCount + 1;
                var rippleTransactionInfo = new RippleTransactionInfo(rippleTxId, lastLedgerIndex, retryCount);
                return this.ApplyEvent(new TransferTransactionConfirmedRipplePresubmitEvent(rippleTransactionInfo));
            }

            return TaskDone.Done;
        }

        Task ITransferTransaction.RippleTransactionComplete(string rippleTxId)
        {
            if (rippleTxId != this.State.RippleTransactionInfo.RippleTxId)
                this.GetLogger().Warn(-1, "TransferTransaction To Ripple TxId Not Match! currenct={0}, param={1}", this.State.RippleTransactionInfo.RippleTxId, rippleTxId);

            if (this.State.Status == TransferTransactionStatus.PreparationCompleted &&
                this.State.RippleTxStatus.GetValueOrDefault() == RippleTransactionStatus.Submited &&
                this.State.TransactionInfo.Target.Payway == Payway.Ripple)
            {
                return this.ApplyEvent(new TransferTransactionConfirmedRippleTxCompleteEvent(rippleTxId));
            }

            return TaskDone.Done;
        }

        async Task<bool> ITransferTransaction.RippleTransactionFail(string rippleTxId, RippleTransactionFailedType failedReason)
        {
            if (rippleTxId != this.State.RippleTransactionInfo.RippleTxId)
                this.GetLogger().Warn(-1, "TransferTransaction To Ripple TxId Not Match! currenct={0}, param={1}", this.State.RippleTransactionInfo.RippleTxId, rippleTxId);

            if (this.State.Status == TransferTransactionStatus.PreparationCompleted &&
                this.State.RippleTxStatus.GetValueOrDefault() == RippleTransactionStatus.Submited &&
                this.State.TransactionInfo.Target.Payway == Payway.Ripple)
            {
                await this.ApplyEvent(new TransferTransactionConfirmedRippleTxFailEvent(rippleTxId, failedReason));
                return true;
            }

            return false;
        }

        public Task<TransferTransactionStatus> GetStatus()
        {
            return Task.FromResult(this.State.Status);
        }

        public Task<RippleTransactionStatus> GetRippleTransactionStatus()
        {
            return Task.FromResult(this.State.RippleTxStatus.GetValueOrDefault());
        }

        public Task<TransferTransactionInfo> GetTransactionInfo()
        {
            return Task.FromResult(this.State.TransactionInfo);
        }

        public Task<RippleTransactionInfo> GetRippleTransactionInfo()
        {
            return Task.FromResult(this.State.RippleTransactionInfo);
        }

        #endregion

        #endregion

        #region Events Handler
        private void Handle(TransferInitilizedEvent @event)
        {
            this.State.Id = @event.TransactionId;
            this.State.SequenceNo = @event.SequenceNo;
            this.State.TransactionInfo = @event.TransferTransactionInfo;
            this.State.Status = TransferTransactionStatus.Submited;

            if (@event.TransferTransactionInfo.Target.Payway == Payway.Ripple)
            {
                this.State.RippleTxStatus = RippleTransactionStatus.Initialized;
            }

            this.State.CreateAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }

        private void Handle(ConfirmedTransferTransactionPreparationEvent @event)
        {
            this.State.Status = TransferTransactionStatus.PreparationCompleted;
            this.State.WriteStateAsync();
        }
        private void Handle(TransferTransactionCanceldEvent @event)
        {
            this.State.Status = TransferTransactionStatus.Canceled;
            this.State.WriteStateAsync();
        }
        #region Common Transfer Events Handler
        private void Handle(TransferTransactionMarkedAsProcessingEvent @event)
        {
            this.State.Status = TransferTransactionStatus.LockeByProcessor;
            this.State.ManagerId = @event.ManagerId;
            this.State.Manager = @event.ManagerName;
            this.State.LockAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(TransferTransactionConfirmCompletedEvent @event)
        {
            this.State.Status = TransferTransactionStatus.Completed;
            this.State.FiTransactionNo = @event.FiTransactionNo;
            this.State.CompleteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        //如果转账失败，考虑把钱自动退到账户上
        private void Handle(TransferTransactionConfirmedFailEvent @event)
        {
            this.State.Status = TransferTransactionStatus.Failed;
            this.State.ManagerId = @event.ManagerId;
            this.State.Manager = @event.Manager;
            this.State.Reason = @event.Reason;
            this.State.FailAt = @event.UTCTimestamp;

            this.State.WriteStateAsync();
        }
        #endregion

        #region Transfer To Ripple Events Handler
        private void Handle(TransferTransactionSubmitedToRippleEvent @event)
        {
            this.State.RippleTxStatus = RippleTransactionStatus.Submited;
        }

        private void Handle(TransferTransactionResubmitedToRippleEvent @event)
        {
            this.State.RippleTxStatus = RippleTransactionStatus.Submited;
        }
        private void Handle(TransferTransactionConfirmedRipplePresubmitEvent @event)
        {
            this.State.RippleTransactionInfo = @event.RippleTransactionInfo;
        }
        private void Handle(TransferTransactionConfirmedRippleTxCompleteEvent @event)
        {
            this.State.RippleTransactionInfo.RippleTxId = @event.RippleTxId;
            this.State.Status=TransferTransactionStatus.Completed;
            this.State.RippleTxStatus = RippleTransactionStatus.Completed;
            this.State.CompleteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(TransferTransactionConfirmedRippleTxFailEvent @event)
        {
            this.State.RippleTransactionInfo.RippleTxId = @event.RippleTxId;
            this.State.RippleTransactionInfo.FailReason = @event.FailedReason;
            this.State.RippleTxStatus = RippleTransactionStatus.Failed;
            this.State.Status = TransferTransactionStatus.Failed;
            this.State.FailAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        #endregion
        #endregion

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }
    }

    public interface ITransferTransactionState : IEventSourcingState
    {
        Guid Id { get; set; }
        string SequenceNo { get; set; }
        TransferTransactionInfo TransactionInfo { get; set; }
        TransferTransactionStatus Status { get; set; }
        RippleTransactionStatus? RippleTxStatus { get; set; }
        string FiTransactionNo { get; set; }
        RippleTransactionInfo RippleTransactionInfo { get; set; }
        Guid? ManagerId { get; set; }
        string Manager { get; set; }
        DateTime CreateAt { get; set; }
        DateTime? LockAt { get; set; }
        DateTime? CompleteAt { get; set; }
        DateTime? FailAt { get; set; }
        string Reason { get; set; }
    }
}
