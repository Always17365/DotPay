using System;
using System.Threading.Tasks;
using Dotpay.Actor.Events;
using Dotpay.Actor.Interfaces;
using Dotpay.Common.Enum;
﻿using Orleans;
using Orleans.Concurrency;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actors.Implementations
{
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class DepositTransaction : EventSourcingGrain<DepositTransaction, IDepositTransactionState>, IDepositTransaction
    {
        #region IDeposit

        async Task IDepositTransaction.Initiliaze(Guid accountId, CurrencyType currency, decimal amount,
                                                  Payway payway, string memo)
        {
            if (!(this.State.Status >= DepositStatus.Started))
                await this.ApplyEvent(new DepositTransactionInitialized(accountId, currency, amount, payway, memo));
        }
        async Task IDepositTransaction.ConfirmDepositPreparation()
        {
            if (this.State.Status == DepositStatus.Started)
                await
                    this.ApplyEvent(new DepositTransactionPreparationCompleted(this.GetPrimaryKey(),
                        this.State.AccountId, this.State.Amount));
        }

        async Task IDepositTransaction.ConfirmDeposit(Guid operatorId, string transferNo)
        {
            if (this.State.Status == DepositStatus.PreparationCompleted)
                await this.ApplyEvent(new DepositTransactionConfirmed(operatorId, transferNo));
        }

        async Task IDepositTransaction.Fail(Guid operatorId, string reason)
        {
            if (this.State.Status == DepositStatus.PreparationCompleted)
                await this.ApplyEvent(new DepositTransactionFailed(operatorId, this.State.AccountId, reason));
        }

        public Task<Immutable<DepositTransactionInfo>> GetTransactionInfo()
        {
            var depositTxInfo = new DepositTransactionInfo(this.State.AccountId, this.State.Currency,
                this.State.Amount, this.State.Payway, this.State.Status);

            return Task.FromResult(depositTxInfo.AsImmutable());
        }

        public Task<Immutable<DepositStatus>> GetStatus()
        {
            return Task.FromResult(this.State.Status.AsImmutable());
        }

        #endregion

        #region Event Handlers
        private void Handle(DepositTransactionInitialized @event)
        {
            this.State.AccountId = @event.AccountId;
            this.State.Payway = @event.Payway;
            this.State.Status = DepositStatus.Started;
            this.State.Currency = @event.Currency;
            this.State.Amount = @event.Amount;
            this.State.Memo = @event.Memo;
            this.State.CreateAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(DepositTransactionPreparationCompleted @event)
        {
            this.State.Status = DepositStatus.PreparationCompleted;
            this.State.PreparteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }

        private void Handle(DepositTransactionConfirmed @event)
        {
            this.State.OperatorId = @event.OperatorId;
            this.State.TransactionNo = @event.TransactionNo;
            this.State.Status = DepositStatus.Completed;
            this.State.CompleteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        private void Handle(DepositTransactionFailed @event)
        {
            this.State.OperatorId = @event.OperatorId;
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
        CurrencyType Currency { get; set; }
        decimal Amount { get; set; }
        DepositStatus Status { get; set; }
        Payway Payway { get; set; }
        string Memo { get; set; }
        DateTime CreateAt { get; set; }
        DateTime PreparteAt { get; set; }
        Guid OperatorId { get; set; }
        string TransactionNo { get; set; }
        DateTime? CompleteAt { get; set; }
        DateTime? FailAt { get; set; }
        string FailReason { get; set; }
    }
    #endregion
}
