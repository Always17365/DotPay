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
using FluentData;
using RabbitMQ.Client.Content;
using System.Security.AccessControl;

namespace DotPay.RippleMonitor
{
    internal class InboundTransferWatcher
    {
        private const string logTxtName = "ProcessLedgerRecord.txt", logProcessIndexTxtName = "ProcessLedgerIndex";
        public static bool Started { get; private set; }
        private static IModel _channel;
        private static CurrencyType _currency;
        private static IDbContext _dbContext;
        private static MQConnectionPool _mqpool;
        private static int currentProcessLedgerIndex;
        private static object _lock = new object();
        private static bool waitCallback = false;
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        internal static void Start(MQConnectionPool mqpool)
        {
            if (InboundTransferWatcher.Started)
            {
                Log.Info("Ripple转入交易监听器已启动");
                return;
            }

            currentProcessLedgerIndex = GetLastProcessLedgerIndex();

            _mqpool = mqpool;

            _dbContext = new DbContext().ConnectionString(Config.DBConnectString, new MySqlProvider());

            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (!InboundTransferWatcher.Started)
                    {
                        Log.Info("Ripple转入交易监听器启动成功,监听交易中...");
                        InboundTransferWatcher.Started = true;
                    }
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，Ripple转入交易监听器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }

                    if (!waitCallback)
                    {
                        lock (_lock)
                        {
                            waitCallback = true;
                            GetTxs(InboundTransferWatcher.currentProcessLedgerIndex + 1, ProcessTxs);
                        }
                    }
                    else
                        Thread.Sleep(1 * 1000);
                }
            }));

            thread.Start();
        }

        #region 私有方法
        private static void GetTxs(int ledgerIndex, Action<List<TransactionRecord>> processTxAction)
        {
            try
            {
                var rippleClient = IoC.Resolve<IRippleClient>();

                Log.Info("开始分析ledger-{0}".FormatWith(ledgerIndex));

                rippleClient.GetTransactions(Config.RippleAccount, ledgerIndex, (error, result) =>
                {
                    try
                    {
                        if (error != null)
                        {
                            if (error.Error == "lgrNotFound" && error.ErrorCode == 17)
                            {
                                Log.Info("已分析到最新的ledger，等待10秒后，继续分析...");
                                Thread.Sleep(10 * 1000);
                            }
                            else
                                Log.Error("查找Ripple钱包历史交易记录时出现错误：{0}", error.Message);
                        }
                        else
                        {
                            if (result.Count > 0)
                            {
                                processTxAction(result);
                            }
                            RecordProcessLedgerIndex(ledgerIndex);
                            Log.Info("ledger-{0}解析完毕".FormatWith(ledgerIndex));
                        }
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            waitCallback = false;
                        }
                    }
                });
            }
            catch (Exception ex2)
            {
                Log.Error("listtransactions RPC访问出错:" + ex2.Message);
            }

        }

        private static void RecordProcessLedgerIndex(int ledgerIndex)
        {
            try
            {
                currentProcessLedgerIndex = ledgerIndex;
                File.AppendAllText(logTxtName, ledgerIndex.ToString() + ",");
                File.WriteAllText(logProcessIndexTxtName, ledgerIndex.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Ripple监听交易记录-记录处理索引，写入文件出错", ex);
            }
        }

        private static int GetLastProcessLedgerIndex()
        {
            var ledgerIndex = 10108727;
            try
            {
                var ledgerIndexStr = File.ReadAllText(logProcessIndexTxtName);

                int.TryParse(ledgerIndexStr, out ledgerIndex);
            }
            catch (Exception ex)
            {
                Log.Error("Ripple监听交易记录-启动时，读取处理索引记录出错", ex);
            }

            return ledgerIndex;
        }
        private static void ProcessTxs(IEnumerable<TransactionRecord> txs)
        {
            txs.ForEach(tx =>
            {
                if (tx.Validated && tx.TransactionDetail.TransactionType == TransactionType.Payment && tx.TransactionDetail.Destination == Config.RippleAccount)
                {
                    var amount = tx.Meta.DeliveredAmount ?? tx.TransactionDetail.Amount;

                    if (amount.Issuer == Config.RippleAccount && amount.Currency == "CNY")
                    {
                        Log.Info("发现新的转入交易...." + Environment.NewLine + IoC.Resolve<IJsonSerializer>().Serialize(tx));

                        var cmd = new CreateInboundTx(tx.TransactionDetail.Hash, tx.TransactionDetail.DestinationTag, amount.Value);

                        try
                        {
                            IoC.Resolve<ICommandBus>().Send(cmd);
                        }
                        catch (NHibernate.Exceptions.GenericADOException ex)
                        {
                            var mysqlex = ex.InnerException as MySql.Data.MySqlClient.MySqlException;
                            if (mysqlex != null && mysqlex.Number == 1062)
                            {
                                Log.Info("已接收该交易，重复的到帐消息" + IoC.Resolve<IJsonSerializer>().Serialize(cmd) + "，做丢弃处理");
                            }
                        }
                        catch (CommandExecutionException ex)
                        {
                            Log.Error("创建新的转入交易指令时出现错误", ex);
                        }
                    }
                }
            });
        }
        #endregion
    }
}
