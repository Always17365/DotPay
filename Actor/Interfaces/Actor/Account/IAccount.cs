using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor
{
    public interface IAccount : Orleans.IGrainWithGuidKey
    {
        Task<ErrorCode> Initialize(long ownerId);
        Task<bool> Validate();
        Task<ErrorCode> AddTransactionPreparation(Guid transactionId, TransactionType transactionType, PreparationType preparationType, CurrencyType currency, decimal amount);
        Task CommitTransactionPreparation(Guid transactionId);
        Task CancelTransactionPreparation(Guid transactionId);
        Task<decimal> GetBalance(CurrencyType currency);
        Task<Dictionary<CurrencyType, decimal>> GetBalances();
        Task<long> GetOwnerId();
    }
}
