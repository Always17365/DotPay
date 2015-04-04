using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Actor.Implementations
{
    internal class MessageProducterManager
    {
        static Random producterIdGenerator = new Random();
        public static IMessageQueueProducter GetProducter()
        {
            var randomId = producterIdGenerator.Next(0, 9);
            return GrainFactory.GetGrain<IMessageQueueProducter>(randomId);
        }

        public static Task RegisterAndBindQueue(string exchange, string exchangeType, string queue, string routeKey = "", bool durable = false)
        {
            var randomId = producterIdGenerator.Next(0, 9);
            var grain = GrainFactory.GetGrain<IMessageQueueProducter>(randomId);

            return grain.RegisterAndBindQueue(exchange, exchangeType, queue, routeKey, durable);
        }
    }
}
