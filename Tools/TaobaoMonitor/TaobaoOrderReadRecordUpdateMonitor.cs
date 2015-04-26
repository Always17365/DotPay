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
using DFramework.Utilities;
using ConfigurationManagerWrapper = DFramework.ConfigurationManagerWrapper;
using Task = System.Threading.Tasks.Task;

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoOrderReadRecordUpdateMonitor
    {
        private static bool _started;
        private static readonly string MysqlconnectionString = ConfigurationManagerWrapper.GetDBConnectionString("taobaodb");
        private static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(MysqlconnectionString);
            connection.Open();
            return connection;
        }

        public static void Start()
        {
            if (_started) return;

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
                                var trades = TaobaoUtils.GetIncrementTaobaoTrade(s.NickName,s.Session);

                                if (trades != null)
                                {
                                    trades = trades.Where(t => (t.Orders.First().Title.Contains("官方充值") || t.Orders.First().Title.Contains("CNY.DotPayco")) &&
                                                               (t.Status == "TRADE_NO_CREATE_PAY" ||
                                                                t.Status == "WAIT_BUYER_PAY" ||
                                                                t.Status == "WAIT_SELLER_SEND_GOODS" ||
                                                                t.Status == "WAIT_BUYER_CONFIRM_GOODS" ||
                                                                t.Status == "TRADE_FINISHED" ||
                                                                t.Status == "TRADE_CLOSED" ||
                                                                t.Status == "TRADE_CLOSED_BY_TAOBAO"
                                                                )
                                        ).ToList();

                                    if (trades.Any())
                                    {
                                        RecordTaobaoTradeToDatabase(trades, s.NickName, s.Session);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("GetCompletePaymentTrade Exception", ex);
                            }
                        });

                    }
                    Task.Delay(60 * 1000).Wait();
                }
            });

            thread.Start();
            Log.Info("-->淘宝交易记录读取-记录器启动成功...");
            _started = true;
        }

        private static void RecordTaobaoTradeToDatabase(List<Trade> trades, string sellerNick, string session)
        {
            const string existSql = "SELECT COUNT(*) FROM taobao WHERE tid=@tid";
            const string selectSql = "SELECT COUNT(*) FROM taobao WHERE tid=@tid AND taobao_status=@taobao_status";
            const string insertSql = "INSERT INTO taobao(tid,seller_nick,buyer_nick,pay_time,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo) " +
                                     "VALUES(@tid,@seller_nick,@buyer_nick,@pay_time,@amount,@has_buyer_message,@taobao_status,@ripple_address,@ripple_status,@txid,@memo)";

            const string updateStatusSql = "UPDATE taobao SET taobao_status=@taobao_status_new " +
                                           " WHERE tid=@tid AND taobao_status=@taobao_status_old AND ripple_status=@ripple_status";

            const string updateForCloseStatusSql = "UPDATE taobao SET taobao_status=@taobao_status_new,memo=''" +
                                          " WHERE tid=@tid AND taobao_status=@taobao_status_old AND ripple_status=@ripple_status";

            try
            {
                using (var conn = OpenConnection())
                {
                    var transaction = conn.BeginTransaction();
                    foreach (var trade in trades)
                    {
                        #region 如果是未付款的单子，检查留言是否正确，不正确的直接关闭交易

                        if (trade.Status == "TRADE_NO_CREATE_PAY" || trade.Status == "WAIT_BUYER_PAY")
                        {
                            var rippleAddress = string.Empty;

                            var needClose = false;

                            if (!trade.HasBuyerMessage) needClose = true;
                            else
                            {
                                var tradeFullInfo = TaobaoUtils.GetTradeFullInfo(trade.Tid, session);
                                rippleAddress = tradeFullInfo.BuyerMessage.Trim();
                                if (rippleAddress.Length < 32 || !rippleAddress.StartsWith("r"))
                                    needClose = true;
                            }

                            if (needClose)
                            {
                                if (TaobaoUtils.CloseOrder(trade.Tid, session))
                                {
                                    Log.Info("发现留言错误的交易，卖家" + sellerNick + "主动关闭该交易,订单号=" + trade.Tid);
                                    TaobaoUtils.NoticeWebMaster("淘宝交易留言错误，关闭交易",
                                        "发现未付款订单{0}留言错误,留言={1}，卖家{2}已主动关闭该交易".FormatWith(trade.Tid, rippleAddress, sellerNick));
                                }
                            }
                        }

                        #endregion

                        #region 如果是刚刚付款的订单
                        else if (trade.Status == "WAIT_SELLER_SEND_GOODS")
                        {
                            var count = conn.Query<int>(existSql, new { tid = trade.Tid });
                            if (count.First() == 0)
                            {
                                Log.Info("单号{0}已付款,开始写入数据库...", trade.Tid);
                                var realAmount = Math.Round(Convert.ToDecimal(trade.Orders[0].Price)) *
                                                 trade.Orders[0].Num;
                                conn.Execute(insertSql,
                                    new
                                    {
                                        tid = trade.Tid,
                                        seller_nick = sellerNick,
                                        buyer_nick = trade.BuyerNick.NullSafe(),
                                        pay_time = trade.PayTime,
                                        amount = realAmount,
                                        has_buyer_message = trade.HasBuyerMessage,
                                        taobao_status = trade.Status,
                                        ripple_address = string.Empty,
                                        ripple_status = RippleTransactionStatus.Init,
                                        txid = string.Empty,
                                        memo = string.Empty
                                    }, transaction);

                                Log.Info("单号{0},写入数据库完毕.", trade.Tid);
                            }
                        }
                        #endregion
                        //当订单已经发货 这个时候等待用户收货，无需做任何处理
                        //-----------------------------------------
                        //用户已确认收货，应该更新订单的状态
                        else if (trade.Status == "TRADE_FINISHED")
                        {
                            var count = conn.Query<int>(selectSql, new { tid = trade.Tid, taobao_status = "WAIT_BUYER_CONFIRM_GOODS" });
                            if (count.First() == 1)
                            {
                                Log.Info("单号{0}已确认收货,更新数据库状态...", trade.Tid);
                                conn.Execute(updateStatusSql,
                                    new
                                    {
                                        tid = trade.Tid,
                                        taobao_status_new = "TRADE_FINISHED",
                                        taobao_status_old = "WAIT_BUYER_CONFIRM_GOODS",
                                        ripple_status = RippleTransactionStatus.Successed,
                                    }, transaction);
                                Log.Info("单号{0},更新为已确认收货.", trade.Tid);
                            }
                        }
                        //用户已确认收货，应该更新订单的状态
                        else if (trade.Status == "TRADE_CLOSED" || trade.Status == "TRADE_CLOSED_BY_TAOBAO")
                        {
                            var count = conn.Query<int>(selectSql, new { tid = trade.Tid, taobao_status = "WAIT_SELLER_SEND_GOODS" });
                            if (count.First() == 1)
                            {
                                Log.Info("单号{0}已关闭交易,更新数据库状态...", trade.Tid);
                                conn.Execute(updateForCloseStatusSql,
                                    new
                                    {
                                        tid = trade.Tid,
                                        taobao_status_new = "TRADE_CLOSED",
                                        taobao_status_old = "WAIT_SELLER_SEND_GOODS",
                                        ripple_status = RippleTransactionStatus.Failed,
                                    }, transaction);
                                Log.Info("单号{0},更新为已关闭交易.", trade.Tid);
                            }
                        }
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
