using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
﻿using Orleans.Concurrency;

namespace Dotpay.Actor.Interfaces
{
    public interface IMessageQueueProducter : Orleans.IGrainWithIntegerKey
    {
        Task RegisterAndBindQueue(string exchange, string exchangeType, string queue, string routeKey = "", bool durable = false);
        Task PublishMessage(MqMessage message, string exchangeName, string routeKey = "", bool durable = false);
    }

    [Immutable]
    [Serializable]
    public class MqMessage { }
}
