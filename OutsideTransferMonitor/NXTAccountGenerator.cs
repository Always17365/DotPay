using FC.Framework;
using DotPay.Command;
using DotPay.Common;
using DotPay.Tools.NXTClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotPay.Tools.NXTMonitor
{
    internal class NXTAccountGenerator
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static CurrencyType _currency;
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private static NXTClient4Net nxtClient;

        internal static void Start(MQConnectionPool mqpool)
        {
            if (NXTAccountGenerator.Started)
            {
                Log.Info("NXT充值地址生成器已启动");
                return;
            }

            nxtClient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase);
            _mqpool = mqpool;
            var currencies = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            StartGenerateNewAddressConsumer(_currency);
        }

        #region 私有方法

        private static void StartGenerateNewAddressConsumer(CurrencyType currency)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                IModel channel;
                var consumer = GetAddressGeneratorConsumer(currency, out channel);

                Log.Info("启动{0}充值地址生成器成功！", currency);

                while (true)
                {
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，虚拟币充值地址生成器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }
                    Log.Info("正在监听{0}新的地址生成指令...", currency);
                    BasicDeliverEventArgs ea = null;
                    try
                    {
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        var generateMessage = IoC.Resolve<IJsonSerializer>().Deserialize<GeneratePaymentAddress>(message);

                        Log.Info("收到{0}新的地址生成指令:{1}", currency, message);

                        if (Config.CoinCode != generateMessage.Currency.ToString())
                        {
                            Log.Fatal("收到的地址生成指令中指定的币种与配置文件中的币种不一致！");
                            break;
                        }

                        var address = string.Empty;

                        if (TryGenerateNewAddress(Config.SecretPhrase + generateMessage.UserID.ToString(), out address))
                        {
                            SendCreatePaymentAddressCommand(generateMessage.Currency, generateMessage.UserID, address);

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        else
                        {
                            Log.Error("生成地址请求失败了!generateMessage:" + message);
                        }

                        Log.Info("成功处理{0}地址生成指令", currency);
                    }
                    catch (System.IO.EndOfStreamException ex)
                    {
                        if (ex.Message.Equals("SharedQueue closed", StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Warn("生成" + currency.ToString() + "地址，消息队列的链接断开了,准备自动重连", ex);

                            consumer = GetAddressGeneratorConsumer(currency, out channel);
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
                        Log.Error("{0}新的收款地址生成后，发送完成消息时出现错误".FormatWith(currency), ex);
                    }
                }
            }));

            thread.Start();
        }

        private static bool TryGenerateNewAddress(string secretPhrase, out string address)
        {
            address = string.Empty;

            try
            {
                var nxtAccountRes = nxtClient.GetAccountIDAsync(secretPhrase).Result;
                address = nxtAccountRes.AccountId + "," + nxtAccountRes.AccountRS+","+nxtAccountRes.PublicKey;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void SendCreatePaymentAddressCommand(CurrencyType currency, int userID, string address)
        {
            var mqname = "NXTSendCreatePaymentAddressCommand";
            var channel = _mqpool.GetMQChannel("NXTSendCreatePaymentAddressCommand");
            var exchangeAndQueueName = Utilities.GenerateExchangeAndQueueNameOfCreatePaymentAddress(currency);
            var cmd = new CreatePaymentAddress(userID, address, currency);
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
        }

        private static QueueingBasicConsumer GetAddressGeneratorConsumer(CurrencyType currency, out IModel channel)
        {
            var mqname = "NXTAccountGenerator";
            channel = _mqpool.GetMQChannel(mqname);

            var exchangeAndQueueName = Utilities.GenerateExchangeAndQueueNameOfGenerateNewAddress(currency);

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
        private class VirtualCoinWithdrawProcessMessage
        {
            public VirtualCoinWithdrawProcessMessage(CurrencyType currency, int withdrawID, string msgID, decimal amount, string address)
            {
                this.Currency = currency;
                this.WithdrawID = withdrawID;
                this.MsgID = msgID;
                this.Amount = amount;
                this.Address = address;
            }

            public CurrencyType Currency { get; private set; }
            public int WithdrawID { get; private set; }
            public string MsgID { get; private set; }
            public decimal Amount { get; private set; }
            public string Address { get; private set; }
        }
        #endregion
    }
}
