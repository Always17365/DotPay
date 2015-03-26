using System;
using System.Text;
using DFramework;
using Dotpay.Actor.Service.Interfaces;
using Dotpay.Common;
using Newtonsoft.Json;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Application.Monitor
{
    internal class TransferTransactionMonitor : IApplicationMonitor
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
            var exchangeName = Constants.TransferTransactionManagerMQName + Constants.ExechangeSuffix;
            var queueName = Constants.TransferTransactionManagerQueueName + Constants.QueueSuffix;
            var routeKey = Constants.TransferTransactionManagerRouteKey;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routeKey);

            var consumer = new TransferTransactionMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class TransferTransactionMessageConsumer : DefaultBasicConsumer
        {
            public TransferTransactionMessageConsumer(IModel model) : base(model) { }

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
                    var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
                    await transferTransactionManager.Receive(message);

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
    }
}
