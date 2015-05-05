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
        public string Id { get; set; }
        public string Type { get; set; }
        public string SequenceNo { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public Bank? Bank { get; set; } 
        public Payway Payway { get; set; } 
        public string Destination { get; set; }
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
    public class TransferTransactionSubmitViewModel
    {
        public Guid TransferUserId { get; set; }
        public Guid TransferTransactionId { get; set; }
        public Guid DestinationAccountId { get; set; }
        public string Destination { get; set; }
        public string RealName { get; set; }
        public Bank? Bank { get; set; }
        public Payway Payway { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
    }

    [Serializable]
    public class TransferTransactionDetailViewModel
    {
        public Guid TransferUserId { get; set; }
        public string SequenceNo { get; set; }
        public Guid TransferTransactionId { get; set; }
        public Guid DestinationAccountId { get; set; }
        public string Destination { get; set; }
        public string RealName { get; set; }
        public Bank? Bank { get; set; }
        public Payway Payway { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; } 
    }
    [Serializable]
    public class TransferTransactionListViewModel
    {
        public string SequenceNo { get; set; }
        public string Destination { get; set; }
        public string Bank { get; set; }
        public string Payway { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }
    }

    [Serializable]
    public class TransferTransactionInfo
    {
        public TransferTransactionInfo(TransferSourceInfo source, TransferTargetInfo target, CurrencyType currency, decimal amount, string memo)
        {
            this.Source = source;
            this.Target = target;
            this.Currency = currency;
            this.Amount = amount;
            this.Memo = memo;
        }

        public TransferSourceInfo Source { get; set; }
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
        public string TxId { get; set; }
        #region 冗余
        public Guid UserId { get; set; }
        public string UserLoginName { get; set; }
        public string Email { get; set; }
        #endregion
        public Guid AccountId { get; set; }
    }
    [Serializable]
    public class TransferTargetInfo
    {
        /// <summary>
        /// 用于标记转账的目标方式
        /// </summary>
        public Payway Payway { get; set; }
        public Bank? Bank { get; set; }
        public string Destination { get; set; }
        public string RealName { get; set; }
        public Guid AccountId { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public string UserLoginName { get; set; }
    }
}
