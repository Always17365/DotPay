using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Service.Interfaces;
using Dotpay.Actor.Tools.Interfaces;
using Dotpay.Actor.Implementations;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using RabbitMQ.Client;
using DFramework;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class DepositTransactionManager : MessageConsumerAbstractGrain, IDepositTransactionManager
    {
        private const string MqExchangeName = "__DepositTransactionManagerExchange";
        private const string MqQueueName = "__DepositTransactionManagerQueue";
        private static readonly ConcurrentDictionary<Guid, TaskScheduler> OrleansSchedulerContainer = new ConcurrentDictionary<Guid, TaskScheduler>();
        private IModel channel;

        async Task IDepositTransactionManager.CreateDepositTransaction(Guid depositTxId, Guid accountId, CurrencyType currency, decimal amount, Payway payway, string memo)
        {
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
            var depositSequenceNo = await GeneratorDepositTransactionSequenceNo(payway);
            await depositTx.Initiliaze(depositSequenceNo, accountId, currency, amount, payway, memo);
        }

        async Task IDepositTransactionManager.ConfirmDepositTransaction(Guid depositTxId, Guid operatorId, string transactionNo)
        {
            var message = new ConfirmDepositTransactionMessage(depositTxId, operatorId, transactionNo);

            await MessageProducterManager.GetProducter().PublishMessage(message, MqExchangeName, durable: true);
            await ProcessConfirmDepositTransaction(message);
        }

        async Task IDepositTransactionManager.DepositTransactionMarkAsFail(Guid depositTxId, Guid operatorId, string reason)
        {
            //try
            //{
            var depositTx = GrainFactory.GetGrain<IDepositTransaction>(depositTxId);
            var depositTxState = await depositTx.GetStatus();

            if (depositTxState == DepositStatus.Started)
            {
                await depositTx.Fail(operatorId, reason);
            }
            //}
            //catch (Exception ex)
            //{
            //    this.GetLogger().Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            //}
        }



        #region override
        protected override Task<bool> CheckSelfCanBeDeactive()
        {
            if (OrleansSchedulerContainer.Count == 1)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }
        public override async Task Receive(MqMessage message)
        {
            var confirmMessage = message as ConfirmDepositTransactionMessage;

            if (confirmMessage != null)
                await ProcessConfirmDepositTransaction(confirmMessage);

            await base.Receive(message);
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
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);
            TaskScheduler tmp;
            OrleansSchedulerContainer.TryRemove(grainId, out tmp);

            return base.OnDeactivateAsync();
        }
        protected override Task StartMessageConsumer()
        {
            channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            channel.ExchangeDeclare(MqExchangeName, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(MqQueueName, true, false, false, null);
            channel.QueueBind(MqQueueName, MqExchangeName, string.Empty);

            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);

            var consumer = new ConfirmDepositTransactionMessageConsumer(channel, grainId);
            channel.BasicQos(0, 1, false);
            channel.BasicConsume(MqQueueName, false, consumer);

            return TaskDone.Done;
        }
        #endregion

        #region Private Methods
        private Task<string> GeneratorDepositTransactionSequenceNo(Payway payway)
        {
            //sequenceNoGeneratorId设计中只有5位,SequenceNoType占据3位 payway占据2位
            var sequenceNoGeneratorId = Convert.ToInt64(SequenceNoType.DepositTransaction.ToString() + payway.ToString());
            var sequenceNoGenerator = GrainFactory.GetGrain<ISequenceNoGenerator>(sequenceNoGeneratorId);

            return sequenceNoGenerator.GetNext();
        }
        private async Task ProcessConfirmDepositTransaction(ConfirmDepositTransactionMessage confirmMessage)
        {
            try
            {
                var depositTx = GrainFactory.GetGrain<IDepositTransaction>(confirmMessage.DepositTxId);
                var depositTxState = await depositTx.GetStatus();
                var depositTxInfo = await depositTx.GetTransactionInfo();
                var account = GrainFactory.GetGrain<IAccount>(depositTxInfo.AccountId);

                if (depositTxState == DepositStatus.Started)
                {
                    await account.AddTransactionPreparation(confirmMessage.DepositTxId, TransactionType.DepositTransaction,
                        PreparationType.CreditPreparation, depositTxInfo.Currency, depositTxInfo.Amount);
                    await depositTx.ConfirmDepositPreparation();
                }

                depositTxState = await depositTx.GetStatus();
                if (depositTxState == DepositStatus.PreparationCompleted)
                {
                    await account.CommitTransactionPreparation(confirmMessage.DepositTxId);
                    await depositTx.ConfirmDeposit(confirmMessage.OperatorId, confirmMessage.TransactionNo);
                }
            }
            catch (Exception ex)
            {
                this.GetLogger("ProcessConfirmDepositTransaction").Error((int)ErrorCode.DepositTransactionManagerError, ex.Message, ex);
            }
        }
        #endregion

        #region Message Consumer

        #region Consumer
        internal class ConfirmDepositTransactionMessageConsumer : RabbitMqMessageConsumer
        {

            public ConfirmDepositTransactionMessageConsumer(IModel model, Guid grainId)
                : base(model, grainId) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<ConfirmDepositTransactionMessage>(messageBody);

                try
                {
                    TaskScheduler scheduler;
                    OrleansSchedulerContainer.TryGetValue(grainId, out scheduler);

                    Debug.Assert(scheduler != null, "ConfirmDepositTransactionMessageConsumer task scheduler is null");

                    var depositTransactionManager = GrainFactory.GetGrain<IDepositTransactionManager>(0);

                    var tcs = new TaskCompletionSource<bool>();

                    Task.Factory.StartNew(() =>
                    {
                        depositTransactionManager.Receive(message).ContinueWith((t
                        =>
                        {
                            if (t.IsFaulted)
                                tcs.SetException(new Exception("ConfirmDepositTransactionMessageConsumer Exception", t.Exception));
                            else if (t.IsCompleted)
                                tcs.SetResult(true);
                            else if (t.IsCanceled)
                                tcs.SetException(new Exception("ConfirmDepositTransactionMessageConsumer Canneled By Scheduler", t.Exception));
                        }));
                    },
                     CancellationToken.None, TaskCreationOptions.None, scheduler);

                    await tcs.Task;

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Model.BasicNack(deliveryTag, false, true);
                    Log.Error("ConfirmDepositTransactionMessageConsumer Exception,Message=" + messageBody, ex);
                }
            }
        }
        #endregion

        [Immutable]
        [Serializable]
        private class ConfirmDepositTransactionMessage : MqMessage
        {
            public ConfirmDepositTransactionMessage(Guid depositTxId, Guid operatorId, string transactionNo)
            {
                this.DepositTxId = depositTxId;
                this.OperatorId = operatorId;
                this.TransactionNo = transactionNo;
            }

            public Guid DepositTxId { get; private set; }
            public Guid OperatorId { get; private set; }
            public string TransactionNo { get; private set; }
        }
        #endregion
    }
}
