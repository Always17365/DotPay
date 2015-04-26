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
    //这里只处理用户成功注册-激活后的账户初始化
    internal class UserActiveMessageMonitor : IApplicationMonitor
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
            var exchangeName = Constants.UserMQName + Constants.ExechangeSuffix;
            var routeKey = Constants.UserRouteKey;
            var queueName = Constants.UserMQName + Constants.QueueSuffix;

            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routeKey);

            var consumer = new UserActiveMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class UserActiveMessageConsumer : DefaultBasicConsumer
        {
            public UserActiveMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
                string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                UserActivedMessage message;

                try
                {
                    message = JsonConvert.DeserializeObject<UserActivedMessage>(messageBody);
                }
                catch (Exception ex)
                {
#if !DEBUG
                    Model.BasicAck(deliveryTag, false);
#endif
                    Log.Error("UserActiveMessageConsumer Message Error Format,Message=" + messageBody, ex);
                    return;
                } 

                try
                {
                    var account = GrainFactory.GetGrain<IAccount>(message.AccountId);
                    await account.Initialize(message.UserId);
                    Log.Debug("User Active And Account has been initialize");
                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Model.BasicNack(deliveryTag, false, true);
                    Log.Error("UserActiveMessageConsumer Exception,Message=" + messageBody, ex);
                }
            }
        }

        #endregion
    }
}
