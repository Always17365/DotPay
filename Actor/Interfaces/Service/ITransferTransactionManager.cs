using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface ITransferTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task<ErrorCode> SubmitTransferTransaction(TransferTransactionInfo transactionInfo);
        Task<ErrorCode> MarkAsProcessing(Guid transferTransactionId, Guid managerId);
        Task<ErrorCode> ConfirmTransactionFail(Guid transferTransactionId, Guid managerId, string reason);
        Task Receive(MqMessage message); 
    }



    #region Mq Message

    [Immutable]
    [Serializable]
    public class TransferTransactionMessage : MqMessage
    {
        public TransferTransactionMessage(string typeName)
        {
            this.Type = typeName;
        }

        public string Type { get; private set; }
    }

    #region SubmitTransferTransactionMessage

    [Immutable]
    [Serializable]
    public class SubmitTransferTransactionMessage : TransferTransactionMessage
    {
        public SubmitTransferTransactionMessage(Guid transferTransactionId, TransferFromDotpayInfo source,
            TransferTargetInfo target, CurrencyType currency, decimal amount, string memo)
            : base(typeof(SubmitTransferTransactionMessage).Name)
        {
            this.Source = source;
            this.Target = target;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.TransferTransactionId = transferTransactionId;
        }

        public Guid TransferTransactionId { get; private set; }
        public TransferFromDotpayInfo Source { get; private set; }
        public TransferTargetInfo Target { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string Memo { get; private set; }
    }

    #endregion

    #region SubmitTransferTransactionToRippleMessage

    [Immutable]
    [Serializable]
    public class SubmitTransferTransactionToRippleMessage : MqMessage
    {
        public SubmitTransferTransactionToRippleMessage(Guid transferTransactionId,
            TransferToRippleTargetInfo target, CurrencyType currency, decimal amount)
        {
            this.TransferTransactionId = transferTransactionId;
            this.Target = target;
            this.Currency = currency;
            this.Amount = amount;
        }

        public Guid TransferTransactionId { get; private set; }
        public TransferToRippleTargetInfo Target { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
    }

    #endregion

    #region RippleTransactionPresubmitMessage

    [Immutable]
    [Serializable]
    public class RippleTransactionPresubmitMessage : TransferTransactionMessage
    {
        public RippleTransactionPresubmitMessage(Guid transferTransactionId, string rippleTxId,
            CurrencyType currency, decimal amount, long lastLedgerIndex)
            : base(typeof(RippleTransactionPresubmitMessage).Name)
        {
            this.TransferTransactionId = transferTransactionId;
            this.RippleTxId = rippleTxId;
            this.Currency = currency;
            this.Amount = amount;
            this.LastLedgerIndex = lastLedgerIndex;
        }

        public Guid TransferTransactionId { get; private set; }
        public string RippleTxId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public long LastLedgerIndex { get; private set; }
    }

    #endregion

    #region RippleTransactionResultMessage

    [Immutable]
    [Serializable]
    public class RippleTransactionResultMessage : TransferTransactionMessage
    {
        public RippleTransactionResultMessage(Guid transferTransactionId, string rippleTxId, bool success,
            RippleTransactionFailedType failedReason)
            : base(typeof(RippleTransactionResultMessage).Name)
        {
            this.TransferTransactionId = transferTransactionId;
            this.RippleTxId = rippleTxId;
            this.Success = success;
            this.FailedReason = failedReason;
        }

        public Guid TransferTransactionId { get; private set; }
        public string RippleTxId { get; private set; }
        public bool Success { get; private set; }
        public RippleTransactionFailedType FailedReason { get; private set; }
    }

    #endregion
    #endregion
}
