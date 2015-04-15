using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
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
        public TransferFromDotpayInfo(Guid accountId)
        {
            this.AccountId = accountId;
        }

        public long UserId { get; set; }
        public string UserLoginName { get; set; }
        public Guid AccountId { get; set; }
    }
}