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
    //这里只处理用户成功注册后的账户初始化，邮件系统交给单独的应用处理
    internal class UserRegisterMonitor : IApplicationMonitor
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
            var routeKey = Constants.UserEmailRouteKey;
            var queueName = Constants.UserEmailMQName + Constants.QueueSuffix;

            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routeKey);

            var consumer = new UserRegisterMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class UserRegisterMessageConsumer : DefaultBasicConsumer
        {
            public UserRegisterMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
                string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                UserInitializedMessage message;

                try
                {
                    message = JsonConvert.DeserializeObject<UserInitializedMessage>(messageBody);
                }
                catch (Exception ex)
                {
#if !DEBUG
                    Model.BasicAck(deliveryTag, false);
#endif
                    Log.Error("UserRegisterMessageConsumer Message Error Format,Message=" + messageBody, ex);
                    return;
                }


                try
                {
                    var account = GrainFactory.GetGrain<IAccount>(message.AccountId);
                    await account.Initialize(message.UserId);

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
