 
﻿using System;
﻿using System.Collections.Concurrent;
﻿using System.Diagnostics;
﻿using System.Text;
﻿using System.Threading;
﻿using System.Threading.Tasks;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Interfaces.Ripple;
﻿using Dotpay.Actor.Ripple.Interfaces;
﻿using Dotpay.Actor.Implementations;
﻿using Newtonsoft.Json;
﻿using Orleans;
﻿using Orleans.Concurrency;
﻿using Orleans.Runtime;
﻿using RabbitMQ.Client;
using DFramework;
using Dotpay.Common;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleToFinancialInstitutionListener : Grain, IRippleToFinancialInstitutionListener
    { 
        private const string MqExchangeName = Constants.RippleToFinancialInstitutionMQName + Constants.ExechangeSuffix;
        private const string MqQueueName = Constants.RippleToFinancialInstitutionMQName + Constants.QueueSuffix;

        public async Task Receive(MqMessage message)
        {
            var rippleTxMessage = message as RippleTxMessage;
            if (rippleTxMessage != null)
            {
                var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(rippleTxMessage.DestinationTag);
                await rippleToFinancialInstitution.Complete(rippleTxMessage.InvoiceId, rippleTxMessage.TxId, rippleTxMessage.Amount);
            }
        }
        #region Override

        public async override Task OnActivateAsync()
        {
            await MessageProducterManager.RegisterAndBindQueue(MqExchangeName, ExchangeType.Direct, MqQueueName, durable: true);

            await base.OnActivateAsync();
        }
        #endregion
    } 

}
