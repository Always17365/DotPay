 
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations.Actors.Transaction
{
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class RefundTransaction : EventSourcingGrain<RefundTransaction, IRefundTransactionState>, IRefundTransaction
    {
        #region IRefundTransaction
        Task IRefundTransaction.Initiliaze(Guid sourceTransactionId, Guid accountId, RefundTransactionType refundTransactionType, CurrencyType currency,
            decimal amount)
        {
            if (this.State.Status < RefundTransactionStatus.Initalized)
            {
                return this.ApplyEvent(new RefundTransactionInitializedEvent(sourceTransactionId, accountId,
                     refundTransactionType, currency, amount));
            }

            return TaskDone.Done;
        }

        Task IRefundTransaction.ConfirmRefundPreparation()
        {
            if (this.State.Status == RefundTransactionStatus.Initalized)
            {
                return this.ApplyEvent(new RefundTransactionPreparationCompletedEvent());
            }

            return TaskDone.Done;
        }

        Task IRefundTransaction.Confirm()
        {
            if (this.State.Status == RefundTransactionStatus.PreparationCompleted)
            {
                return this.ApplyEvent(new RefundTransactionConfirmedEvent());
            }

            return TaskDone.Done;
        }

        public Task<RefundTransactionStatus> GetStatus()
        {
            return Task.FromResult(this.State.Status);
        }

        #endregion

        #region Event Handlers

        private void Handle(RefundTransactionInitializedEvent @event)
        {
            this.State.SourceTransactionId = @event.SourceTransactionId;
            this.State.AccountId = @event.AccountId;
            this.State.Status = RefundTransactionStatus.Initalized;
            this.State.Currency = @event.Currency;
            this.State.Amount = @event.Amount;
            this.State.CreateAt = @event.UTCTimestamp;
        }
        private void Handle(RefundTransactionPreparationCompletedEvent @event)
        {
            this.State.Status = RefundTransactionStatus.PreparationCompleted;
        }
        private void Handle(RefundTransactionConfirmedEvent @event)
        {
            this.State.Status = RefundTransactionStatus.Completed;
            this.State.CompleteAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }

        #endregion
    }

    public interface IRefundTransactionState : IEventSourcingState
    {
        Guid SourceTransactionId { get; set; }
        Guid AccountId { get; set; }
        RefundTransactionStatus Status { get; set; }
        CurrencyType Currency { get; set; }
        decimal Amount { get; set; }
        DateTime CreateAt { get; set; }
        DateTime CompleteAt { get; set; }
    }
}
