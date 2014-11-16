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
using DotPay.Command;

namespace DotPay.OutsideTransferMonitor
{
    internal class InsideTransferWatcher
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        internal static void Start(MQConnectionPool mqpool)
        {
            if (InsideTransferWatcher.Started)
            {
                Log.Info("内部转账监控器启动");
                return;
            }
            _mqpool = mqpool;

            StartInsideTransferProcessConsumer();
        }

        #region 私有方法

        private static void StartInsideTransferProcessConsumer()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                IModel channel;
                var consumer = GetUserRegisteredConsumer(out channel);

                Log.Info("内部转账-处理器启动！");

                while (true)
                {
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，内部转账-处理器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }
                    Log.Info("正在监听内部转账消息...");
                    BasicDeliverEventArgs ea = null;
                    try
                    {
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        var txMessage = IoC.Resolve<IJsonSerializer>().Deserialize<InsideTransactionCreateMessage>(message);

                        Log.Info("收到内部转账消息:{0}", message);


                        var exist = string.Empty;

                        var cmd = new InsideTransferComplete(txMessage.TransferTxID, txMessage.Currency);

                        IoC.Resolve<ICommandBus>().Send(cmd);
                    }
                    catch (CommandExecutionException ex)
                    {
                        if (ex.ErrorCode == (int)ErrorCode.TransferTransactionNotPending)
                        {
                            Log.Warn("这个交易可能已经被处理或取消了", ex);

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (System.IO.EndOfStreamException ex)
                    {
                        if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Warn("激活用户钱包时，消息队列的链接断开了,准备自动重连", ex);

                            consumer = GetUserRegisteredConsumer(out channel);
                            if (ea != null)
                                channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ea != null)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                        Log.Error("发送完成消息时出现错误", ex);
                    }
                }
            }));

            thread.Start();
        }



        private static QueueingBasicConsumer GetUserRegisteredConsumer(out IModel channel)
        {
            var mqname = "UserRegisterWatcher";
            channel = _mqpool.GetMQChannel(mqname);

            var exchangeAndQueueName = Utilities.GetExchangeAndQueueNameOfUserRegistered();

            channel.ExchangeDeclare(exchangeAndQueueName.Item1, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(exchangeAndQueueName.Item2, true, false, false, null);
            channel.QueueBind(exchangeAndQueueName.Item2, exchangeAndQueueName.Item1, string.Empty);
            channel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(exchangeAndQueueName.Item2, false, consumer);

            return consumer;
        }
        #endregion

        #region 数据实体类
        private class InsideTransactionCreateMessage : TransactionMessage
        {
            public InsideTransactionCreateMessage(int transferTxID, int fromUserID, int toUserID, CurrencyType currency, decimal amount, PayWay payway, string description)
            {
                this.TransferTxID = transferTxID;
                this.FromUserID = fromUserID;
                this.ToUserID = toUserID;
                this.Currency = currency;
                this.Amount = amount;
                this.PayWay = payway;
                this.Description = description;
                this.TransactionFlg = TransactionType.Inside;
            }
            public int FromUserID { get; private set; }
            public int ToUserID { get; private set; }
            public CurrencyType Currency { get; private set; }
            public decimal Amount { get; private set; }
            public PayWay PayWay { get; private set; }
            public string Description { get; private set; }
            public int TransferTxID { get; private set; }
        }

        private abstract class TransactionMessage
        {
            public TransactionType TransactionFlg { get; protected set; }
        }

        private enum TransactionType
        {
            Inside = 1,
            Outside = 2,
            Exchange = 3
        }
        #endregion
    }
}
