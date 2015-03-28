
using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Dotpay.Actor.Events;
﻿using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Ripple.Interfaces;
using Orleans;
﻿using Orleans.Concurrency;
﻿using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actor.Implementations
{ 
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class RippleToFinancialInstitution : EventSourcingGrain<RippleToFinancialInstitution, IRippleToFinancialInstitutionState>, IRippleToFinancialInstitution
    {
        #region IRippleToFinancialInstitution
        Task IRippleToFinancialInstitution.Initialize(string invoiceId, TransferToFinancialInstitutionTargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo)
        {
            if (string.IsNullOrEmpty(this.State.InvoiceId))
                return this.ApplyEvent(new RippleToFinancialInstitutionInitialized(invoiceId, transferTargetInfo, amount, sendAmount, memo));

            return TaskDone.Done;
        }
        async Task<ErrorCode> IRippleToFinancialInstitution.Complete(string invoiceId, string txId, decimal sendAmount)
        {
            if (this.State.InvoiceId != invoiceId) return ErrorCode.RippleTransactionInvoiceIdNotMatch;
            if (this.State.SendAmount != sendAmount) return ErrorCode.RippleTransactionAmountNotMatch;
            if (this.State.ReceiveAt.HasValue) return ErrorCode.None;

            await this.ApplyEvent(new RippleToFinancialInstitutionCompleted(invoiceId, txId, sendAmount));
            return ErrorCode.None;
        }
        #endregion

        #region Event Handlers
        private void Handle(RippleToFinancialInstitutionInitialized @event)
        {
            this.State.InvoiceId = @event.InvoiceId;
            this.State.TransferTargetInfo = @event.TransferTargetInfo;
            this.State.Amount = @event.Amount;
            this.State.SendAmount = @event.SendAmount;
            this.State.Memo = @event.Memo;
            this.State.CreateAt = @event.UTCTimestamp;
        }
        private void Handle(RippleToFinancialInstitutionCompleted @event)
        {
            this.State.TxId = @event.TxId;
            this.State.ReceiveAt = @event.UTCTimestamp;
            this.State.WriteStateAsync();
        }
        #endregion
    }


    public interface IRippleToFinancialInstitutionState : IEventSourcingState
    {
        string InvoiceId { get; set; }
        string TxId { get; set; }
        TransferTargetInfo TransferTargetInfo { get; set; }
        decimal Amount { get; set; }
        decimal SendAmount { get; set; }
        string Memo { get; set; }
        DateTime CreateAt { get; set; }
        DateTime? ReceiveAt { get; set; }
    }
}
