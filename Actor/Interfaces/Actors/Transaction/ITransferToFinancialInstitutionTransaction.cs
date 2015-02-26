using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Dotpay.Actor.Interfaces
{
    /// <summary>
    /// transfer transaction to financial institution
    /// </summary>
    public interface ITransferToFinancialInstitutionTransaction : IGrainWithGuidKey
    {
        Task Initialize(Immutable<TransferSourceInfo> transferSourceInfo, Immutable<TransferTargetInfo> transferTargetInfo, decimal amount, string memo);
        Task ConfirmTransactionPreparation();
        Task<ErrorCode> MarkAsProcessing(Guid operatorId);
        Task<ErrorCode> MarkAsCompleted(Guid operatorId, string transferTxNo);
        Task<ErrorCode> MarkAsFailed(Guid operatorId, string reason);
    }

    [Immutable]
    [Serializable]
    public class TransferSourceInfo
    {
        public Payway Payway { get; set; }
    }

    [Immutable]
    [Serializable]
    public class TransferFromRippleInfo : TransferSourceInfo
    {
        public string TxId { get; set; }
    }

    [Immutable]
    [Serializable]
    public class TransferFromDotpayInfo : TransferSourceInfo
    {
        public Guid AccountId { get; set; }
    }

    [Immutable]
    [Serializable]
    public class TransferTargetInfo
    {
        public string RealName { get; set; }
        public Payway Payway { get; set; }
        public string DestinationAccount { get; set; }
    }

    /// <summary>
    /// transfer to third party payment organization
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToTppInfo : TransferTargetInfo
    {
    }

    /// <summary>
    /// transfer to bank
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToBankInfo : TransferTargetInfo
    {
        public Bank Bank { get; set; }
    }
}
