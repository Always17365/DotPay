using System;
﻿using System.Collections.Concurrent;
﻿using System.Text;
﻿using System.Threading.Tasks;
﻿using DFramework;
﻿using Dotpay.Actor.Interfaces;
﻿using Orleans;
﻿using Orleans.Concurrency;
﻿using RabbitMQ.Client;
﻿using RabbitMQ.Client.Content;

namespace Dotpay.Actor.Implementations
{
    [StatelessWorker]
    public class MessageQueueProducter : Grain, IMessageQueueProducter
    {
        private static ConcurrentDictionary<string, bool> _initializeMap = new ConcurrentDictionary<string, bool>();
        private IModel _channel;
        Task IMessageQueueProducter.RegisterAndBindQueue(string exchange, string exchangeType, string queue, string routeKey, bool durable)
        {
            if (string.IsNullOrWhiteSpace(routeKey.NullSafe())) routeKey = ""; 

            if (!_initializeMap.ContainsKey(queue))
            {
                var connection = RabbitMqConnectionManager.GetConnection();

                this._channel = connection.CreateModel();
                this._channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable, false, null);
                this._channel.QueueDeclare(queue, durable, false, false, null);
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

            if (durable)
                ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

            this._channel.BasicPublish(exchangeName, routeKey, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
            return TaskDone.Done;
        }

        public override Task OnDeactivateAsync()
        {
            if (_channel.IsOpen) _channel.Close();
            else
            {
                try { _channel.Close(); }
                catch (Exception) { }
            }
            return base.OnDeactivateAsync(); 
        }
    }
}
