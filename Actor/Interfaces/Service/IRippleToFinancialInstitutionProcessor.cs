 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces.Ripple
{
    public interface IRippleToFinancialInstitutionProcessor : Orleans.IGrainWithIntegerKey
    {
        Task Receive(MqMessage message);
    }

    [Immutable]
    [Serializable]
    public class RippleToFITxMessage : MqMessage
    {
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public int DestinationTag { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
