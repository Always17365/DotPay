using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Ripple;
using Dotpay.Actor.Service;
using Dotpay.Common;
using Dotpay.Common.Enum;
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
    internal class RippleToFIMonitor : IApplicationMonitor
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
            var exchangeName = Constants.RippleToFIMQName + Constants.ExechangeSuffix;
            var queueName = Constants.RippleToFIMQName + Constants.QueueSuffix;
            _channel = RabbitMqConnectionManager.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, string.Empty);

            var consumer = new RippleTxMessageMessageConsumer(_channel);
            _channel.BasicQos(0, 1, false);
            _channel.BasicConsume(queueName, false, consumer);

        }

        #region Message Consumer

        private class RippleTxMessageMessageConsumer : DefaultBasicConsumer
        {
            public RippleTxMessageMessageConsumer(IModel model) : base(model) { }
            public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                var messageBody = Encoding.UTF8.GetString(body);
                RippleTxMessage message;
                //destinationTag如何取出用户的Id
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
                    //DestinationTag的前两位，是转账到哪里的标志位
                    var destinationTag = message.DestinationTag;

                    var paywayFlg = destinationTag.ToString().Substring(0, 2);
                    var identityFlg = Convert.ToInt64(destinationTag.ToString().Substring(2));
                    Payway payway;
                    Enum.TryParse(paywayFlg, true, out payway);

                    if (payway == Payway.Dotpay)
                    {
                        //如果是直接转到点付用户，则直接发送充值消息，作为一个来自Ripple的自动充值来处理
                        var quote = GrainFactory.GetGrain<IRippleToDotpayQuote>(identityFlg);
                        var quoteInfo = await quote.GetQuoteInfo();
                        var user = GrainFactory.GetGrain<IUser>(quoteInfo.UserId);
                        var depositTxId = Guid.NewGuid();
                        var accountId = await user.GetAccountId();

                        if (!ValidateInvoinceId(accountId, message.DestinationTag, payway, message.Currency,
                                                message.Amount, message.InvoiceId))
                        {
                            Log.Error("RippleTxMessageMessageConsumer InvoiceId Invalid.txid=" + message.TxId);
                        }
                        var rippleDepositMessage = new RippleDepositTransactionMessage(depositTxId, accountId,
                            message.TxId, message.InvoiceId, identityFlg, message.Amount);

                        var depositTransactionManager = GrainFactory.GetGrain<IDepositTransactionManager>(11);

                        await depositTransactionManager.Receive(rippleDepositMessage);
                    }
                    else
                    {
                        //如果是点付直通车的直转，发送直转tx消息，交给RippleToFiService处理
                        var rippleToFiTxMessage = new RippleToFiTxMessage()
                        {
                            RippleToFiTxId = identityFlg,
                            Amount = message.Amount,
                            Currency = message.Currency,
                            InvoiceId = message.InvoiceId,
                            TxId = message.TxId
                        };
                        var rippleToFi = GrainFactory.GetGrain<IRippleToFiService>(0);
                        await rippleToFi.Receive(rippleToFiTxMessage);
                    }

                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error("Process Ripple Tx Message Error,Message=" + messageBody, ex);
                    Model.BasicNack(deliveryTag, false, true);
                }
            }

            private bool ValidateInvoinceId(Guid accountId, long destinationTag, Payway payway, CurrencyType currency, decimal amount, string invoiceId)
            {
                var signMessage = accountId.ToString() + destinationTag + payway + currency + amount;

                var result = GenerateInvoiceId(signMessage);

                return invoiceId.Equals(result, StringComparison.OrdinalIgnoreCase);
            }

            private static string GenerateInvoiceId(string signMessage)
            {
                return Utilities.Sha256Sign(signMessage);
            }
        }
        #endregion
    }
}
