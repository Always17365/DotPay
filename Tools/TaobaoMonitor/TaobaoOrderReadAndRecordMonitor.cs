using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DFramework;
using MySql.Data.MySqlClient;
using Top.Api.Domain;
using Dapper;
using Task = System.Threading.Tasks.Task;

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoOrderReadAndRecordMonitor
    {
        private static bool started;
        private static readonly string mysqlconnectionString = ConfigurationManagerWrapper.GetDBConnectionString("taobaodb");
        private static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(mysqlconnectionString);
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
                            var trades = TaobaoUtils.GetCompletePaymentTrade(session);

                            if (trades != null && trades.Any())
                            {
                                Log.Info("读到{0}条淘宝已付款交易,开始写入数据库..." + trades.Count);
                                RecordTaobaoTradeToDatabase(trades);
                                Log.Info("淘宝交易写入数据库完毕." + trades.Count);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("GetCompletePaymentTrade Exception", ex);
                        }
                    }

                    Task.Delay(60 * 1000).Wait();
                }
            });

            thread.Start();
            Log.Info("-->淘宝交易记录读取-记录器启动成功...");
            started = true;
        }

        private static void RecordTaobaoTradeToDatabase(List<Trade> trades)
        {
            const string insertSql = "INSERT INTO taobao(tid,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo) " +
                                     "VALUES(@tid,@amount,@has_buyer_message,@taobao_status,@ripple_address,@ripple_status,@txid,@memo)";
            try
            {
                using (var conn = OpenConnection())
                {
                    var transaction = conn.BeginTransaction();
                    foreach (var trade in trades)
                    {
                        conn.Execute(insertSql,
                            new
                            {
                                tid = trade.Tid,
                                amount = Math.Round(Convert.ToDecimal(trade.TotalFee)),
                                has_buyer_message = trade.HasBuyerMessage,
                                taobao_status = trade.Status,
                                ripple_address = string.Empty,
                                ripple_status = RippleTransactionStatus.Init,
                                txid = string.Empty,
                                memo = string.Empty
                            }, transaction);
                    }
                    transaction.Commit();
                }
            }

            catch (MySqlException ex)
            {
                if (ex.Number != 1062)
                    Log.Error("RecordTaobaoTradeToDatabase MYSQL Exception", ex);
            }
            catch (Exception ex)
            {
                Log.Error("RecordTaobaoTradeToDatabase Exception", ex);
            }
        }
    }
}
