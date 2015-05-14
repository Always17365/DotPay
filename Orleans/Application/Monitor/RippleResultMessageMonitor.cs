using System;
using System.Text;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.Common;
using Newtonsoft.Json;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Application.Monitor
{
    //Dotpay转账到Ripple后，此处主要监听Ripple处理交易的结果
    internal class RippleResultMessageMonitor : IApplicationMonitor
    {
        private IModel _channel;
        private bool started;
        public void Start()
        {
            if (started) return;

            StartMessageConsumer();
            started = true;
        }

        public void Stop()
        {
            if (started)
            {
                if (_channel != null)
                    _channel.Close();

                started = false;
            }
        }

        private void StartMessageConsumer()
        {
            var exchangeName = Constants.RippleTxResultMQName + Constants.ExechangeSuffix;
            var routeKey = string.Empty;
            var queueName = Constants.RippleTxResultMQName + Constants.QueueSuffix;

            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routeKey);

            var consumer = new RippleResultMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class RippleResultMessageConsumer : DefaultBasicConsumer
        {
            public RippleResultMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
                string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);

                try
                {
                    var message = JsonConvert.DeserializeObject<TransferTransactionMessage>(messageBody);

                    if (message.Type == (uint)TransferTransactionMessageType.RippleTransactionPresubmitMessage)
                    {
                        message = JsonConvert.DeserializeObject<RippleTransactionPresubmitMessage>(messageBody);
                    }
                    else if (message.Type == (uint)TransferTransactionMessageType.RippleTransactionResultMessage)
                    {
                        message = JsonConvert.DeserializeObject<RippleTransactionResultMessage>(messageBody);
                    }
                    Log.Info("收到ripple的转账结果消息" + messageBody);
                    var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
                    await transferTransactionManager.Receive(message);

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {

                    Log.Error("RippleResultMessageConsumer Message Error Format,Message=" + messageBody, ex);
                    return;
                }
            }
        }

        #endregion
    }
}
