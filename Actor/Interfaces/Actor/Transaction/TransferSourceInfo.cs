using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces
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
        public Guid AccountId { get; set; }
    }
}