using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Implementations;
using Dotpay.Actor.Interfaces;
using Dotpay.Actor.Service.Interfaces;
using Dotpay.Actor.Tools.Interfaces;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class TransferTransactionManager : MessageConsumerAbstractGrain, ITransferTransactionManager, IRemindable
    {
        private const string MqTransferExchangeName = "__TransferTransactionManagerExchange";
        private const string MqReminderQueueName = "__TransferTransactionManagerReminderQueue";
        private const string MqReminderRouteKey = "Reminder";
        private const string MqToRippleQueueName = "__TransferToRippleQueue";
        private const string MqToRippleQueueRouteKey = "ToRipple";
        private const string MqRefundExchangeName = "__RefundTransactionManagerExchange";

        private static readonly ConcurrentDictionary<Guid, TaskScheduler> OrleansSchedulerContainer =
            new ConcurrentDictionary<Guid, TaskScheduler>();

        async Task<ErrorCode> ITransferTransactionManager.SubmitTransferTransaction(
            TransferTransactionInfo transactionInfo)
        {
            var transferFromDotpay = transactionInfo.Source as TransferFromDotpayInfo;

            if (transferFromDotpay != null)
            {
                var transferTransactionId = Guid.NewGuid();
                var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
                var transferTransactionSeq =
                    await this.GeneratorTransferTransactionSequenceNo(transactionInfo.Target.Payway);
                await transferTransaction.Initialize(transferTransactionSeq, transactionInfo);
                var message = new SubmitTransferTransactionMessage(transferTransactionId, transferFromDotpay,
                    transactionInfo.Target,
                    transactionInfo.Currency, transactionInfo.Amount, transactionInfo.Memo);
                await
                    MessageProducterManager.GetProducter()
                        .PublishMessage(message, MqTransferExchangeName, MqReminderRouteKey, true);
                return await ProcessSubmitedTransferTransaction(message);
            }

            return ErrorCode.None;
        }

        Task<ErrorCode> ITransferTransactionManager.MarkAsProcessing(Guid transferTransactionId, Guid operatorId)
        {
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            return transferTransaction.MarkAsProcessing(operatorId);
        }

        async Task<ErrorCode> ITransferTransactionManager.ConfirmTransactionFail(Guid transferTransactionId,
            Guid operatorId, string reason)
        {
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var code = await transferTransaction.ConfirmFail(operatorId, reason);
            if (code == ErrorCode.None)
                await PublishRefundMessage(transferTransactionId);

            return code;
        }

        #region override

        protected override Task<bool> CheckSelfCanBeDeactive()
        {
            if (OrleansSchedulerContainer.Count == 1)
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
        public override async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            //Reminder用来做3分钟后检查RippleTx结果
            var transferTxId = new Guid(reminderName);
            var transferTx = GrainFactory.GetGrain<ITransferTransaction>(transferTxId);
            var rippleTxStatus = await transferTx.GetRippleTransactionStatus();

            if (rippleTxStatus == RippleTransactionStatus.Submited)
            {
                var rippleTxInfo = await transferTx.GetRippleTransactionInfo();
                var rippleRpcClient = GrainFactory.GetGrain<IRippleRpcClient>(0);
                var currentLedgerIndex = await rippleRpcClient.GetLastLedgerIndex();
                if (rippleTxInfo.LastLedgerIndex < currentLedgerIndex)
                {
                    var validateResult = await rippleRpcClient.ValidateRippleTx(rippleTxInfo.RippleTxId);

                    if (validateResult == RippleTransactionValidateResult.NotFound)
                    {
                        //resubmit
                        await transferTx.ReSubmitToRipple();
                        var transaferTxInfo = await transferTx.GetTransactionInfo();
                        await
                            PublishSubmitToRippleMessage(transferTxId,
                                (TransferToRippleTargetInfo)transaferTxInfo.Target,
                                transaferTxInfo.Currency, transaferTxInfo.Amount);
                    }
                    else if (validateResult == RippleTransactionValidateResult.Success)
                    {
                        //complete
                        await transferTx.RippleTransactionComplete(rippleTxInfo.RippleTxId);
                    }
                }
            }

            await base.ReceiveReminder(reminderName, status);
        }
        public override async Task Receive(MqMessage message)
        {
            var transactionMessage = message as SubmitTransferTransactionMessage;

            if (transactionMessage != null)
            {
                await ProcessSubmitedTransferTransaction(transactionMessage);
                return;
            }

            var rippleTransactionPresubmitMessage = message as RippleTransactionPresubmitMessage;

            if (rippleTransactionPresubmitMessage != null)
            {
                await ProcessRippleTransactionPresubmitMessage(rippleTransactionPresubmitMessage);
                return;
            }

            await base.Receive(message);
        }

        protected override Task StartMessageConsumer()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);

            var consumer = new TransferTransactionMessageConsumer(channel, grainId);
            channel.BasicQos(0, 1, false);
            channel.BasicConsume(MqReminderQueueName, false, consumer);

            return TaskDone.Done;
        }

        public override async Task OnActivateAsync()
        {
            string extKey;
            var grainId = this.GetPrimaryKey(out extKey);
            OrleansSchedulerContainer.AddOrUpdate(grainId, TaskScheduler.Current, (k, v) => TaskScheduler.Current);
            await RegisterAndBindMqQueue();
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

        #region Private Methods

        private Task<string> GeneratorTransferTransactionSequenceNo(Payway payway)
        {
            //sequenceNoGeneratorId设计中只有5位,SequenceNoType占据3位 payway占据2位
            var sequenceNoGeneratorId = Convert.ToInt64(SequenceNoType.TransferTransaction + payway.ToString());
            var sequenceNoGenerator = GrainFactory.GetGrain<ISequenceNoGenerator>(sequenceNoGeneratorId);

            return sequenceNoGenerator.GetNext();
        }

        private async Task<ErrorCode> ProcessSubmitedTransferTransaction(SubmitTransferTransactionMessage message)
        {
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(message.TransferTransactionId);
            var account = GrainFactory.GetGrain<IAccount>(message.Source.AccountId);



            if (await transferTransaction.GetStatus() == TransferTransactionStatus.Submited)
            {
                var target = message.Target as InnerTransferTargetInfo;

                if (target != null && !await account.Validate())
                {
                    await transferTransaction.Cancel(TransferTransactionCancelReason.TargetAccountNotExist);
                    return ErrorCode.None;
                }

                var opCode = await account.AddTransactionPreparation(message.TransferTransactionId,
                    TransactionType.TransferTransaction, PreparationType.DebitPreparation, message.Currency, message.Amount);

                //如果金额不足
                if (opCode == ErrorCode.AccountBalanceNotEnough)
                {
                    await transferTransaction.Cancel(TransferTransactionCancelReason.BalanceNotEnough);
                    await account.CancelTransactionPreparation(message.TransferTransactionId);
                }

                await transferTransaction.ConfirmTransactionPreparation();
            }

            if (await transferTransaction.GetStatus() == TransferTransactionStatus.PreparationCompleted)
            {
                await account.CommitTransactionPreparation(message.TransferTransactionId);

                if (message.Target.Payway == Payway.Ripple)
                {
                    await transferTransaction.SubmitToRipple();
                    await PublishSubmitToRippleMessage(message.TransferTransactionId,
                        (TransferToRippleTargetInfo)message.Target, message.Currency, message.Amount);
                }
            }

            return ErrorCode.None;
        }

        //处理ripple presubmit message
        private async Task ProcessRippleTransactionPresubmitMessage(RippleTransactionPresubmitMessage message)
        {
            var transferTransactionId = message.TransferTransactionId;
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var txInfo = await transferTransaction.GetTransactionInfo();

            if (message.Currency != txInfo.Currency || txInfo.Amount != message.Amount)
            {
                var errorMsg =
                    "ProcessRippleTransactionPresubmitMessage Currency or Amount not match! tx currency={0},message currency={1}; tx amount={2}, message amount={3}"
                        .FormatWith(txInfo.Currency, message.Currency, txInfo.Amount, message.Amount);
                this.GetLogger().Error(-1, errorMsg);

                Debug.Assert((message.Currency == txInfo.Currency && txInfo.Amount != message.Amount), errorMsg);
            }

            await transferTransaction.RippleTransactionPersubmit(message.RippleTxId, message.LastLedgerIndex);
            //如果遇到一个tx，多次presubmit,每次调用presubmit都会取消上次的Reminder,最后一次的Presubmit Reminder才会生效
            await
                this.RegisterOrUpdateReminder(message.TransferTransactionId.ToString(), TimeSpan.FromMinutes(3),
                    TimeSpan.FromMinutes(2));
        }

        //处理ripple presubmit message
        private async Task ProcessRippleTransactionResultMessage(RippleTransactionResultMessage message)
        {
            var transferTransactionId = message.TransferTransactionId;
            var transferTransaction = GrainFactory.GetGrain<ITransferTransaction>(transferTransactionId);
            var txInfo = await transferTransaction.GetTransactionInfo();

            if (message.Success)
                await transferTransaction.RippleTransactionComplete(message.RippleTxId);
            else
            {
                await transferTransaction.RippleTransactionFail(message.RippleTxId, message.FailedReason);
                await PublishRefundMessage(transferTransactionId);
            }
        }

        private async Task RegisterAndBindMqQueue()
        {
            await MessageProducterManager.RegisterAndBindQueue(MqTransferExchangeName, ExchangeType.Direct, MqReminderQueueName,
                  MqReminderRouteKey, true);
            await MessageProducterManager.RegisterAndBindQueue(MqTransferExchangeName, ExchangeType.Direct, MqToRippleQueueName,
                  MqToRippleQueueRouteKey, true);
        }

        private async Task PublishSubmitToRippleMessage(Guid transferTransactionId, TransferToRippleTargetInfo target,
            CurrencyType currency, decimal amount)
        {
            var toRippleMessage = new SubmitTransferTransactionToRippleMessage(transferTransactionId,
                target, currency, amount);
            await MessageProducterManager.GetProducter()
                .PublishMessage(toRippleMessage, MqTransferExchangeName, MqToRippleQueueRouteKey, true);
        }

        private async Task PublishRefundMessage(Guid transferTransactionId)
        {
            var toRippleMessage = new RefundTransactionMessage(Guid.NewGuid(), transferTransactionId,
                RefundTransactionType.TransferRefund);
            await MessageProducterManager.GetProducter()
                .PublishMessage(toRippleMessage, MqRefundExchangeName, "", true);
        }

        #endregion

        #region Message Consumer

        #region Consumer

        private class TransferTransactionMessageConsumer : RabbitMqMessageConsumer
        {
            public TransferTransactionMessageConsumer(IModel model, Guid grainId)
                : base(model, grainId)
            {
            }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
                string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                TransferTransactionMessage message;

                try
                {
                    message = JsonConvert.DeserializeObject<TransferTransactionMessage>(messageBody);

                    if (message.Type == typeof(SubmitTransferTransactionMessage).Name)
                    {
                        message = JsonConvert.DeserializeObject<SubmitTransferTransactionMessage>(messageBody);
                    }
                    else if (message.Type == typeof(RippleTransactionPresubmitMessage).Name)
                    {
                        message = JsonConvert.DeserializeObject<RippleTransactionPresubmitMessage>(messageBody);
                    }
                    else if (message.Type == typeof(RippleTransactionResultMessage).Name)
                    {
                        message = JsonConvert.DeserializeObject<RippleTransactionResultMessage>(messageBody);
                    }
                }
                catch (Exception ex)
                {
                    Model.BasicAck(deliveryTag, false);
                    Log.Error("TransferTransactionMessageConsumer Message Error Format,Message=" + messageBody, ex);
                    return;
                }


                try
                {
                    TaskScheduler scheduler;
                    OrleansSchedulerContainer.TryGetValue(grainId, out scheduler);

                    Debug.Assert(scheduler != null,
                        "TransferTransactionMessageConsumer message consumer task scheduler is null");

                    var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);

                    var tcs = new TaskCompletionSource<bool>();

                    Task.Factory.StartNew(() =>
                    {
                        transferTransactionManager.Receive(message).ContinueWith((t =>
                        {
                            if (t.IsFaulted)
                                tcs.SetException(new Exception("TransferTransactionMessageConsumer Exception",
                                    t.Exception));
                            else if (t.IsCompleted)
                                tcs.SetResult(true);
                            else if (t.IsCanceled)
                                tcs.SetException(
                                    new Exception("TransferTransactionMessageConsumer Canneled By Scheduler",
                                        t.Exception));
                        }));
                    },
                        CancellationToken.None, TaskCreationOptions.None, scheduler);

                    await tcs.Task;

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Model.BasicNack(deliveryTag, false, true);
                    Log.Error("TransferTransactionMessageConsumer Exception,Message=" + messageBody, ex);
                }
            }
        }

        #endregion

        #region Mq Message

        [Immutable]
        [Serializable]
        internal class TransferTransactionMessage : MqMessage
        {
            public TransferTransactionMessage(string typeName)
            {
                this.Type = typeName;
            }

            public string Type { get; private set; }
        }

        #region SubmitTransferTransactionMessage

        [Immutable]
        [Serializable]
        internal class SubmitTransferTransactionMessage : TransferTransactionMessage
        {
            public SubmitTransferTransactionMessage(Guid transferTransactionId, TransferFromDotpayInfo source,
                TransferTargetInfo target, CurrencyType currency, decimal amount, string memo)
                : base(typeof(SubmitTransferTransactionMessage).Name)
            {
                this.Source = source;
                this.Target = target;
                this.Currency = currency;
                this.Amount = amount;
                this.Memo = memo;
                this.TransferTransactionId = transferTransactionId;
            }

            public Guid TransferTransactionId { get; private set; }
            public TransferFromDotpayInfo Source { get; private set; }
            public TransferTargetInfo Target { get; private set; }
            public CurrencyType Currency { get; private set; }
            public decimal Amount { get; private set; }
            public string Memo { get; private set; }
        }

        #endregion

        #region SubmitTransferTransactionToRippleMessage

        [Immutable]
        [Serializable]
        internal class SubmitTransferTransactionToRippleMessage : MqMessage
        {
            public SubmitTransferTransactionToRippleMessage(Guid transferTransactionId,
                TransferToRippleTargetInfo target, CurrencyType currency, decimal amount)
            {
                this.TransferTransactionId = transferTransactionId;
                this.Target = target;
                this.Currency = currency;
                this.Amount = amount;
            }

            public Guid TransferTransactionId { get; private set; }
            public TransferToRippleTargetInfo Target { get; private set; }
            public CurrencyType Currency { get; private set; }
            public decimal Amount { get; private set; }
        }

        #endregion

        #region RippleTransactionPresubmitMessage

        [Immutable]
        [Serializable]
        internal class RippleTransactionPresubmitMessage : TransferTransactionMessage
        {
            public RippleTransactionPresubmitMessage(Guid transferTransactionId, string rippleTxId,
                CurrencyType currency, decimal amount, long lastLedgerIndex)
                : base(typeof(RippleTransactionPresubmitMessage).Name)
            {
                this.TransferTransactionId = transferTransactionId;
                this.RippleTxId = rippleTxId;
                this.Currency = currency;
                this.Amount = amount;
                this.LastLedgerIndex = lastLedgerIndex;
            }

            public Guid TransferTransactionId { get; private set; }
            public string RippleTxId { get; private set; }
            public CurrencyType Currency { get; private set; }
            public decimal Amount { get; private set; }
            public long LastLedgerIndex { get; private set; }
        }

        #endregion

        #region RippleTransactionResultMessage

        [Immutable]
        [Serializable]
        internal class RippleTransactionResultMessage : TransferTransactionMessage
        {
            public RippleTransactionResultMessage(Guid transferTransactionId, string rippleTxId, bool success,
                RippleTransactionFailedType failedReason)
                : base(typeof(RippleTransactionResultMessage).Name)
            {
                this.TransferTransactionId = transferTransactionId;
                this.RippleTxId = rippleTxId;
                this.Success = success;
                this.FailedReason = failedReason;
            }

            public Guid TransferTransactionId { get; private set; }
            public string RippleTxId { get; private set; }
            public bool Success { get; private set; }
            public RippleTransactionFailedType FailedReason { get; private set; }
        }

        #endregion
        #endregion

        #endregion
    }
}