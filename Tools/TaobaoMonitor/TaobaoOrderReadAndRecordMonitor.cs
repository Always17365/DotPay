﻿using System;
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

                            if (trades != null)
                            {
                                trades = trades.Where(t => t.Orders.First().Title.Contains("官方充值")).ToList();

                                if (trades.Any())
                                {
                                    RecordTaobaoTradeToDatabase(trades);
                                }
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
            const string selectSql = "SELECT COUNT(*) FROM taobao WHERE tid=@tid";
            const string insertSql = "INSERT INTO taobao(tid,buyer_nick,pay_time,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo) " +
                                     "VALUES(@tid,@buyer_nick,@pay_time,@amount,@has_buyer_message,@taobao_status,@ripple_address,@ripple_status,@txid,@memo)";
            try
            {
                using (var conn = OpenConnection())
                {
                    var transaction = conn.BeginTransaction();
                    foreach (var trade in trades)
                    {
                        var count = conn.Query<int>(selectSql, new { tid = trade.Tid });

                        if (count.First() == 0)
                        {
                            Log.Info("单号{0}已付款,开始写入数据库...", trade.Tid);
                            conn.Execute(insertSql,
                                new
                                {
                                    tid = trade.Tid,
                                    buyer_nick = trade.BuyerNick.NullSafe(),
                                    pay_time = trade.PayTime,
                                    amount = Math.Round(Convert.ToDecimal(trade.TotalFee)),
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
