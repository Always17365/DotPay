 
﻿using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Service;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Tools
{
    public interface IRippleRpcClient : IGrainWithIntegerKey
    {
        Task<long> GetLastLedgerIndex();
        Task<RippleTransactionValidateResult> ValidateRippleTx(string txid);
    }

    #region GetLastLedgerIndexRequestMessage

    [Immutable]
    [Serializable]
    public class GetLastLedgerIndexRequestMessage : MqMessage
    {
        public RippleValidateRequestType Type { get; set; }

        public GetLastLedgerIndexRequestMessage(RippleValidateRequestType type)
        {
            this.Type = type;
        } 
    }

    #endregion

    #region ValidateTxRequestMessage

    [Immutable]
    [Serializable]
    public class ValidateTxRequestMessage : MqMessage
    {
        public RippleValidateRequestType Type { get;private set; }
        public string RippleTxId { get; private set; }

        public ValidateTxRequestMessage(RippleValidateRequestType type,string rippleTxId)
        {
            this.Type = type;
            this.RippleTxId = rippleTxId;
        }
    }

    #endregion
}
