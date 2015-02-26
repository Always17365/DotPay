using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Interfaces.Tools;
using Dotpay.Actors.Implementations.Events.Transaction;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using Orleans.EventSourcing;
﻿using Orleans.Providers;

namespace Dotpay.Actors.Implementations.Actors.Transaction
{
    /// <summary>
    /// Orleans grain implementation class TransferToFinancialInstitutionTransaction
    /// </summary>
    [StorageProvider(ProviderName = "CouchbaseStore")]
    public class TransferToFinancialInstitutionTransaction : EventSourcingGrain<TransferToFinancialInstitutionTransaction, ITransferToFinancialInstitutionTransactionState>, ITransferToFinancialInstitutionTransaction
    {
        #region ITransferToFinancialInstitutionTransaction
        async Task ITransferToFinancialInstitutionTransaction.Initialize(Immutable<TransferSourceInfo> transferSourceInfo, Immutable<TransferTargetInfo> transferTargetInfo, decimal amount,
              string memo)
        {
            var sequenceNoGenerator = GrainFactory.GetGrain<ISequenceNoGenerator>((int)SequenceNoType.TransferToFinancialInstitutionNo);
            var sequenceNo = await sequenceNoGenerator.GetNext();

            if (this.State.Status < TransferTransactionStatus.Submited)
                await this.ApplyEvent(new TransferToFinancialInstitutionTransactionInitilized(
                      transferSourceInfo.Value, transferTargetInfo.Value, sequenceNo.Value, amount, memo));
        }

        async Task ITransferToFinancialInstitutionTransaction.ConfirmTransactionPreparation()
        {
            if (this.State.Status == TransferTransactionStatus.Submited)
            {
                await
                    this.ApplyEvent(new TransferToFinancialInstitutionTransactionPreparationCompleted(
                        this.State.Source, this.State.Target, this.State.Amount));
            }
        }

        async Task<ErrorCode> ITransferToFinancialInstitutionTransaction.MarkAsProcessing(Guid operatorId)
        {
            if (this.State.Status == TransferTransactionStatus.PreparationCompleted)
            {
                await this.ApplyEvent(new TransferToFinancialInstitutionMarkProcessing(operatorId));
                return ErrorCode.None;
            }
            else
                return ErrorCode.TranasferToFiIsLockedByOther;
        }

        async Task<ErrorCode> ITransferToFinancialInstitutionTransaction.MarkAsCompleted(Guid operatorId, string transferTxNo)
        {
            if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.OperatorId == operatorId)
            {
                await this.ApplyEvent(new TransferToFinancialInstitutionMarkCompleted(operatorId, transferTxNo));
                return ErrorCode.None;
            }
            else if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.OperatorId != operatorId)
                return ErrorCode.TranasferToFiIsLockedByOther;
            else
                return ErrorCode.None;
        }

        async Task<ErrorCode> ITransferToFinancialInstitutionTransaction.MarkAsFailed(Guid operatorId, string reason)
        {
            if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.OperatorId == operatorId)
            {
                await this.ApplyEvent(new TransferToFinancialInstitutionMarkCompleted(operatorId, reason));
                return ErrorCode.None;
            }
            else if (this.State.Status == TransferTransactionStatus.LockeByProcessor && this.State.OperatorId != operatorId)
                return ErrorCode.TranasferToFiIsLockedByOther;
            else
                return ErrorCode.None;
        }
        #endregion
    }

    public interface ITransferToFinancialInstitutionTransactionState : IEventSourcingState
    {
        TransferSourceInfo Source { get; set; }
        TransferTargetInfo Target { get; set; }
        TransferTransactionStatus Status { get; set; }
        string SequenceNo { get; set; }
        decimal Amount { get; set; }
        string FITransactionNo { get; set; }
        Guid OperatorId { get; set; }
        DateTime CreateAt { get; set; }
        DateTime? LockAt { get; set; }
        DateTime? CompleteAt { get; set; }
        DateTime? FailAt { get; set; }
        string Reason { get; set; }
    }
}
