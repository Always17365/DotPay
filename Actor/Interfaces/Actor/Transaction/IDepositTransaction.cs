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
        Task Initiliaze(string sequenceNo, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task ConfirmDepositPreparation();
        Task ConfirmDeposit(Guid operatorId, string transsactionNo);
        Task Fail(Guid operatorId, string reason);
        Task<DepositTransactionInfo> GetTransactionInfo();
        Task<DepositStatus> GetStatus();
    }

    [Immutable]
    [Serializable]
    public class DepositTransactionInfo
    {
        public DepositTransactionInfo(Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            this.AccountId = accountId;
            this.Currency = currency;
            this.Amount = amount;
            this.Payway = payway;
            this.Memo = memo;
        }

        public Guid AccountId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public Payway Payway { get; private set; }
        public string Memo { get; private set; }
    }
}
