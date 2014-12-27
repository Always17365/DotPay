using DotPay.Common;
using FC.Framework;
using FC.Framework.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotPay.Command;
using FluentData;
using RabbitMQ.Client.Content;
using System.Security.AccessControl;
using FC.Framework.Utilities;

namespace DotPay.TransferMonitor
{
    /// <summary>
    /// 此监控器目的在于等待ripple转账提交成功的消息,收到消息后，对dotpay系统内的转账进行确认
    /// </summary>
    internal class OutboundTxProcessor
    {
        private static MQConnectionPool _mqpool;
        internal static void Start(MQConnectionPool mqpool)
        {
            Check.Argument.IsNotNull(mqpool, "mqpool");

            Log.Info("正在转入交易监控器...");
            _mqpool = mqpool;

            StartOutboundTxConfirmConsumer();

            Log.Info("启动转入交易监控器成功！");
        }

        private static void StartOutboundTxConfirmConsumer()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                IModel channel;

                var consumer = GetInboundTxConsumer(out channel);

                while (true)
                {
                    Log.Info("正在监听转入交易消息...");
                    BasicDeliverEventArgs ea = null;
                    var cmdTypeDesc = string.Empty;
                    try
                    {
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        var inboundTPPMsg = IoC.Resolve<IJsonSerializer>().Deserialize<RippleInboundToThirdPartyPaymentTxMessage>(message);

                        if (!string.IsNullOrEmpty(inboundTPPMsg.Account))
                        {
                            //cmdTypeDesc = "第三方支付直转交易";
                            Log.Info("收到新的第三方支付({0})直转交易消息:{1}", inboundTPPMsg.PayWay, message);

                            //var cmd = new CreateThirdPartyPaymentTransfer(inboundTPPMsg.TxId, inboundTPPMsg.Account, inboundTPPMsg.Amount, inboundTPPMsg.PayWay, inboundTPPMsg.SourcePayWay, inboundTPPMsg.Memo);
                            //IoC.Resolve<ICommandBus>().Send(cmd);

                            channel.BasicAck(ea.DeliveryTag, false);

                            Log.Info("成功处理第三方支付({0})直转交易消息");
                        }
                        else
                        {
                            var inboundMsg = IoC.Resolve<IJsonSerializer>().Deserialize<InboundTxMessage>(message);
                            Log.Info("收到新的转入交易消息:{0}", message);

                            var cmd = new CreateInboundCNYDeposit(inboundMsg.ToUserID, inboundMsg.Amount, inboundMsg.PayWay, inboundMsg.TxId);

                            IoC.Resolve<ICommandBus>().Send(cmd);

                            channel.BasicAck(ea.DeliveryTag, false);

                            Log.Info("成功新的转入交易消息");
                        }
                    }
                    catch (NHibernate.Exceptions.GenericADOException ex)
                    {
                        var mysqlex = ex.InnerException as MySql.Data.MySqlClient.MySqlException;
                        if (mysqlex != null && mysqlex.Number == 1062)
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Log.Info("已接收该交易，重复的到帐消息" + message + "，做丢弃处理");
                        }
                        else if (ea != null)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    }
                    catch (System.IO.EndOfStreamException ex)
                    {
                        if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Warn("处理" + cmdTypeDesc + "时，消息队列的链接断开了,准备自动重连", ex);

                            consumer = GetInboundTxConsumer(out channel);
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ea != null)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                        Log.Error("转入交易消息处理过程出现异常:" + ex.Message, ex);
                    }
                }
            }));

            thread.Start();
        }
        private static QueueingBasicConsumer GetInboundTxConsumer(out IModel channel)
        {
            var mqname = "GetInboundTransactionConsumer";

            channel = _mqpool.GetMQChannel(mqname);

            var exchangeAndQueueName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferForSubmit();

            channel.ExchangeDeclare(exchangeAndQueueName.Item1, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(exchangeAndQueueName.Item2, true, false, false, null);
            channel.QueueBind(exchangeAndQueueName.Item2, exchangeAndQueueName.Item1, string.Empty);
            channel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(exchangeAndQueueName.Item2, false, consumer);

            return consumer;
        }

        #region 数据实体类型
        private class InboundTxMessage
        {
            public InboundTxMessage(int toUserID, string txid, PayWay payway, decimal amount)
            {
                this.ToUserID = ToUserID;
                this.TxId = txid;
                this.Amount = amount;
                this.PayWay = payway;
            }
            public int ToUserID { get; private set; }
            public string TxId { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
        }

        [Serializable]
        private class RippleInboundToThirdPartyPaymentTxMessage
        {
            public RippleInboundToThirdPartyPaymentTxMessage(string account, decimal amount, PayWay payway, string txid, string memo)
            {
                this.Account = account;
                this.TxId = txid;
                this.Amount = amount;
                this.PayWay = payway;
                this.SourcePayWay = PayWay.Ripple;
                this.Memo = memo;
            }
            public string Account { get; private set; }
            public string TxId { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
            public PayWay SourcePayWay { get; private set; }
            public string Memo { get; private set; }
        }

        #endregion
    }
}
