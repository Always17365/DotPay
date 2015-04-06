using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Providers;
﻿using Orleans.Runtime;

namespace Dotpay.Actor.Implementations
{
    [StorageProvider(ProviderName = Constants.StorageProviderName)]
    public class RippleToFIQuote : Grain<IRippleToFIQuoteState>, IRippleToFIQuote, IRemindable
    {
        async Task IRippleToFIQuote.Initialize(string invoiceId, TransferToFITargetInfo transferTargetInfo, decimal amount,
              decimal sendAmount, string memo)
        {
            if (this.State.Status < RippleTransactionStatus.Completed) return;

            this.State.Id = this.GetPrimaryKeyLong();
            this.State.InvoiceId = invoiceId;
            this.State.TransferTargetInfo = transferTargetInfo;
            this.State.Amount = amount;
            this.State.SendAmount = sendAmount;
            this.State.Memo = memo;
            this.State.CreateAt = DateTime.Now;
            this.State.Status = RippleTransactionStatus.Initialized;
            await this.RegisterOrUpdateReminder(this.GetPrimaryKeyLong().ToString(),
                       TimeSpan.FromDays(7), TimeSpan.FromHours(1));
            await this.State.WriteStateAsync();
        }

        async Task<ErrorCode> IRippleToFIQuote.Complete(string invoiceId, string txId, decimal sendAmount)
        {
            if (this.State.Status == RippleTransactionStatus.Initialized)
            {
                if (this.State.InvoiceId.Equals(invoiceId, StringComparison.OrdinalIgnoreCase) &&
                this.State.SendAmount == sendAmount)
                {
                    this.State.TxId = txId;
                    this.State.Status = RippleTransactionStatus.Completed;
                    this.State.ReceiveAt = DateTime.Now;
                    var reminder = await this.GetReminder(this.GetPrimaryKeyLong().ToString());
                    if (reminder != null) await this.UnregisterReminder(reminder);
                    await this.State.WriteStateAsync();
                }
                else if (!this.State.InvoiceId.Equals(invoiceId, StringComparison.OrdinalIgnoreCase))
                {
                    return ErrorCode.RippleTransactionInvoiceIdNotMatch;
                }
                else if (this.State.SendAmount != sendAmount)
                {
                    return ErrorCode.RippleTransactionAmountNotMatch;
                }
            }

            return ErrorCode.None;
        }

        Task<RippleToFiQuoteInfo> IRippleToFIQuote.GetQuoteInfo()
        {
            return Task.FromResult(new RippleToFiQuoteInfo(this.State.InvoiceId, this.State.TransferTargetInfo,
                this.State.Amount, this.State.SendAmount, this.State.Memo));
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName == this.GetPrimaryKeyLong().ToString())
            {
                if (this.State.Status == RippleTransactionStatus.Initialized)
                {
                    await this.State.ClearStateAsync();
                    var reminder = await this.GetReminder(this.GetPrimaryKeyLong().ToString());
                    if (reminder != null) await this.UnregisterReminder(reminder);
                }
            }
        }
    }

    public interface IRippleToFIQuoteState : IGrainState
    {
        long Id { get; set; }
        RippleTransactionStatus Status { get; set; }
        string InvoiceId { get; set; }
        string TxId { get; set; }
        TransferToFITargetInfo TransferTargetInfo { get; set; }
        decimal Amount { get; set; }
        decimal SendAmount { get; set; }
        string Memo { get; set; }
        DateTime CreateAt { get; set; }
        DateTime? ReceiveAt { get; set; }
    }
}
