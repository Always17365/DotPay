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
    internal class RefundTransactionMonitor : IApplicationMonitor
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
            var exchangeName = Constants.RefundTransactionManagerMQName + Constants.ExechangeSuffix;
            var queueName = Constants.RefundTransactionManagerMQName + Constants.QueueSuffix;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, string.Empty);

            //考虑做一个负载,启动多个消费者
            var consumer = new RefundTransactionMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }

        #region Message Consumer

        private class RefundTransactionMessageConsumer : DefaultBasicConsumer
        { 
            public RefundTransactionMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<RefundTransactionMessage>(messageBody);

                try
                {
                    var refundTransactionManager = GrainFactory.GetGrain<IRefundTransactionManager>(10);
                    await refundTransactionManager.Receive(message);

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
}
