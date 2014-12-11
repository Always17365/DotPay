using DotPay.Common;
using DotPay.RippleCommand;
using FC.Framework;
using FC.Framework.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RippleRPC.Net;
using RippleRPC.Net.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentData;
using RabbitMQ.Client.Content;
using System.Security.AccessControl;

namespace DotPay.RippleMonitor
{
    internal class RippleOutboundTransferWatcher
    {
        private const string logTxtName = "ProcessLedgerRecord.txt", logProcessIndexTxtName = "ProcessLedgerIndex";
        public static bool Started { get; private set; }
        private static IModel _channel;
        private static CurrencyType _currency;
        private static IDbContext _dbContext;
        private static MQConnectionPool _mqpool;
        private static int currentProcessLedgerIndex;
        private static object _lock = new object();
        private static bool waitCallback = false;
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        internal static void Start(MQConnectionPool mqpool)
        {
            _mqpool = mqpool;
            _dbContext = new DbContext().ConnectionString(Config.DBConnectString, new MySqlProvider());
            StartSignWatcher();
            Started = true;

        }
        private static void StartSignWatcher()
        {
            if (RippleOutboundTransferWatcher.Started)
            {
                Log.Info("Ripple转出交易SignMonitor已启动");
                return;
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                IModel channel;

                var consumer = GetOutboundTransferTransactionForSignConsumer(out channel);

                Log.Info("Ripple转出交易SignMonitor启动成功");

                while (true)
                {
                    Log.Info("正在监听Ripple转出交易Sign命令...");
                    BasicDeliverEventArgs ea = null;
                    var cmdTypeDesc = string.Empty;
                    try
                    {
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        var outboundTxMsg = IoC.Resolve<IJsonSerializer>().Deserialize<RippleOutboundTxMessage>(message);

                        Log.Info("收到新的Ripple Outbound Tx sign指令:{0}", message);

                        var tx = new Transaction()
                        {
                            Account = Config.RippleAccount,
                            Destination = outboundTxMsg.Destination,
                            DestinationTag = outboundTxMsg.DestinationTag,
                            Amount = new RippleCurrencyValue()
                            {
                                _Value = outboundTxMsg.Amount.ToString(),
                                Issuer = outboundTxMsg.Destination,
                                Currency = outboundTxMsg.Currency
                            },
                            TransactionType = TransactionType.Payment,
                            Paths = outboundTxMsg.Paths
                        };


                        IoC.Resolve<IRippleClientAsync>().Sign(tx, Config.RippleSecret)
                           .ContinueWith((data) =>
                           {
                               if (data.Result.Item1 == null)
                               {
                                   //直接执行sign成功cmd
                                   var cmd = new SignOutboundTx(outboundTxMsg.OutboundTxId, data.Result.Item2.Transaction.Hash, data.Result.Item2.Transaction.TxBlob);
                                   try
                                   {
                                       IoC.Resolve<ICommandBus>().Send(cmd);
                                   }
                                   catch (Exception ex)
                                   {
                                       Log.Error("outboundtx sign cmd{0}执行时出现错误".FormatWith(IoC.Resolve<IJsonSerializer>().Serialize(cmd)), ex);
                                   }
                               }
                               else
                               {
                                   //发送sign失败消息，并在此提交重新sign
                               }
                           })
                           .Start();



                        channel.BasicAck(ea.DeliveryTag, false);

                        Log.Info("成功处理{0}新的交易创建指令");
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
                    catch (CommandExecutionException ex)
                    {
                        if (ex.ErrorCode == (int)ErrorCode.PaymentTransactionIsCompleted)
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Log.Info("充值已处理完毕，重复的指令{1}，丢弃!", message);
                        }
                    }
                    catch (System.IO.EndOfStreamException ex)
                    {
                        if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Warn("处理" + cmdTypeDesc + "时，消息队列的链接断开了,准备自动重连", ex);

                            consumer = GetOutboundTransferTransactionForSignConsumer(out channel);
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ea != null)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                        Log.Error(cmdTypeDesc + "处理过程中出现错误:" + ex.Message, ex);
                    }
                }
            }));

            thread.Start();
        }

        private static QueueingBasicConsumer GetOutboundTransferTransactionForSignConsumer(out IModel channel)
        {
            var mqname = "GetOutboundTransferTransactionForSignConsumer";

            channel = _mqpool.GetMQChannel(mqname);

            var exchangeAndQueueName = Utilities.GenerateExchangeAndQueueNameOfOutboundTransferForSign();

            channel.ExchangeDeclare(exchangeAndQueueName.Item1, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(exchangeAndQueueName.Item2, true, false, false, null);
            channel.QueueBind(exchangeAndQueueName.Item2, exchangeAndQueueName.Item1, string.Empty);
            channel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(exchangeAndQueueName.Item2, true, consumer);

            return consumer;
        }

        [Serializable]
        private class RippleOutboundTxMessage
        {
            public RippleOutboundTxMessage(int outboundTxId, string destination, int destinationtag, decimal amount, string currency, decimal sendMax, List<object> paths)
            {
                this.OutboundTxId = outboundTxId;
                this.Destination = destination;
                this.DestinationTag = destinationtag;
                this.Amount = amount;
                this.Currency = currency;
                this.SendMax = sendMax;
                this.Paths = paths;
            }
            public int OutboundTxId { get; private set; }
            public string Destination { get; private set; }
            public int DestinationTag { get; private set; }
            public decimal Amount { get; private set; }
            public decimal SendMax { get; private set; }
            public string Currency { get; private set; }
            public List<object> Paths { get; private set; }
        }
    }
}
