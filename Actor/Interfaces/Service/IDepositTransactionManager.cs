 ﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 ﻿using Dotpay.Actor.Interfaces;
 ﻿using Dotpay.Common.Enum;
 ﻿using Orleans;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IDepositTransactionManager : Orleans.IGrainWithIntegerKey
    {
        Task Start();
        Task CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo);
        Task DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason);
        Task Receive(MqMessage message);
    }
}
