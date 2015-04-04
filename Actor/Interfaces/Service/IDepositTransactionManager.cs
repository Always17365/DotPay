 ﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 ﻿using Dotpay.Actor;
 ﻿using Dotpay.Common;
 ﻿using Dotpay.Common.Enum;
 ﻿using Orleans;
 ﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Service
{
    public interface IDepositTransactionManager : Orleans.IGrainWithIntegerKey
    {
        //来自Ripple的直冲
        Task ProccessRippleDeposit(Guid depositTxId, Guid accountId, CurrencyType currency, string rippleTxId, decimal amount, Payway payway, string memo);
        Task CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task<ErrorCode> ConfirmDepositTransaction(Guid depositTxId, Guid managerId, string transactionNo);
        Task<ErrorCode> DepositTransactionMarkAsFail(Guid depositTxId, Guid managerId, string reason);
        Task Receive(MqMessage message);
    }


    [Immutable]
    [Serializable]
    public class ConfirmDepositTransactionMessage : MqMessage
    {
        public ConfirmDepositTransactionMessage(Guid depositTxId, Guid managerId, string transactionNo)
        {
            this.DepositTxId = depositTxId;
            this.ManagerId = managerId;
            this.TransactionNo = transactionNo;
        }

        public Guid DepositTxId { get; private set; }
        public Guid ManagerId { get; private set; }
        public string TransactionNo { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class RippleDepositTransactionMessage : MqMessage
    {
        public RippleDepositTransactionMessage(Guid depositTxId, Guid accountId, string rippleTxId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            this.DepositTxId = depositTxId;
            this.AccountId = accountId;
            this.RippleTxId = rippleTxId;
            this.Currency = currency;
            this.Amount = amount;
            this.Payway = payway;
            this.Memo = memo;
        }

        public Guid DepositTxId { get; private set; }
        public Guid AccountId { get; private set; }
        public string RippleTxId { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Amount { get; private set; }
        public Payway Payway { get; private set; }
        public string Memo { get; private set; }
    }
}
