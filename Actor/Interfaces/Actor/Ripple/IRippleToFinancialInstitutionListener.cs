 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces.Ripple
{
    public interface IRippleToFinancialInstitutionListener : Orleans.IGrainWithIntegerKey
    {
        Task Receive(MqMessage message);
    }

    [Immutable]
    [Serializable]
    public class RippleTxMessage : MqMessage
    {
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public int DestinationTag { get; set; }
        public decimal Amount { get; set; }
    }
}
