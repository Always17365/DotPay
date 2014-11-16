using DotPay.Common;
using DotPay.Command;
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

namespace DotPay.InsideTransferMonitor
{
    internal class InsideTransferCreatedWatcher
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        internal static void Start(MQConnectionPool mqpool)
        {
            if (InsideTransferCreatedWatcher.Started)
            {
                Log.Info("用户注册监控器启动");
                return;
            }
            _mqpool = mqpool;

            StartProcessConsumer();
        }

        #region 私有方法

        private static void StartProcessConsumer()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                IModel channel;
                var consumer = GetUserRegisteredConsumer(out channel);

                Log.Info("用户注册-钱包自动激活监控器启动！");

                while (true)
                {
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，用户注册-钱包自动激活监控器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }
                    Log.Info("正在监听用户注册消息...");
                    BasicDeliverEventArgs ea = null;
                    try
                    {
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        var registerMessage = IoC.Resolve<IJsonSerializer>().Deserialize<UserReisterMessage>(message);

                        Log.Info("收到用户注册消息:{0}", message);


                        var exist = string.Empty;
                        if (!Cache.TryGet("active_ripple_wallet" + registerMessage.UserID, out exist))
                        {
                            var rippleClient = IoC.Resolve<IRippleClient>();
                            var activeAmount = 25;
                            int transferFee = (int)(0.01 * 1000000);

                            var transaction = new Transaction
                            {
                                Flags = TransactionFlags.tfFullyCanonicalSig,
                                DestinationTag = 110,
                                Fee = transferFee,
                                TransactionType = TransactionType.Payment,
                                Account = Config.RippleAccount,
                                Destination = registerMessage.RippleAddress,
                                Amount = new RippleCurrencyValue { Currency = "XRP", Issuer = string.Empty, Value = activeAmount }
                            };


                            rippleClient.Sign(transaction, Config.RippleSecret, (signError, result) =>
                            {
                                if (signError == null)
                                {
                                    //记录转账交易数据
                                    var cmd = new RecordActiveWalletTx(result.Transaction.Hash, result.TxBlob, Config.RippleAccount, registerMessage.RippleAddress, activeAmount, transferFee);
                                    IoC.Resolve<ICommandBus>().Send(cmd);
                                    var recordID = cmd.Result;
                                    Log.Info("SIGN OK" + result.Transaction.Hash);
                                    rippleClient.Submit(result.TxBlob, (submitError, payment) =>
                                    {
                                        try
                                        {
                                            if (submitError != null)
                                            {
                                                Log.Error("激活钱包过程中出现了错误,原消息为:" + message, submitError);
                                                //记录转账失败
                                                var activeSuccessCmd = new ActiveWalletTxSubmitFail(recordID, submitError.Error + submitError.Message);
                                                IoC.Resolve<ICommandBus>().Send(activeSuccessCmd);

                                                if (channel.IsOpen)
                                                    channel.BasicNack(ea.DeliveryTag, false, true);
                                            }
                                            else
                                            {
                                                Log.Info("激活钱包成功" + message);
                                                Cache.Add("active_ripple_wallet" + registerMessage.UserID, string.Empty);
                                                //记录转账成功
                                                var activeSuccessCmd = new ActiveWalletTxSubmitSuccess(recordID);
                                                IoC.Resolve<ICommandBus>().Send(activeSuccessCmd);

                                                if (channel.IsOpen)
                                                    channel.BasicAck(ea.DeliveryTag, false);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error("激活钱包回到过程中出现未处理的异常:" + message, ex);
                                        }
                                    });
                                }
                                else
                                {
                                    Log.Error("激活钱包过程中出现了发送xrp交易签名错误,原消息为:" + message, signError);
                                    if (channel.IsOpen)
                                        channel.BasicNack(ea.DeliveryTag, false, true);
                                }
                            });
                        }
                        else
                        {
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
            public InsideTransactionCreateMessage(int fromUserID, int toUserID, CurrencyType currency, decimal amount, PayWay payway, string description)
            {
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
