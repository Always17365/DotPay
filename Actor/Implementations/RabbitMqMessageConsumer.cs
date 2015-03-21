using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime;
using RabbitMQ.Client;

namespace Dotpay.Actor.Implementations
{
    internal abstract class RabbitMqMessageConsumer : DefaultBasicConsumer
    {
        protected Guid grainId; 

        public RabbitMqMessageConsumer(IModel model, Guid grainId)
            : base(model)
        {
            this.grainId = grainId;
        }
    }
}
