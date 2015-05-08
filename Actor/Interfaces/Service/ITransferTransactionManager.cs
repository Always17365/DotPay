using System;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Service
{
    public interface ITransferTransactionManager : IGrainWithIntegerKey
    {
        Task<ErrorCode> SubmitTransferToDotpayTransaction(Guid transferTransactionId, Guid sourceAccountId, Guid targetAccountId, string targetUserRealName, CurrencyType currency, decimal amount, string memo, string paymentPassword);
        Task<ErrorCode> SubmitTransferToTppTransaction(Guid transferTransactionId, Guid sourceAccountId, string targetAccount, string realName, Payway targetPayway, CurrencyType currency, decimal amount, string memo, string paymentPassword);
        Task<ErrorCode> SubmitTransferToBankTransaction(Guid transferTransactionId, Guid sourceAccountId, string targetAccount, string realName, Bank targetBank, CurrencyType currency, decimal amount, string memo, string paymentPassword);
        Task<ErrorCode> SubmitTransferToRippleTransaction(Guid transferTransactionId, Guid sourceAccountId, string rippleAddress, CurrencyType currency, decimal amount, string memo, string paymentPassword);
        Task<ErrorCode> MarkAsProcessing(Guid transferTransactionId, Guid managerId);
        Task<ErrorCode> ConfirmTransactionFail(Guid transferTransactionId, Guid managerId, string reason);
        Task<ErrorCode> ConfirmTransactionComplete(Guid transferTransactionId, string transferNo,decimal amount, Guid managerId);
        Task Receive(MqMessage message);
    }



    #region Mq Message

    public enum TransferTransactionMessageType : uint
    {
        SubmitTransferTransactionMessage = 1,
        RippleTransactionPresubmitMessage=2,
        RippleTransactionResultMessage=3
    }

    [Immutable]
    [Serializable]
    public class TransferTransactionMessage : MqMessage
    {
        public uint Type { get; set; }
    }

    #region SubmitTransferTransactionMessage

    [Immutable]
    [Serializable]
    public class SubmitTransferTransactionMessage : TransferTransactionMessage
    {
        public SubmitTransferTransactionMessage(Guid transferTransactionId, Guid userId,
            TransferTargetInfo target, CurrencyType currency, decimal amount, string memo, TransferTransactionMessageType type)
        {
            this.Type = (uint)type;
            this.Target = target;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
            this.TransferTransactionId = transferTransactionId;
            this.UserId = userId;
        }

        public Guid TransferTransactionId { get; private set; }
        public Guid UserId { get; private set; }
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
            string destination, CurrencyType currency, decimal amount)
        {
            this.TransferTransactionId = transferTransactionId;
            this.Destination = destination;
            this.Currency = currency;
            this.Amount = amount;
        }

        public Guid TransferTransactionId { get; private set; }
        public string Destination { get; private set; }
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
            CurrencyType currency, decimal amount, long lastLedgerIndex, TransferTransactionMessageType type)
        {
            this.TransferTransactionId = transferTransactionId;
            this.RippleTxId = rippleTxId;
            this.Currency = currency;
            this.Amount = amount;
            this.LastLedgerIndex = lastLedgerIndex;
            this.Type = (uint)type;
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
            RippleTransactionFailedType failedReason, TransferTransactionMessageType type)
        {
            this.TransferTransactionId = transferTransactionId;
            this.RippleTxId = rippleTxId;
            this.Success = success;
            this.FailedReason = failedReason;
            this.Type = (uint)type;
        }

        public Guid TransferTransactionId { get; private set; }
        public string RippleTxId { get; private set; }
        public bool Success { get; private set; }
        public RippleTransactionFailedType FailedReason { get; private set; }
    }

    #endregion
    #endregion
}
