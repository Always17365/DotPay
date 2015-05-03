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

        #region 冗余
        public Guid UserId { get; set; }
        public string UserLoginName { get; set; }
        public string Email { get; set; } 
        #endregion
        public Guid AccountId { get; set; }
    }
}