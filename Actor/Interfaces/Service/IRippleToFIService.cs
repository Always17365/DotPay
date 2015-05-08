 
﻿using System;
﻿using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Ripple
{
    public interface IRippleToFiService : IGrainWithIntegerKey
    {
        Task<ErrorCode> MarkAsProcessing(long rippleToFiTransactionId, Guid managerId);
        Task<ErrorCode> ConfirmTransactionComplete(long rippleToFiTransactionId, string transferNo, Guid managerId, string managerMemo);
        Task<ErrorCode> ConfirmTransactionFail(long rippleToFiTransactionId, string reason, Guid managerId);
        Task Receive(MqMessage message);
    }

    [Immutable]
    [Serializable]
    public class RippleTxMessage : MqMessage
    {
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public int DestinationTag { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
    }

    [Immutable]
    [Serializable]
    public class RippleToFiTxMessage : MqMessage
    {
        public long RippleToFiTxId { get; set; }
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
