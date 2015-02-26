/*
Project Orleans Cloud Service SDK ver. 1.0
 
Copyright (c) Microsoft Corporation
 
All rights reserved.
 
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

﻿using System;
﻿using System.Collections.Concurrent;
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using DFramework;
﻿using Dotpay.Actor.Interfaces;
﻿using Orleans;
﻿using Orleans.Concurrency;
﻿using RabbitMQ.Client;
﻿using RabbitMQ.Client.Content;

namespace Dotpay.Actors.Implementations
{
    [StatelessWorker]
    public class MessageQueueProducter : Orleans.Grain, IMessageQueueProducter
    {
        private static ConcurrentDictionary<string, bool> _initializeMap = new ConcurrentDictionary<string, bool>();
        private IModel _channel;
        Task IMessageQueueProducter.RegisterAndBindQueue(string exchange, string exchangeType, string queue, string routeKey = "", bool durable = false)
        {
            if (!_initializeMap.ContainsKey(queue))
            {
                var factory = IoC.Resolve<IConnectionFactory>();
                var connection = factory.CreateConnection();

                this._channel = connection.CreateModel();
                this._channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
                this._channel.QueueDeclare(queue, true, false, false, null);
                this._channel.QueueBind(queue, exchange, routeKey);

                _initializeMap.AddOrUpdate(queue, true, (key, oldValue) => true);
            }

            return TaskDone.Done;
        }

        public Task PublishMessage(MqMessage message, string exchangeName, string routeKey = "", bool durable = false) 
        {
            var messageBody = IoC.Resolve<IJsonSerializer>().Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(messageBody);
            var build = new BytesMessageBuilder(this._channel);
            build.WriteBytes(bytes);
            ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

            this._channel.BasicPublish(exchangeName, routeKey, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
            return TaskDone.Done;
        }


        public override Task OnActivateAsync()
        {
            base.DelayDeactivation(TimeSpan.FromDays(365 * 10));
            return base.OnActivateAsync();
        }
    }
}
