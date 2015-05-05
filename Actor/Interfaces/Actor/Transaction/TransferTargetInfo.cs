using System;
using Dotpay.Common.Enum;
using Orleans.Concurrency;

namespace Dotpay.Actor
{
    [Immutable]
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