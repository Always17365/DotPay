 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Tools.Interfaces
{
    public interface IRippleRpcClient : Orleans.IGrainWithIntegerKey
    {
        Task<long> GetLastLedgerIndex();
        Task<RippleTransactionValidateResult> ValidateRippleTx(string txid);
    }
}
