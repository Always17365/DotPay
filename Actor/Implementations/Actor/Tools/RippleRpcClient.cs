using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Implementations;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Actor.Tools.Implementations
{
    public class RippleRpcClient : Grain, IRippleRpcClient
    {
        private GetLastLedgerIndexRpcClient indexRpcClient;
        private ValidatorRpcClient txValidatorClient;
        Task<long> IRippleRpcClient.GetLastLedgerIndex()
        {
            return indexRpcClient.GetLastLedgerIndex();
        }

        Task<RippleTransactionValidateResult> IRippleRpcClient.ValidateRippleTx(string txid)
        {
            return txValidatorClient.ValidateTx(txid);
        }

        public override Task OnActivateAsync()
        {
            indexRpcClient = new GetLastLedgerIndexRpcClient(RabbitMqConnectionManager.GetConnection());
            txValidatorClient = new ValidatorRpcClient(RabbitMqConnectionManager.GetConnection());
            return base.OnActivateAsync();
        }

        #region rpc client

        private class GetLastLedgerIndexRpcClient
        {
            private readonly IModel _channel;
            private readonly string _replyQueueName;
            private readonly ConcurrentDictionary<string, TaskCompletionSource<long>> _requsetDic = new ConcurrentDictionary<string, TaskCompletionSource<long>>();

            private const string RIPPLE_VALIDATE_EXCHANGE_NAME =
                Constants.RippleValidatorMQName + Constants.ExechangeSuffix;
            private const string RIPPLE_VALIDATE_QUEUE =
                Constants.RippleValidatorMQName + Constants.QueueSuffix;
            private const string RIPPLE_LAST_LEDGER_INDEX_QUEUE_REPLY =
                Constants.RippleLedgerIndexResultQueueName + Constants.QueueSuffix;

            public GetLastLedgerIndexRpcClient(IConnection connection)
            {
                this._channel = connection.CreateModel();
                this._replyQueueName = this._channel.QueueDeclare(RIPPLE_LAST_LEDGER_INDEX_QUEUE_REPLY, true, false, false, null);
                this._channel.ExchangeDeclare(RIPPLE_VALIDATE_EXCHANGE_NAME, ExchangeType.Direct);
                this._channel.QueueDeclare(RIPPLE_VALIDATE_QUEUE, true, false, false, null);
                this._channel.QueueBind(RIPPLE_VALIDATE_QUEUE, RIPPLE_VALIDATE_EXCHANGE_NAME, "");
                var consumer = new ValidatorMessageConsumer(this._channel, (p, b) =>
                {
                    TaskCompletionSource<long> ts;
                    this._requsetDic.TryRemove(p.CorrelationId, out ts);

                    if (ts != null)
                    {
                        try
                        {
                            var result = Convert.ToInt64(Encoding.UTF8.GetString(b));
                            ts.SetResult(result);
                        }
                        catch
                        {
                            ts.SetResult(-1);
                        }
                    }
                });
                this._channel.BasicConsume(this._replyQueueName, true, consumer);

            }

            public Task<long> GetLastLedgerIndex()
            {
                var corrId = Guid.NewGuid().ToString();
                var props = this._channel.CreateBasicProperties();
                props.ReplyTo = this._replyQueueName;
                props.CorrelationId = corrId;
                props.Expiration = "10000";

                var message = IoC.Resolve<IJsonSerializer>().Serialize(new GetLastLedgerIndexRequestMessage(RippleValidateRequestType.GetLedgerIndex));
                var messageBytes = Encoding.UTF8.GetBytes(message);
                this._channel.BasicPublish(RIPPLE_VALIDATE_EXCHANGE_NAME, string.Empty, props, messageBytes);
                var ts = new TaskCompletionSource<long>();

                this._requsetDic.TryAdd(corrId, ts);

                Task.Factory.StartNew(() =>
                {
                    Task.Delay(10000).ContinueWith((t) =>
                    {
                        TaskCompletionSource<long> tsGet;
                        if (this._requsetDic.TryRemove(corrId, out tsGet))
                        {
                            tsGet.SetResult(-1);
                        }
                    });
                });
                return ts.Task;
            }
        }

        private class ValidatorRpcClient
        {
            private readonly IModel channel;
            private readonly DefaultBasicConsumer consumer;
            private readonly string replyQueueName;
            private IConnection connection;
            private ConcurrentDictionary<string, TaskCompletionSource<RippleTransactionValidateResult>> requsetDic = new ConcurrentDictionary<string, TaskCompletionSource<RippleTransactionValidateResult>>();
            private const string RIPPLE_VALIDATE_EXCHANGE_NAME =
              Constants.RippleValidatorMQName + Constants.ExechangeSuffix;
            private const string RIPPLE_VALIDATE_QUEUE =
                Constants.RippleValidatorMQName + Constants.QueueSuffix;
            private const string RIPPLE_VALIDATE_QUEUE_REPLY =
                Constants.RippleValidateResultQueueName + Constants.QueueSuffix;

            public ValidatorRpcClient(IConnection connection)
            {
                this.connection = connection;
                channel = connection.CreateModel();
                replyQueueName = channel.QueueDeclare(RIPPLE_VALIDATE_QUEUE_REPLY, true, false, false, null);
                channel.ExchangeDeclare(RIPPLE_VALIDATE_EXCHANGE_NAME, ExchangeType.Direct);
                channel.QueueDeclare(RIPPLE_VALIDATE_QUEUE, true, false, false, null);
                channel.QueueBind(RIPPLE_VALIDATE_QUEUE, RIPPLE_VALIDATE_EXCHANGE_NAME, "");
                consumer = new ValidatorMessageConsumer(channel, (p, b) =>
                {
                    TaskCompletionSource<RippleTransactionValidateResult> ts;
                    requsetDic.TryRemove(p.CorrelationId, out ts);

                    if (ts != null)
                    {
                        try
                        {
                            var result = Convert.ToInt32(Encoding.UTF8.GetString(b));
                            ts.SetResult((RippleTransactionValidateResult)result);
                        }
                        catch
                        {
                            ts.SetResult(RippleTransactionValidateResult.Timeout);
                        }
                    }
                });
                channel.BasicConsume(replyQueueName, true, consumer);
            }

            /// <summary>
            /// </summary>
            /// <param name="txid"></param>
            /// <returns>-1 超时，1 true, 0 false</returns>
            public Task<RippleTransactionValidateResult> ValidateTx(string txid)
            {
                var corrId = Guid.NewGuid().ToString();
                var props = channel.CreateBasicProperties();
                props.ReplyTo = replyQueueName;
                props.CorrelationId = corrId;
                props.Expiration = "10000";

                var message = IoC.Resolve<IJsonSerializer>().Serialize(new ValidateTxRequestMessage(RippleValidateRequestType.ValidatorTx, txid));
                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(RIPPLE_VALIDATE_EXCHANGE_NAME, string.Empty, props, messageBytes);

                var ts = new TaskCompletionSource<RippleTransactionValidateResult>();

                requsetDic.TryAdd(corrId, ts);

                Task.Factory.StartNew(() =>
                {
                    Task.Delay(10000).ContinueWith((t) =>
                    {
                        TaskCompletionSource<RippleTransactionValidateResult> tsGet;
                        if (requsetDic.TryRemove(corrId, out tsGet))
                        {
                            tsGet.SetResult(RippleTransactionValidateResult.Timeout);
                        }
                    });
                });
                return ts.Task;
            }
        }

        #region Consumer

        private class ValidatorMessageConsumer : DefaultBasicConsumer
        {
            private Action<IBasicProperties, byte[]> action;

            public ValidatorMessageConsumer(IModel model, Action<IBasicProperties, byte[]> task)
                : base(model)
            {
                this.action = task;
            }

            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);

                try
                {
                    this.action(properties, body);
                }
                catch (Exception ex)
                {
                    Log.Error("ValidatorMessageConsumer Deserialize Message Exception.", ex);
                }
            }
        }
        #endregion

        #endregion
    }
}
