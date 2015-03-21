 
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

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class RippleToFinancialInstitutionListener : MessageConsumerAbstractGrain, IRippleToFinancialInstitutionListener
    {
        private const string MqExchangeName = "__RippleToFinancialInstitutionExchange";
        private const string MqQueueName = "__RippleToFinancialInstitutionQueue";
        internal static readonly ConcurrentDictionary<Guid, TaskScheduler> OrleansSchedulerContainer = new ConcurrentDictionary<Guid, TaskScheduler>();

        #region Override
        protected override Task<bool> CheckSelfCanBeDeactive()
        {
            if (OrleansSchedulerContainer.Count == 1)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }

        public override async Task Receive(MqMessage message)
        {
            var rippleTxMessage = message as RippleTxMessage;
            if (rippleTxMessage != null)
            {
                var rippleToFinancialInstitution = GrainFactory.GetGrain<IRippleToFinancialInstitution>(rippleTxMessage.DestinationTag);
                await rippleToFinancialInstitution.Complete(rippleTxMessage.InvoiceId, rippleTxMessage.TxId, rippleTxMessage.Amount);
                await base.Receive(message);
            }
        }

        public override Task OnActivateAsync()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);
            OrleansSchedulerContainer.AddOrUpdate(grainId, TaskScheduler.Current, (k, v) => TaskScheduler.Current);
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            string extKey; TaskScheduler tmp;

            var grainId = this.GetPrimaryKey(out extKey);
            OrleansSchedulerContainer.TryRemove(grainId, out tmp);
            return base.OnDeactivateAsync();
        }
        protected override Task StartMessageConsumer()
        {
            this.channel.ExchangeDeclare(MqExchangeName, ExchangeType.Direct, true, false, null);
            this.channel.QueueDeclare(MqQueueName, true, false, false, null);
            this.channel.QueueBind(MqQueueName, MqExchangeName, string.Empty);

            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);

            var consumer = new RippleTxMessageConsumer(this.channel, grainId);
            consumer.OnCancel();
            this.channel.BasicQos(0, 1, false);
            this.channel.BasicConsume(MqQueueName, false, consumer);

            return TaskDone.Done;
        }
        #endregion
    }


    #region MessageConsumer
    internal class RippleTxMessageConsumer : RabbitMqMessageConsumer
    {
        public RippleTxMessageConsumer(IModel model, Guid grainId)
            : base(model, grainId) { }
        public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var messageBody = Encoding.UTF8.GetString(body);
            RippleTxMessage message;

            try
            {
                message = JsonConvert.DeserializeObject<RippleTxMessage>(messageBody);
            }
            catch (Exception ex)
            {
                Model.BasicAck(deliveryTag, false);
                Log.Error("Ripple Tx Message Error Format,Message=" + messageBody, ex);
                return;
            }

            try
            {
                TaskScheduler scheduler;
                RippleToFinancialInstitutionListener.OrleansSchedulerContainer.TryGetValue(grainId, out scheduler);

                Debug.Assert(scheduler != null, "RippleTxMessageConsumer task scheduler is null");

                var rippleToFinancialInstitutionListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionListener>(0);

                var tcs = new TaskCompletionSource<bool>();

                Task.Factory.StartNew(() =>
                {
                    rippleToFinancialInstitutionListener.Receive(message).ContinueWith((t
                        =>
                        {
                            if (t.IsFaulted)
                                tcs.SetException(new Exception("RippleTxMessageConsumer Exception", t.Exception));
                            else if (t.IsCompleted)
                                tcs.SetResult(true);
                            else if (t.IsCanceled)
                                tcs.SetException(new Exception("RippleTxMessageConsumer Canneled By Scheduler", t.Exception));
                        }));
                }, CancellationToken.None, TaskCreationOptions.None, scheduler);

                await tcs.Task;

                Model.BasicAck(deliveryTag, false);
            }
            catch (Exception ex)
            {
                Log.Error("Process Ripple Tx Message Error,Message=" + messageBody, ex);
                Model.BasicNack(deliveryTag, false, true);
            }
        }
    }
    [Immutable]
    [Serializable]
    internal class RippleTxMessage : MqMessage
    {
        public string TxId { get; set; }
        public string InvoiceId { get; set; }
        public int DestinationTag { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion

}
