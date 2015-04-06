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
    internal class DepositRecheckerMonitor : IApplicationMonitor
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
            var exchangeName = Constants.DepositTransactionManagerMQName + Constants.ExechangeSuffix;
            var queueName = Constants.DepositTransactionManagerMQName + Constants.QueueSuffix;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, string.Empty);

            var consumer = new DepositTransactionMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class DepositTransactionMessageConsumer : DefaultBasicConsumer
        {

            public DepositTransactionMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                MqMessage message = null;

                try
                {
                    message = JsonConvert.DeserializeObject<ConfirmDepositTransactionMessage>(messageBody);
                }
                catch (Exception ex)
                {
                    Log.Error("DepositTransactionMessageConsumer Deserialize Message Exception.", ex);
                }

                try
                {
                    var depositTransactionManager = GrainFactory.GetGrain<IDepositTransactionManager>(10);

                    await depositTransactionManager.Receive(message);

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Model.BasicNack(deliveryTag, false, true);
                    Log.Error("DepositRecheckerMonitor Exception,Message=" + messageBody, ex);
                }
            }
        }
        #endregion
    }
}
