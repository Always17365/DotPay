using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor.Interfaces.Ripple;
using Dotpay.Actor.Service.Interfaces;
using Dotpay.Common;
using Newtonsoft.Json;
using Orleans;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;

namespace Dotpay.Application.Monitor
{
    internal class RippleToFinancialInstitutionMonitor : IApplicationMonitor
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
            var exchangeName = Constants.RippleToFinancialInstitutionMQName + Constants.ExechangeSuffix;
            var queueName = Constants.RippleToFinancialInstitutionMQName + Constants.QueueSuffix;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, string.Empty);

            var consumer = new RippleTxMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }

        #region Message Consumer

        internal class RippleTxMessageConsumer : DefaultBasicConsumer
        {
            public RippleTxMessageConsumer(IModel model) : base(model) { }
            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                RippleTxMessage message;

                try
                {
                    message = JsonConvert.DeserializeObject<RippleTxMessage>(messageBody);
                }
                catch (Exception ex)
                {
                    Model.BasicAck(deliveryTag, false);
                    Log.Error("Ripple Tx Message Error Format,Message=" + messageBody, ex);
                    return;
                }

                try
                {
                    var rippleToFinancialInstitutionListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionListener>(0);

                    await rippleToFinancialInstitutionListener.Receive(message);

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error("Process Ripple Tx Message Error,Message=" + messageBody, ex);
                    Model.BasicNack(deliveryTag, false, true);
                }
            }
        }
        #endregion
    }
}
