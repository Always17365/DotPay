using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces
{
    public interface IAccount : Orleans.IGrainWithGuidKey
    {
        Task<ErrorCode> Initialize(Guid ownerId);
        Task<bool> Validate();
        Task<ErrorCode> AddTransactionPreparation(Guid transactionId, TransactionType transactionType, PreparationType preparationType, CurrencyType currency, decimal amount);
        Task CommitTransactionPreparation(Guid transactionId);
        Task CancelTransactionPreparation(Guid transactionId);
        Task<decimal> GetBalance(CurrencyType currency);
        Task<Immutable<Dictionary<CurrencyType, decimal>>> GetBalances();
    }
}
