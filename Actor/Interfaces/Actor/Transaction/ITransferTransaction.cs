using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Dotpay.Actor
{
    /// <summary>
    /// transfer transaction to financial institution or to ripple
    /// </summary>
    public interface ITransferTransaction : IGrainWithGuidKey
    {
        Task Initialize(string sequenceNo, TransferTransactionInfo transferTransactionInfo);
        Task ConfirmTransactionPreparation();
        Task Cancel(TransferTransactionCancelReason cancelReason);

        #region 手工处理接口
        Task<ErrorCode> MarkAsProcessing(Guid managerId);
        Task<ErrorCode> ConfirmComplete(Guid managerId, string transferTxNo);
        Task<ErrorCode> ConfirmFail(Guid managerId, string reason);
        #endregion

        #region 自动处理接口 for ripple
        Task SubmitToRipple();
        /// <summary>
        /// 在Ripple提交之后，特定原因下，重新提交该笔交易到Ripple.
        /// <remarks>由于网络等原因，未返回结果，需要提交Rpc请求，判断当前的处理结果，检查如果未成功处理的情况,重复提交。原因只能是一个，即TxnNotFound</remarks> 
        /// </summary>
        /// <returns></returns>
        Task ReSubmitToRipple();
        Task RippleTransactionPersubmit(string rippleTxId, long lastLedgerIndex);
        Task RippleTransactionComplete(string rippleTxId);
        Task RippleTransactionFail(string rippleTxId, RippleTransactionFailedType railedReason);
        #endregion

        Task<TransferTransactionStatus> GetStatus();
        Task<RippleTransactionStatus> GetRippleTransactionStatus();
        Task<TransferTransactionInfo> GetTransactionInfo();
        Task<RippleTransactionInfo> GetRippleTransactionInfo();
    }

    [Immutable]
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

    [Immutable]
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
}
