using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Tools.Interfaces;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Tools.Implementations
{
    public class RippleRpcClient : Orleans.Grain, IRippleRpcClient
    {
        Task<long> IRippleRpcClient.GetLastLedgerIndex()
        {
            throw new NotImplementedException();
        }

        Task<RippleTransactionValidateResult> IRippleRpcClient.ValidateRippleTx(string txid)
        {
            throw new NotImplementedException();
        }
    }
}
