 
﻿using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Tools
{
    public interface IRippleRpcClient : IGrainWithIntegerKey
    {
        Task<long> GetLastLedgerIndex();
        Task<RippleTransactionValidateResult> ValidateRippleTx(string txid);
    }
}
