using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DFramework;
using DFramework.Utilities;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using Top.Api.Domain;
using ConfigurationManagerWrapper = DFramework.ConfigurationManagerWrapper;
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
                                tradesInDb.ForEach(t =>
                                {
                                    string buyer_message = string.Empty;

                                    if (CheckIfHasUnconfirmOrder(t.buyer_nick))
                                    {
                                        //如果用户上次充值未确认,则该笔充值提示用户，并等待用户确认上笔充值后，再次为用户充值
                                        var result = MarkTaobaoAutoDepositHasOneUnConfirmOrder(t.tid);

                                        if (result > 0)
                                        {
                                            Log.Info("发现用户{0}有未确认收货的订单，更新新订单的备注为'有未确认收货订单'", t.buyer_nick);
                                        }
                                    }
                                    else if (t.has_buyer_message)
                                    {
                                        var tradeTaobao = TaobaoUtils.GetTradeFullInfo(t.tid, session);

                                        Debug.Assert(Math.Round(Convert.ToDecimal(tradeTaobao.TotalFee)) == t.amount);

                                        buyer_message = tradeTaobao.BuyerMessage.Trim();
                                        var rippleAddress = buyer_message;

                                        if (rippleAddress.Length >= 32 && rippleAddress.StartsWith("r") &&
                                            UpdateRippleAddressAndRippleStatusOfTaobaoAutoDeposit(t.tid,
                                                rippleAddress) == 1)
                                        {
                                            //如果数据库更新为pending(代表已提交【请处理】消息到队列中,更新数据库后，将不在重复提交)
                                            //当然，如果在数据库更新成功后，消息有提交失败的可能。
                                            //如果消息提交了，更新下数据库字段即可再次提交
                                            //!=消息提交后，除了Lose Tx Validator更新状态，否则不会在提交=!
                                            PublishMessage(
                                                new TaobaoDepositMessage(t.tid, tradeTaobao.BuyerMessage, t.amount),
                                                RippleSendIOUExchangeName, RippleSendIOUTaobaoDepositRouteKey);
                                            NoticeWebMaster("发现淘宝自动充值，已提交处理",
                                                "淘宝交易号={0}，金额={1},地址={2}".FormatWith(t.tid, t.amount,
                                                    tradeTaobao.BuyerMessage));
                                            Log.Info("tid={0} 提交成功..mq", t.tid);
                                        }
                                        else
                                        {
                                            Log.Info("tid={0} 的未留言或留言不正确,标记为失败", t.tid);
                                            MarkTaobaoAutoDepositMissBuyerMessage(t.tid, buyer_message);
                                            NoticeWebMaster("发现淘宝自动充值，用户未正确留言", "淘宝交易号={0}，金额={1},留言={2}".FormatWith(t.tid, t.amount, buyer_message));
                                        }
                                    }
                                    else
                                    {
                                        Log.Info("tid={0} 的未留言或留言不正确,标记为失败", t.tid);
                                        MarkTaobaoAutoDepositMissBuyerMessage(t.tid);
                                        NoticeWebMaster("发现淘宝自动充值，用户未正确留言", "淘宝交易号={0}，金额={1},留言={2}".FormatWith(t.tid, t.amount, buyer_message));
                                    }


                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Deposit Dispatcher Exception", ex);
                        }
                    }

                    Task.Delay(10 * 1000).Wait();
                }
            });

            thread.Start();
            Log.Info("-->自动充值提交器启动成功...");
            started = true;
        }

        private static IEnumerable<TaobaoAutoDeposit> ReadTaobaoTradeWhichUnprocess()
        {
            const string sql =
                "SELECT tid,amount,buyer_nick,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo,first_submit_at" +
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
        //检查用户是否有已经充值成功，但用户仍未确认收货的订单
        private static bool CheckIfHasUnconfirmOrder(string buyerNick)
        {
            const string sql =
                "SELECT COUNT(*)" +
                "  FROM taobao " +
                " WHERE buyer_nick=@buyer_nick AND taobao_status=@taobao_status AND ripple_status=@ripple_status";
            try
            {
                using (var conn = OpenConnection())
                {
                    var count = conn.ExecuteScalar(sql, new { buyer_nick = buyerNick, taobao_status = "WAIT_BUYER_CONFIRM_GOODS", ripple_status = RippleTransactionStatus.Successed });

                    return Convert.ToInt32(count) > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error("CheckIfHasUnconfirmOrder Exception", ex);
                return false;
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


        private static int MarkTaobaoAutoDepositMissBuyerMessage(long tid, string errmsg = "")
        {
            const string sql =
                "UPDATE taobao SET ripple_status=@ripple_status_new,memo=@memo,ripple_address=@ripple_address " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status_old";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new { memo = "留言错误", ripple_address = errmsg, ripple_status_new = RippleTransactionStatus.Failed, taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status_old = RippleTransactionStatus.Init });
                }
            }
            catch (Exception ex)
            {
                Log.Error("MarkTaobaoAutoDepositMissBuyerMessage Exception", ex);
                return 0;
            }
        }
        private static int MarkTaobaoAutoDepositHasOneUnConfirmOrder(long tid)
        {
            const string sql =
                "UPDATE taobao SET memo=@memo " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status AND memo<>'有未确认收货订单'";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new { memo = "有未确认收货订单", taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status = RippleTransactionStatus.Init });
                }
            }
            catch (Exception ex)
            {
                Log.Error("MarkTaobaoAutoDepositHasOneUnConfirmOrder Exception", ex);
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

        private static void NoticeWebMaster(string title, string message)
        {
            var mails = ConfigurationManagerWrapper.AppSettings["noticeMails"];
            var mailList = mails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (mailList != null && mailList.Any())
                mailList.ForEach(m =>
                {
                    EmailHelper.SendMailAsync(m, title, message);
                });
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
