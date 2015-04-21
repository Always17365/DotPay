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
    internal class EmailMessageMonitor : IApplicationMonitor
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

            var consumer = new EmailMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }
        #region Consumer

        private class EmailMessageConsumer : DefaultBasicConsumer
        {
            public EmailMessageConsumer(IModel model) : base(model) { }

            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
                string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                MqMessage message;

                try
                {
                    var regMessage = JsonConvert.DeserializeObject<UserRegisterMessage>(messageBody);
                    if (!string.IsNullOrEmpty(regMessage.ActiveToken))
                    {
                        Log.Debug("收到注册邮件消息:"+messageBody);
                    }
                    else
                    {
                        var forgetMessage = JsonConvert.DeserializeObject<UserForgetPasswordMessage>(messageBody);

                        if (forgetMessage.Type == typeof (UserForgetLoginPasswordMessage).Name)
                        {
                            message = forgetMessage.ToUserForgetLoginPasswordMessage();
                            Log.Debug("收到找回登陆密码邮件消息:" + messageBody);
                        }
                        else
                        {
                            message = forgetMessage.ToUserForgetPaymentPasswordMessage();
                            Log.Debug("收到找回支付密码邮件消息:" + messageBody);
                        }
                    }
                }
                catch (Exception ex)
                {

                    Log.Error("EmailMessageConsumer Message Error Format,Message=" + messageBody, ex);
                    return;
                } 
            }
        }

        #endregion
    }
}
