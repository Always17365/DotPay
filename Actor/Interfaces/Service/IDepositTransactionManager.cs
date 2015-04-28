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
        Task<Tuple<ErrorCode, string>> CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task<ErrorCode> ConfirmDepositTransaction(Guid depositTxId, Guid? managerId, string transactionNo, decimal amount);
        Task<ErrorCode> DepositTransactionMarkAsFail(Guid depositTxId, Guid managerId, string reason);
        Task Receive(MqMessage message);
    }


    [Immutable]
    [Serializable]
    public class ConfirmDepositTransactionMessage : MqMessage
    {
        public ConfirmDepositTransactionMessage(Guid depositTxId, Guid? managerId, string transactionNo, decimal amount)
        {
            this.DepositTxId = depositTxId;
            this.ManagerId = managerId;
            this.TransactionNo = transactionNo;
            this.Amount = amount;
        }

        public Guid DepositTxId { get; private set; }
        public Guid? ManagerId { get; private set; }
        public string TransactionNo { get; private set; }
        public decimal Amount { get; private set; }
    }

    [Immutable]
    [Serializable]
    public class RippleDepositTransactionMessage : MqMessage
    {
        public RippleDepositTransactionMessage(Guid depositTxId, Guid accountId, string rippleTxId, string invoiceId, long rippleToDotpayQuoteId, decimal sendAmount)
        {
            this.DepositTxId = depositTxId;
            this.AccountId = accountId;
            this.RippleTxId = rippleTxId;
            this.InvoiceId = invoiceId;
            this.RippleToDotpayQuoteId = rippleToDotpayQuoteId;
            this.SendAmount = sendAmount;
        }

        public Guid DepositTxId { get; private set; }
        public Guid AccountId { get; private set; }
        public string RippleTxId { get; private set; }
        public string InvoiceId { get; private set; }
        public long RippleToDotpayQuoteId { get; private set; }
        public decimal SendAmount { get; private set; }
    }
}
