using System;
using System.Threading.Tasks;
using Dotpay.Common.Enum;
using Orleans;

namespace Dotpay.Actor.Tools.Implementations
{
    public class RippleRpcClient : Grain, IRippleRpcClient
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
