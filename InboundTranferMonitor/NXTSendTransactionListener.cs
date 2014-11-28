using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using System.Threading;
using System.Net;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using DotPay.Command;
using DotPay.Common;
using FluentData;
using RabbitMQ.Client.Events;
using DotPay.Tools.NXTClient;

namespace DotPay.Tools.NXTMonitor
{
    internal class NXTSendTransactionListener
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static CurrencyType _currency;
        private static NXTClient4Net nxtClient;

        internal static void Start(MQConnectionPool mqpool)
        {
            if (NXTSendTransactionListener.Started)
            {
                Log.Info("{0}提现处理器已启动", Config.CoinCode);
                return;
            }

            nxtClient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase);
            _mqpool = mqpool;

            var currencies = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            StartSendPaymentTransactionConsumer(_currency);
        }

        #region 私有方法

        private static void StartSendPaymentTransactionConsumer(CurrencyType currency)
        {
            var thread = new Thread(new ThreadStart(() =>
             {
                 var consumer = GetSendTransactionListenerConsumer(currency);

                 Log.Info("启动{0}提现处理器成功！", currency);

                 while (true)
                 {
                     if (Program.Fusing)
                     {
                         Log.Info("发生熔断事件，虚拟币提现处理器停止运行");
                         break;
                     }

                     Log.Info("正在监听{0}新的提现指令...", currency);
                     BasicDeliverEventArgs ea = null;
                     try
                     {
                         ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                         var body = ea.Body;
                         var message = Encoding.UTF8.GetString(body);

                         var withdrawMessage = IoC.Resolve<IJsonSerializer>().Deserialize<VirtualCoinWithdrawProcessMessage>(message);

                         Log.Info("收到{0}新的提现指令:{1}", currency, message);

                         if (Config.CoinCode != withdrawMessage.Currency.ToString())
                         {
                             Log.Fatal("收到的提现指令中指定的币种与配置文件中的币种不一致！");
                             break;
                         }

                         var txid = string.Empty;
                         var txfee = 0M;

                         if (TrySendTransactionToP2p(withdrawMessage.MsgID, withdrawMessage.Address, withdrawMessage.NXTPublicKey, withdrawMessage.Amount, out txid, out txfee))
                         {
                             SendCompleteCommand(withdrawMessage.Currency, withdrawMessage.WithdrawUniqueID, txid, txfee);
                         }
                         else
                         {
                             SendWithdrawFailOrAddressInvalidCommand(withdrawMessage.Currency, withdrawMessage.WithdrawUniqueID, VirtualCoinWithdrawFailProcessType.MarkFail);
                             Log.Error("提现处理时，发送交易到P2P网络失败，请及时手工处理该笔交易!msg:" + message);
                         }

                         Log.Info("成功处理{0}提现指令", currency);

                     }
                     catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ex)
                     {
                         Log.Warn("提现NXT处理时，消息队列的链接断开了,准备自动重连", ex);

                         consumer = GetSendTransactionListenerConsumer(currency);
                     }
                     catch (System.IO.EndOfStreamException ex)
                     {
                         if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                         {
                             Log.Warn("提现NXT处理时，消息队列的链接断开了,准备自动重连", ex);

                             consumer = GetSendTransactionListenerConsumer(currency);
                         }
                     }
                     catch (Exception ex)
                     {
                         Log.Error(currency.ToString() + "提现指令处理过程中出现错误:" + ex.Message);
                     }
                 }
             }));

            thread.Start();
        }

        private static bool TrySendTransactionToP2p(string msgId, string address, string publicKey, decimal amount, out string txid, out decimal txfee)
        {
            txid = string.Empty;
            txfee = 0;

            try
            {
                if (!Config.Debug)
                {
                    if (ExistSendTransactionMessage(msgId, out txid))
                    {
                        Log.Info("提现发送交易消息Id={0}已处理过，不再重复处理", msgId);
                    }
                    else
                    {
                        Cache.Add(msgId, txid);
                        var res = nxtClient.SendMoneyAsnc(address, (int)amount, publicKey).Result;

                        txid = res.Transaction;

                        var tx = default(GetTransactionResponse);

                        if (TryGetTransaction(txid, out tx))
                        {
                            txfee = tx.FeeNQT / 100000000;
                        }
                    }
                }
                else
                {
                    Log.Debug("DEBUG模式下，不会真的发送交易出去，以下是交易的数据:" + Environment.NewLine + "send " + amount + _currency + " to " + address);
                    txid = Guid.NewGuid().Shrink() + Guid.NewGuid().Shrink();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("提现发币错误:" + Environment.NewLine + "send " + amount + _currency + " to " + address, ex);

                return false;
            }
        }

        private static void SendCompleteCommand(CurrencyType currency, string withdrawUniqueID, string txid, decimal txfee)
        {
            try
            {
                var mqname = "NXTSendCompleteCommand";
                var channel = _mqpool.GetMQChannel(mqname);

                var exchangeAndQueueName = Utilities.GenerateVirtualCoinCompletePaymentExchangeAndQueueName();

                var cmd = new CompleteVirtualCoinWithdraw(withdrawUniqueID, txid, txfee, currency);

                var build = new BytesMessageBuilder(channel);
                build.WriteBytes(Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(cmd)));
                ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;
                try
                {
                    channel.BasicPublish(exchangeAndQueueName.Item1, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                }
                catch (EndOfStreamException ex)
                {
                    if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                    {
                        channel = _mqpool.GetMQChannel(mqname);
                        channel.BasicPublish(exchangeAndQueueName.Item1, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("提现处理完毕后，发送完成消息时出现错误:withdrawID={0},currency={1},txid={3}".FormatWith(withdrawUniqueID, currency, txid), ex);
                }
            }
            catch (Exception ex)
            {
                Log.Error("提现处理完毕后，发送完成消息时出现错误:withdrawID={0},currency={1},txid={3}".FormatWith(withdrawUniqueID, currency, txid), ex);
            }
        }

        private static void SendWithdrawFailOrAddressInvalidCommand(CurrencyType currency, string withdrawUniqueID, VirtualCoinWithdrawFailProcessType processType)
        {
            var cmdMessage = string.Empty;
            try
            {
                var mqname = currency.ToString() + "SendWithdrawFailOrAddressInvalidCommand";
                var channel = _mqpool.GetMQChannel(mqname);
                var exchangeAndQueueName = Utilities.GenerateVirtualCoinWithdrawTranferFailOrAddressInvalidExchangeAndQueueName();
                var nullUserID = 0;

                if (processType == VirtualCoinWithdrawFailProcessType.Cancel)
                {
                    Log.Info("{0}提现{1}的发送到p2p失败，发送处理失败指令", currency, withdrawUniqueID);
                    var cmd = new CancelVirtualCoinWithdraw(withdrawUniqueID, nullUserID, string.Empty, currency);
                    cmdMessage = IoC.Resolve<IJsonSerializer>().Serialize(cmd);
                }
                else
                {
                    Log.Info("{0}提现{1}的提现地址不合法，发送撤销提现指令", currency, withdrawUniqueID);
                    var cmd = new VirtualCoinWithdrawFail(withdrawUniqueID, nullUserID, string.Empty, currency);
                    cmdMessage = IoC.Resolve<IJsonSerializer>().Serialize(cmd);

                }


                var build = new BytesMessageBuilder(channel);
                build.WriteBytes(Encoding.UTF8.GetBytes(cmdMessage));
                ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;
                try
                {
                    channel.BasicPublish(exchangeAndQueueName.Item1, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                }
                catch (EndOfStreamException ex)
                {
                    if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                    {
                        channel = _mqpool.GetMQChannel(mqname);
                        channel.BasicPublish(exchangeAndQueueName.Item1, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("发送提现失败处理消息时--失败了:" + cmdMessage, ex);
                }

            }
            catch (Exception ex)
            {
                Log.Error("发送提现失败处理消息时--失败了:" + cmdMessage, ex);
            }
        }

        private static bool ExistSendTransactionMessage(string msgId, out string txid)
        {
            bool isExist = false;

            isExist = Cache.TryGet(msgId, out txid);

            return isExist;
        }

        #region try get transaction
        private static bool TryGetTransaction(string txid, out GetTransactionResponse tx)
        {
            tx = default(GetTransactionResponse);
            try
            {
                tx = nxtClient.GetTransactionAsync(txid).Result;
                return true;
            }
            catch (NXTCallException ex)
            {
                Log.Warn("TryGetTransaction访问出错:" + ex.Message);
                return false;
            }
        }
        #endregion


        private static QueueingBasicConsumer GetSendTransactionListenerConsumer(CurrencyType currency)
        {
            var mqname = currency.ToString() + "SendTransactionListener";
            var channel = _mqpool.GetMQChannel(mqname);
            var exchangeAndQueueName = Utilities.GenerateVirtualCoinSendPaymentExchangeAndQueueName(currency);

            channel.ExchangeDeclare(exchangeAndQueueName.Item1, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(exchangeAndQueueName.Item2, true, false, false, null);
            channel.QueueBind(exchangeAndQueueName.Item2, exchangeAndQueueName.Item1, string.Empty);
            channel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channel);
            //auto ack ,最大程度上避免重复发出币
            channel.BasicConsume(exchangeAndQueueName.Item2, true, consumer);

            return consumer;
        }
        #endregion

        #region 数据实体类
        private class VirtualCoinWithdrawProcessMessage
        {
            public VirtualCoinWithdrawProcessMessage(CurrencyType currency, string withdrawUniqueID, string msgID, decimal amount, string address, string nxtPublicKey = "")
            {
                this.Currency = currency;
                this.WithdrawUniqueID = withdrawUniqueID;
                this.MsgID = msgID;
                this.Amount = amount;
                this.Address = address;
                this.NXTPublicKey = nxtPublicKey;
            }

            public CurrencyType Currency { get; private set; }
            public string WithdrawUniqueID { get; private set; }
            public string MsgID { get; private set; }
            public decimal Amount { get; private set; }
            public string Address { get; private set; }
            public string NXTPublicKey { get; private set; }
        }
        #endregion
    }
}
