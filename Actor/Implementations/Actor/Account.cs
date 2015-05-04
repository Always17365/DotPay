using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Events;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class Account : EventSourcingGrain<Account, IAccountState>, IAccount
    {
        #region IAccount
        async Task<ErrorCode> IAccount.Initialize(Guid ownerId)
        {
            if (this.State.OwnerId.IsNullOrEmpty())
            {
                await this.ApplyEvent(new AccountInitializeEvent(this.GetPrimaryKey(), ownerId));
                return ErrorCode.None;
            }
            else
                return ErrorCode.AccountHasInitilized;
        }

        Task<bool> IAccount.Validate()
        {
            return Task.FromResult(!this.State.OwnerId.IsNullOrEmpty());
        }

        async Task<ErrorCode> IAccount.AddTransactionPreparation(Guid transactionId, TransactionType transactionType,
            PreparationType preparationType, CurrencyType currency, decimal amount)
        {
            if (this.State.TransactionPreparations == null) this.State.TransactionPreparations = new Dictionary<Guid, TransactionPreparation>();
            if (preparationType == PreparationType.DebitPreparation && this.GetAvailableBalance(currency) < amount)
                return ErrorCode.AccountBalanceNotEnough;

            if (!this.State.TransactionPreparations.ContainsKey(transactionId))
            {
                await
                    this.ApplyEvent(
                        new TransactionPreparationAddedEvent(new TransactionPreparation(transactionId, transactionType,
                            preparationType, currency, amount)));
            }

            return ErrorCode.None;
        }

        async Task IAccount.CommitTransactionPreparation(Guid transactionId)
        {
            TransactionPreparation transferTransactionPreparationInfo;
            if (this.State.TransactionPreparations != null && this.State.TransactionPreparations.TryGetValue(transactionId, out transferTransactionPreparationInfo))
            {
                var currentBalance = this.GetCurrentBalance(transferTransactionPreparationInfo.Currency);
                if (transferTransactionPreparationInfo.PreparationType == PreparationType.DebitPreparation)
                    currentBalance -= transferTransactionPreparationInfo.Amount;
                else
                    currentBalance += transferTransactionPreparationInfo.Amount;

                await this.ApplyEvent(new TransactionPreparationCommittedEvent(transferTransactionPreparationInfo, currentBalance));
            }
        }

        async Task IAccount.CancelTransactionPreparation(Guid transactionId)
        {
            TransactionPreparation transferTransactionPreparationInfo;

            if (this.State.TransactionPreparations != null && this.State.TransactionPreparations.TryGetValue(transactionId, out transferTransactionPreparationInfo))
            {
                await this.ApplyEvent(new TransactionPreparationCanceledEvent(transferTransactionPreparationInfo));
            }
        }

        Task<decimal> IAccount.GetBalance(CurrencyType currency)
        {
            return Task.FromResult(this.GetCurrentBalance(currency));
        }

        Task<Dictionary<CurrencyType, decimal>> IAccount.GetBalances()
        {
            var dic = new Dictionary<CurrencyType, decimal>();
            this.State.Balances.ForEach(b =>
            {
                dic.Add(b.Currency, b.Amount);
            });
            return Task.FromResult(dic);
        }

        public Task<Guid> GetOwnerId()
        {
            return Task.FromResult(this.State.OwnerId);
        }

        #endregion

        #region Event Handlers
        private void Handle(AccountInitializeEvent @event)
        {
            var currencyList = new List<AccountBalance>
            {
                new AccountBalance() { Currency = CurrencyType.Cny ,Amount = 0} 
            };

            this.State.Id = @event.AccountId;
            this.State.OwnerId = @event.OwnerId;
            this.State.Balances = currencyList;
            this.State.TransactionPreparations = new Dictionary<Guid, TransactionPreparation>();

            this.State.WriteStateAsync();
        }

        private void Handle(TransactionPreparationAddedEvent @event)
        {
            this.State.TransactionPreparations.Add(@event.TransferTransactionPreparation.TransactionId, @event.TransferTransactionPreparation);
        }
        private void Handle(TransactionPreparationCommittedEvent @event)
        {
            this.State.TransactionPreparations.Remove(@event.TransactionPreparation.TransactionId);

            var balance = this.State.Balances.SingleOrDefault(b => b.Currency == @event.TransactionPreparation.Currency);

            if (balance == null)
            {
                balance = new AccountBalance()
                {
                    Currency = @event.TransactionPreparation.Currency
                };
                this.State.Balances.Add(balance);
            }

            balance.Amount = @event.CurrentBalance;

            this.State.WriteStateAsync();
        }
        private void Handle(TransactionPreparationCanceledEvent @event)
        {
            this.State.TransactionPreparations.Remove(@event.TransactionPreparation.TransactionId);
        }
        #endregion

        #region Private Methods
        private decimal GetAvailableBalance(CurrencyType currency)
        {
            var availableBalance = this.GetCurrentBalance(currency);

            if (this.State.TransactionPreparations.Any())
            {
                var totalDebitTransactionPreparationAmount = 0M;
                var debitTransactionPreparations = this.State.TransactionPreparations.Values
                                                             .Where(x => x.PreparationType == PreparationType.DebitPreparation &&
                                                                         x.Currency == currency);
                if (debitTransactionPreparations.Any())
                    totalDebitTransactionPreparationAmount = debitTransactionPreparations.Sum(debitTransactionPreparation => debitTransactionPreparation.Amount);

                availableBalance = availableBalance - totalDebitTransactionPreparationAmount;
            }

            return availableBalance;
        }
        private decimal GetCurrentBalance(CurrencyType currency)
        {
            var balance = this.State.Balances.SingleOrDefault(b => b.Currency == currency);

            return balance != null ? balance.Amount : 0M;
        }
        #endregion
    }

    public interface IAccountState : IEventSourcingState
    {
        Guid Id { get; set; }
        Dictionary<Guid, TransactionPreparation> TransactionPreparations { get; set; }
        Guid OwnerId { get; set; }
        List<AccountBalance> Balances { get; set; }
    }

    public class AccountBalance
    {
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
