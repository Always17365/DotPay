using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DFramework;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using Top.Api.Domain;
using Task = System.Threading.Tasks.Task;

namespace Dotpay.TaobaoMonitor
{
    internal class DepositDispatcher
    {
        private static bool started;
        private static readonly string MysqlConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("taobaodb");
        private static readonly string RabbitMqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectString");
        private static IModel _channel;
        private const string RippleSendIOUExchangeName = "__RippleSendIOU_Exchange";
        private const string RippleSendIOUTaobaoDepositRouteKey = "Taobao";
        private const string RippleSendIOUDirectDepositQueue = "__RippleSendIOU_Taobao_Direct_Deposit";
        private static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(MysqlConnectionString);
            connection.Open();
            return connection;
        }

        public static void Start()
        {
            if (started) return;

            var thread = new Thread(() =>
            {
                while (true)
                {
                    var session = TaobaoUtils.GetTaobaoSession();

                    if (string.IsNullOrWhiteSpace(session)) TaobaoUtils.NoticeWebMaster();

                    else
                    {
                        try
                        {
                            var tradesInDb = ReadTaobaoTradeWhichUnprocess();

                            if (tradesInDb.Any())
                            {
                                Log.Info("读取到{0}待提交的淘宝自动充值,开始提交到自动充值处理器..", tradesInDb.Count());
                                tradesInDb.ForEach(t =>
                                {
                                    if (t.has_buyer_message)
                                    {
                                        var tradeTaobao = TaobaoUtils.GetTradeFullInfo(t.tid, session);

                                        Debug.Assert(Math.Round(Convert.ToDecimal(tradeTaobao.TotalFee)) == t.amount);

                                        if (UpdateRippleAddressAndRippleStatusOfTaobaoAutoDeposit(t.tid,
                                            tradeTaobao.BuyerMessage.Trim()) == 1)
                                        {
                                            //如果数据库更新为pending(代表已提交【请处理】消息到队列中,更新数据库后，将不在重复提交)
                                            //当然，如果在数据库更新成功后，消息有提交失败的可能。
                                            //如果消息提交了，更新下数据库字段即可再次提交
                                            //!=消息提交后，除了Lose Tx Validator更新状态，否则不会在提交=!
                                            PublishMessage(
                                                new TaobaoDepositMessage(t.tid, tradeTaobao.BuyerMessage, t.amount),
                                                RippleSendIOUExchangeName, RippleSendIOUTaobaoDepositRouteKey);

                                            Log.Info("tid={0} 提交成功..mq", t.tid);
                                        }
                                    }
                                    else
                                    {
                                        Log.Info("tid={0} 的未留言,标记为失败", t.tid);
                                        MarkTaobaoAutoDepositMissBuyerMessage(t.tid);
                                    }
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("GetCompletePaymentTrade Exception", ex);
                        }
                    }

                    Task.Delay(10 * 1000).Wait();
                }
            });

            thread.Start();
            Log.Info("-->自动充值提交其启动成功...");
            started = true;
        }

        private static IEnumerable<TaobaoAutoDeposit> ReadTaobaoTradeWhichUnprocess()
        {
            const string sql =
                "SELECT tid,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo,first_submit_at" +
                "  FROM taobao " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status";
            try
            {
                using (var conn = OpenConnection())
                {
                    var tradesInDb = conn.Query<TaobaoAutoDeposit>(sql, new { taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status = RippleTransactionStatus.Init });

                    return tradesInDb;
                }
            }
            catch (Exception ex)
            {
                Log.Error("RecordTaobaoTradeToDatabase Exception", ex);
                return null;
            }
        }

        private static int UpdateRippleAddressAndRippleStatusOfTaobaoAutoDeposit(long tid, string rippleAddress)
        {
            const string sql =
                "UPDATE taobao SET ripple_address=@ripple_address,ripple_status=@ripple_status_new " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status_old";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new { ripple_address = rippleAddress, ripple_status_new = RippleTransactionStatus.Pending, taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status_old = RippleTransactionStatus.Init });
                }
            }
            catch (Exception ex)
            {
                Log.Error("RecordTaobaoTradeToDatabase Exception", ex);
                return 0;
            }
        }


        private static int MarkTaobaoAutoDepositMissBuyerMessage(long tid)
        {
            const string sql =
                "UPDATE taobao SET ripple_status=@ripple_status_new,memo=@memo " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status_old";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new { memo = "留言错误", ripple_status_new = RippleTransactionStatus.Failed, taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status_old = RippleTransactionStatus.Init });
                }
            }
            catch (Exception ex)
            {
                Log.Error("MarkTaobaoAutoDepositMissBuyerMessage Exception", ex);
                return 0;
            }
        }

        private static IModel GetMessageProducter()
        {
            if (_channel == null)
            {
                var factory = new ConnectionFactory { Uri = RabbitMqConnectionString, AutomaticRecoveryEnabled = true };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
                _channel.ExchangeDeclare(RippleSendIOUExchangeName, ExchangeType.Direct, true, false, null);
                _channel.QueueDeclare(RippleSendIOUDirectDepositQueue, true, false, false, null);
                _channel.QueueBind(RippleSendIOUDirectDepositQueue, RippleSendIOUExchangeName, RippleSendIOUTaobaoDepositRouteKey);
            }
            return _channel;
        }

        private static void PublishMessage(TaobaoDepositMessage message, string exchange, string routeKey)
        {
            var channel = GetMessageProducter();
            var messageBody = IoC.Resolve<IJsonSerializer>().Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(messageBody);
            var build = new BytesMessageBuilder(channel);
            build.WriteBytes(bytes);


            ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

            channel.BasicPublish(exchange, routeKey, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
        }

        [Serializable]
        internal class TaobaoDepositMessage
        {
            public TaobaoDepositMessage(long tid, string rippleAddress, int amount)
            {
                this.Tid = tid;
                this.RippleAddress = rippleAddress;
                this.Amount = amount;
            }

            public long Tid { get; set; }
            public string RippleAddress { get; set; }
            public int Amount { get; set; }
        }
    }
}
