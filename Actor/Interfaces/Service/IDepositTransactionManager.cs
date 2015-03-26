 ﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 ﻿using Dotpay.Actor.Interfaces;
 ﻿using Dotpay.Common.Enum;
 ﻿using Orleans;
 ﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IDepositTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo);
        Task DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason);
        Task Receive(MqMessage message);
    }


    [Immutable]
    [Serializable]
    public class ConfirmDepositTransactionMessage : MqMessage
    {
        public ConfirmDepositTransactionMessage(Guid depositTxId, Guid operatorId, string transactionNo)
        {
            this.DepositTxId = depositTxId;
            this.OperatorId = operatorId;
            this.TransactionNo = transactionNo;
        }

        public Guid DepositTxId { get; private set; }
        public Guid OperatorId { get; private set; }
        public string TransactionNo { get; private set; }
    }
}
