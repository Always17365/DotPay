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
    /// <summary>
    /// <remarks>
    ///  淘宝消息会有两种，一种是Dotpay直接充值，一种是Ripple直冲
    ///  对于两种不同的充值，在这里对消息进行重分发
    ///  1.dotpay充值,发送
    /// </remarks>
    /// </summary>
    internal class TaobaoDepositMessageMonitor : IApplicationMonitor
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
            var exchangeName = Constants.TaobaoMQName + Constants.ExechangeSuffix;
            var queueName = Constants.TaobaoMQName + Constants.QueueSuffix;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, string.Empty);

            var consumer = new TaobaoMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class TaobaoMessageConsumer : DefaultBasicConsumer
        {

            public TaobaoMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                MqMessage message = null;

                try
                {
                    message = JsonConvert.DeserializeObject<TaobaoDepositTransactionMessage>(messageBody);
                }
                catch (Exception ex)
                {
                    Log.Error("TaobaoMessageConsumer Deserialize Message Exception.", ex);
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
