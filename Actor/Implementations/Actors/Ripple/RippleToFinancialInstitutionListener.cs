 
﻿using System;
using System.Collections.Generic;
﻿using System.ComponentModel;
﻿using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using System.Threading;
﻿using Dotpay.Actor.Interfaces.Ripple;
﻿using Newtonsoft.Json;
﻿using Orleans;
﻿using RabbitMQ.Client;
using DFramework;

namespace Dotpay.Actors.Implementations
{
    public class RippleToFinancialInstitutionListener : Grain, IRippleToFinancialInstitutionListener, IRemindable
    {
        private const string MqExchangeName = "__RippleToFinancialInstitutionExchange";
        private const string MqQueueName = "__RippleToFinancialInstitutionQueue";
        private bool _started;
        internal static TaskScheduler OrleansScheduler;

        //private static Thread _consumerThread;
        Task IRippleToFinancialInstitutionListener.Start()
        {
            if (!this._started)
            {
                var factory = IoC.Resolve<IConnectionFactory>();
                var connection = factory.CreateConnection();

                var channel = connection.CreateModel();

                channel.ExchangeDeclare(MqExchangeName, ExchangeType.Direct, true, false, null);
                channel.QueueDeclare(MqQueueName, true, false, false, null);
                channel.QueueBind(MqQueueName, MqExchangeName, string.Empty);
                var consumer = new RippleTxMessageConsumer(channel);

                channel.BasicQos(0, 1, false);
                channel.BasicConsume(MqQueueName, false, consumer);

                this.RegisterOrUpdateReminder("keepAlive", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(65));
                this._started = true;
            }
            return TaskDone.Done;
        }

        async Task IRippleToFinancialInstitutionListener.CompleteRippleToFinancialInstitution(string invoiceId, int destinaionTag, string txId, decimal amount)
        {
            var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(destinaionTag);
            await rippleToFinancialInstitution.Complete(invoiceId, txId, amount);
        }

        Task IRemindable.ReceiveReminder(string reminderName, Orleans.Runtime.TickStatus status)
        {
            //this.GetLogger().Info(DateTime.Now.ToShortTimeString() + "-->I'm alive.");
            OrleansScheduler = TaskScheduler.Current;
            return TaskDone.Done;
        }

        public override Task OnActivateAsync()
        {
            OrleansScheduler = TaskScheduler.Current;
            return base.OnActivateAsync();
        }
    }


    public class RippleTxMessageConsumer : DefaultBasicConsumer
    {
        //private Action<RippleTxMessage> action;
        //private Action<Exception> errorCallback;
        public RippleTxMessageConsumer(IModel model)
            : base(model) { }
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var messageBody = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<RippleTxMessage>(messageBody);
            try
            {
                var rippleToFinancialInstitutionListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionListener>(0);

                var success = Task.Factory.StartNew(() =>
                 {
                     rippleToFinancialInstitutionListener.CompleteRippleToFinancialInstitution(message.InvoiceId,
                         message.DestinationTag, message.TxId, message.Amount);
                 },
                 CancellationToken.None, TaskCreationOptions.None,
                 scheduler: RippleToFinancialInstitutionListener.OrleansScheduler).Wait(TimeSpan.FromSeconds(5));

                if (success)
                {
                    Model.BasicAck(deliveryTag, false);
                }
                else
                {
                    Model.BasicNack(deliveryTag, false, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error("process ripple tx message error", ex);
                Model.BasicNack(deliveryTag, false, true);
            }
        }
    }

    public class RippleTxMessage
    {
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public int DestinationTag { get; set; }
        public decimal Amount { get; set; }
    }

}
