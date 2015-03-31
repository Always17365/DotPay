using System;
using System.Threading.Tasks;
using Dotpay.Actor.Events;
using Dotpay.Actor.Interfaces;
using Dotpay.Common.Enum;
 using Orleans;
using Orleans.Concurrency;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class DepositTransaction : EventSourcingGrain<DepositTransaction, IDepositTransactionState>, IDepositTransaction
    {
        #region IDeposit

        async Task IDepositTransaction.Initiliaze(string sequenceNo, Guid accountId, CurrencyType currency, decimal amount,
                                                  Payway payway, string memo)
        {
            if (!(this.State.Status >= DepositStatus.Started))
                await this.ApplyEvent(new DepositTransactionInitializedEvent(sequenceNo, accountId, currency, amount, payway, memo));
        }
        async Task IDepositTransaction.ConfirmDepositPreparation()
        {
            if (this.State.Status == DepositStatus.Started)
                await
                    this.ApplyEvent(new DepositTransactionPreparationCompletedEvent(this.GetPrimaryKey(),
                        this.State.AccountId, this.State.Amount));
        }

        async Task IDepositTransaction.ConfirmDeposit(Guid? managerId, string transferNo)
        {
            if (this.State.Status == DepositStatus.PreparationCompleted)
                await this.ApplyEvent(new DepositTransactionConfirmedEvent(managerId, transferNo));
        }

        async Task IDepositTransaction.Fail(Guid managerId, string reason)
        {
            if (this.State.Status == DepositStatus.PreparationCompleted)
                await this.ApplyEvent(new DepositTransactionFailedEvent(managerId, this.State.AccountId, reason));
        }

        public Task<DepositTransactionInfo> GetTransactionInfo()
        {
            var depositTxInfo = new DepositTransactionInfo(this.State.AccountId, this.State.Currency,
                this.State.Amount, this.State.Payway, this.State.Memo);

            return Task.FromResult(depositTxInfo);
        }

        public Task<DepositStatus> GetStatus()
        {
            return Task.FromResult(this.State.Status);
        }

        #endregion

        #region Event Handlers
        private void Handle(DepositTransactionInitializedEvent @event)
        {
            this.State.SequenceNo = @event.SequenceNo;
            this.State.AccountId = @event.AccountId;
            this.State.Payway = @event.Payway;
            this.State.Status = DepositStatus.Started;
            this.State.Currency = @event.Currency;
            this.State.Amount = @event.Amount;
            this.State.Memo = @event.Memo;
            this.State.CreateAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(DepositTransactionPreparationCompletedEvent @event)
        {
            this.State.Status = DepositStatus.PreparationCompleted;
            this.State.WriteStateAsync();
        }

        private void Handle(DepositTransactionConfirmedEvent @event)
        {
            this.State.ManagerId = @event.ManagerId;
            this.State.TransactionNo = @event.TransactionNo;
            this.State.Status = DepositStatus.Completed;
            this.State.CompleteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(DepositTransactionFailedEvent @event)
        {
            this.State.ManagerId = @event.ManagerId;
            this.State.Status = DepositStatus.Fail;
            this.State.FailAt = @event.UTCTimestamp;
            this.State.FailReason = @event.Reason;
            this.State.WriteStateAsync();
        }
        #endregion
    }

    #region DepositTransaction
    public interface IDepositTransactionState : IEventSourcingState
    {
        Guid AccountId { get; set; }
        string SequenceNo { get; set; }
        CurrencyType Currency { get; set; }
        decimal Amount { get; set; }
        DepositStatus Status { get; set; }
        Payway Payway { get; set; }
        string Memo { get; set; }
        DateTime CreateAt { get; set; }
        Guid? ManagerId { get; set; }
        string TransactionNo { get; set; }
        DateTime? CompleteAt { get; set; }
        DateTime? FailAt { get; set; }
        string FailReason { get; set; }
    }
    #endregion
}
