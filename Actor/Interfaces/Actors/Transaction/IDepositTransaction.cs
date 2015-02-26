using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces
{
    /// <summary>
    /// 充值
    /// <remarks>暂时只考虑人民币的充值,currency实际留作以后扩展</remarks>
    /// </summary>
    public interface IDepositTransaction : Orleans.IGrainWithGuidKey
    {
        Task Initiliaze(Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo); 
        Task ConfirmDepositPreparation();
        Task ConfirmDeposit(Guid operatorId, string transsactionNo);
        Task Fail(Guid operatorId, string reason);
        Task<Immutable<DepositTransactionInfo>> GetTransactionInfo();
        Task<Immutable<DepositStatus>> GetStatus();
    }

    [Immutable]
    [Serializable]
    public class DepositTransactionInfo
    {
        public DepositTransactionInfo(Guid accountId, CurrencyType currency, decimal amount, Payway payway, DepositStatus state)
        {
            this.AccountId = accountId;
            this.Currency = currency;
            this.Amount = amount;
            this.Payway = payway;
            this.State = state;
        }

        public Guid UserId { get { return this.AccountId; } }
        public Guid AccountId { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public Payway Payway { get; set; }
        public DepositStatus State { get; set; }
    }
}
