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
    /// <summary>
    /// <remarks>
    ///       此处收到的消息应该有2种
    ///         1.直通车的消息
    ///         2.点付用户的收款消息--可以作为一种充值手段
    ///       但得到的消息数据结构是一致的，至于到底是哪一个类，需要根据InvoiceId和DestinationTag做判断
    /// </remarks>
    /// </summary>
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

            var consumer = new RippleToFITxMessageMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }

        #region Message Consumer

        private class RippleToFITxMessageMessageConsumer : DefaultBasicConsumer
        {
            public RippleToFITxMessageMessageConsumer(IModel model) : base(model) { }
            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                RippleToFITxMessage message;
                //destinationTag如何取出用户的Id
                try
                {
                    message = JsonConvert.DeserializeObject<RippleToFITxMessage>(messageBody);
                }
                catch (Exception ex)
                {
                    Model.BasicAck(deliveryTag, false);
                    Log.Error("Ripple Tx Message Error Format,Message=" + messageBody, ex);
                    return;
                }

                try
                {
                    var rippleToFinancialInstitutionListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionProcessor>(0);

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
