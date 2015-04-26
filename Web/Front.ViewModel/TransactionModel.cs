using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Newtonsoft.Json;

namespace Dotpay.Front.ViewModel
{

    [Serializable]
    public class IndexTransactionListViewModel
    {
        public string Type { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Payway { get; set; }
        public string Memo { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? CompleteAt { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class DepositTransactionListViewModel
    {
        public string SequenceNo { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public DepositStatus Status { get; set; }
        public Payway Payway { get; set; }
        public string Memo { get; set; }
        public DateTime CreateAt { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class TransferTransactionListViewModel
    {
        public string SequenceNo { get; set; }
        public TransferTransactionInfo TransactionInfo { get; set; }
        public TransferTransactionStatus Status { get; set; }
        public RippleTransactionStatus? RippleTxStatus { get; set; }
        public string FiTransactionNo { get; set; }
        public RippleTransactionInfo RippleTransactionInfo { get; set; }
        public Guid? ManagerId { get; set; }
        public string Manager { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? LockAt { get; set; }
        public DateTime? CompleteAt { get; set; }
        public DateTime? FailAt { get; set; }
        public string Reason { get; set; }
    }

    [Serializable]
    public class TransferTransactionInfo
    {
        public TransferTransactionInfo(TransferFromDotpayInfo source, TransferTargetInfo target, CurrencyType currency, decimal amount, string memo)
        {
            this.Source = source;
            this.Target = target;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
        }

        public TransferFromDotpayInfo Source { get; set; }
        public TransferTargetInfo Target { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
    }

    [Serializable]
    public class RippleTransactionInfo
    {
        public RippleTransactionInfo(string rippleTxId, long lastLedgerIndex, int retryCount)
        {
            this.RippleTxId = rippleTxId;
            this.LastLedgerIndex = lastLedgerIndex;
            this.RetryCount = retryCount;
        }

        public string RippleTxId { get; set; }
        public long LastLedgerIndex { get; set; }
        public RippleTransactionFailedType FailReason { get; set; }
        public int RetryCount { get; set; }
    }
    [Serializable]
    public class TransferSourceInfo
    {
        public Payway Payway { get; set; }
    }

    [Serializable]
    public class TransferFromRippleInfo : TransferSourceInfo
    {
        public string TxId { get; set; }
    }

    [Serializable]
    public class TransferFromDotpayInfo : TransferSourceInfo
    {
        public TransferFromDotpayInfo(Guid accountId)
        {
            this.AccountId = accountId;
        }

        public Guid UserId { get; set; }
        public string UserLoginName { get; set; }
        public Guid AccountId { get; set; }
    }
    [Serializable]
    public abstract class TransferTargetInfo
    {
        public Payway Payway { get; set; }
    }

    [Serializable]
    public class TransferToRippleTargetInfo : TransferTargetInfo
    {
        public string Destination { get; set; }
    }
    [Serializable]
    public class TransferToDotpayTargetInfo : TransferTargetInfo
    {
        public string RealName { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public string UserLoginName { get; set; }
    }
    [Serializable]
    public abstract class TransferToFITargetInfo : TransferTargetInfo
    {
        public string RealName { get; set; }
        public string DestinationAccount { get; set; }
    }
    [Serializable]
    public class TransferToTppTargetInfo : TransferToFITargetInfo
    {
    }

    [Serializable]
    public class TransferToBankTargetInfo : TransferToFITargetInfo
    {
        public Bank Bank { get; set; }
    }
}
