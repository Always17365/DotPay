using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using DFramework;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Actor.Implementations;
using Dotpay.Common.Enum;
using Newtonsoft.Json;
using Orleans;
﻿using Orleans.Concurrency;
using Orleans.Runtime;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    public class RefundTransactionManager : MessageConsumerAbstractGrain, IRefundTransactionManager
    {
        private const string MqRefundExchangeName = "__RefundTransactionManagerExchange";
        private const string MqRefundQueueName = "__RefundTransactionManagerQueue";
        private static readonly ConcurrentDictionary<Guid, TaskScheduler> OrleansSchedulerContainer = new ConcurrentDictionary<Guid, TaskScheduler>();

        #region Override
        public override async Task Receive(MqMessage message)
        {
            var refundMessage = message as RefundTransactionMessage;

            if (refundMessage != null)
            {
                if (refundMessage.RefundTransactionType == RefundTransactionType.TransferRefund)
                {
                    var transferTx = GrainFactory.GetGrain<ITransferTransaction>(refundMessage.TransactionId);
                    var transferTxInfo = await transferTx.GetTransactionInfo();
                    var refundTx = GrainFactory.GetGrain<IRefundTransaction>(refundMessage.RefundTransactionId);
                    var account = GrainFactory.GetGrain<IAccount>(transferTxInfo.Source.AccountId);

                    if (await refundTx.GetStatus() < RefundTransactionStatus.Initalized)
                    {
                        await refundTx.Initiliaze(refundMessage.TransactionId, transferTxInfo.Source.AccountId,
                            refundMessage.RefundTransactionType, transferTxInfo.Currency, transferTxInfo.Amount);
                    }

                    if (await refundTx.GetStatus() == RefundTransactionStatus.Initalized)
                    {
                        await account.AddTransactionPreparation(refundMessage.RefundTransactionId,
                                TransactionType.RefundTransaction, PreparationType.CreditPreparation,
                                transferTxInfo.Currency, transferTxInfo.Amount);
                        await refundTx.ConfirmRefundPreparation();
                    }

                    if (await refundTx.GetStatus() == RefundTransactionStatus.PreparationCompleted)
                    {
                        await account.CommitTransactionPreparation(refundMessage.RefundTransactionId);
                        await refundTx.Confirm();
                    }
                }
            }

            await base.Receive(message);
        }

        protected override Task<bool> CheckSelfCanBeDeactive()
        {
            if (OrleansSchedulerContainer.Count == 1)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }

        protected override async Task StartMessageConsumer()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);

            var consumer = new RefundTransactionMessageConsumer(channel, grainId);
            channel.BasicQos(0, 1, false);
            channel.BasicConsume(MqRefundQueueName, false, consumer); 
        }

        public override async Task OnActivateAsync()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);

            OrleansSchedulerContainer.AddOrUpdate(grainId, TaskScheduler.Current, (k, v) => TaskScheduler.Current);
            await MessageProducterManager.RegisterAndBindQueue(MqRefundExchangeName, ExchangeType.Direct, MqRefundQueueName, "", true);
            await base.OnActivateAsync();
        }
        public override Task OnDeactivateAsync()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);
            TaskScheduler tmp;
            OrleansSchedulerContainer.TryRemove(grainId, out tmp);

            return base.OnDeactivateAsync();
        }
        #endregion

        #region Message Consumer
        internal class RefundTransactionMessageConsumer : RabbitMqMessageConsumer
        {

            public RefundTransactionMessageConsumer(IModel model, Guid grainId)
                : base(model, grainId) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<RefundTransactionMessage>(messageBody);

                try
                {
                    TaskScheduler scheduler;
                    OrleansSchedulerContainer.TryGetValue(grainId, out scheduler);

                    Debug.Assert(scheduler != null, "RefundTransactionMessageConsumer task scheduler is null");

                    var refundTransactionManager = GrainFactory.GetGrain<IRefundTransactionManager>(0);
                    var tcs = new TaskCompletionSource<bool>();

                    Task.Factory.StartNew(() =>
                    {
                        refundTransactionManager.Receive(message).ContinueWith((t
                         =>
                        {
                            if (t.IsFaulted)
                                tcs.SetException(new Exception("RefundTransactionMessageConsumer Exception", t.Exception));
                            else if (t.IsCompleted)
                                tcs.SetResult(true);
                            else if (t.IsCanceled)
                                tcs.SetException(new Exception("RefundTransactionMessageConsumer Canneled By Scheduler", t.Exception));
                        }));
                    },
                    CancellationToken.None, TaskCreationOptions.None, scheduler);

                    await tcs.Task;
                    Model.BasicAck(deliveryTag, false);

                }
                catch (Exception ex)
                {
                    Model.BasicNack(deliveryTag, false, true);
                    Log.Error("RefundTransactionMessageConsumer Exception,message=" + messageBody, ex);
                }
            }
        }
        #endregion
    }
    [Immutable]
    [Serializable]
    internal class RefundTransactionMessage : MqMessage
    {
        public RefundTransactionMessage(Guid refundTransactionId, Guid transactionId, RefundTransactionType refundTransactionType)
        {
            this.RefundTransactionId = refundTransactionId;
            this.TransactionId = transactionId;
            this.RefundTransactionType = refundTransactionType;
        }

        public Guid RefundTransactionId { get; private set; }
        public Guid TransactionId { get; private set; }
        public RefundTransactionType RefundTransactionType { get; private set; }
    }
}
