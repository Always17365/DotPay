using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    [Immutable]
    [Serializable]
    public abstract class TransferTargetInfo
    {
        public Payway Payway { get; set; }
    }

    /// <summary>
    /// transfer to financial institution(alipay tenpay bank)
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToRippleTargetInfo : TransferTargetInfo
    {
        public string Destination { get; set; }
    }

    /// <summary>
    /// inner transfer target
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToDotpayTargetInfo : TransferTargetInfo
    {
        public string RealName { get; set; }
        public Guid AccountId { get; set; }
    }

    /// <summary>
    /// transfer to financial institution(alipay tenpay bank)
    /// </summary>
    [Immutable]
    [Serializable]
    public abstract class TransferToFITargetInfo : TransferTargetInfo
    {
        public string RealName { get; set; }
        public string DestinationAccount { get; set; }
    }
    /// <summary>
    /// transfer to third party payment organization
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToTppTargetInfo : TransferToFITargetInfo
    {
    }

    /// <summary>
    /// transfer to bank
    /// </summary>
    [Immutable]
    [Serializable]
    public class TransferToBankTargetInfo : TransferToFITargetInfo
    {
        public Bank Bank { get; set; }
    }
}