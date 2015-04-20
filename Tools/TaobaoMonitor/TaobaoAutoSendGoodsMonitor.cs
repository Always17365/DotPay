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

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoAutoSendGoodsMonitor
    {
        private static bool started;
        private static readonly string MysqlConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("taobaodb");
        private static readonly string RabbitMqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectString");
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
                    var sessionList = TaobaoUtils.GetTaobaoSessionList();

                    if (sessionList.Any())
                    {
                        sessionList.ForEach(s =>
                        {
                            try
                            {
                                var tradesInDb = ReadTaobaoTradeWhichNeedSendGoods(s.NickName);

                                if (tradesInDb.Any())
                                {
                                    Log.Info("读取{0}到{1}待发货的淘宝自动充值,开始自动发货..", s.NickName, tradesInDb.Count());
                                    tradesInDb.ForEach(t =>
                                    {
                                        if (TaobaoUtils.SendGoods(t.tid, s.Session))
                                        {
                                            MarkTaobaoAutoDepositHasSendGoods(t.tid);
                                        }
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Taobao Auto Send Goods Exception", ex);
                            }
                        });
                    }
                    Task.Delay(10 * 1000).Wait();
                }
            });

            thread.Start();
            Log.Info("-->自动发货处理器启动成功...");
            started = true;
        }

        private static IEnumerable<TaobaoAutoDeposit> ReadTaobaoTradeWhichNeedSendGoods(string sellerNick)
        {
            const string sql =
                "SELECT tid,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo" +
                "  FROM taobao " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status AND seller_nick=@seller_nick";
            try
            {
                using (var conn = OpenConnection())
                {
                    var tradesInDb = conn.Query<TaobaoAutoDeposit>(sql, new
                    {
                        taobao_status = "WAIT_SELLER_SEND_GOODS",
                        ripple_status = RippleTransactionStatus.Successed,
                        seller_nick = @sellerNick
                    });

                    return tradesInDb;
                }
            }
            catch (Exception ex)
            {
                Log.Error("ReadTaobaoTradeWhichNeedSendGoods Exception", ex);
                return null;
            }
        }

        private static int MarkTaobaoAutoDepositHasSendGoods(long tid)
        {
            const string sql =
                "UPDATE taobao SET taobao_status=@taobao_status_new " +
                " WHERE tid=@tid AND taobao_status=@taobao_status AND ripple_status=@ripple_status";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new
                    {
                        tid = tid,
                        taobao_status_new = "WAIT_BUYER_CONFIRM_GOODS",
                        ripple_status = RippleTransactionStatus.Successed,
                        taobao_status = "WAIT_SELLER_SEND_GOODS"
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("MarkTaobaoAutoDepositHasSendGoods Exception", ex);
                return 0;
            }
        }
    }
}
