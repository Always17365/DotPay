 ﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 ﻿using Dotpay.Common.Enum;
 ﻿using Orleans;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IDepositTransactionManager : Orleans.IGrain
    {
        Task CreateDepositTransaction(Guid depositTxId, Guid userId, CurrencyType currency, decimal amount, Payway payway, string memo);
        Task ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo);
        Task DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason);
    }
}
