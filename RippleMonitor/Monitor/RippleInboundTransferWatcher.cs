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
using FluentData;
using RabbitMQ.Client.Content;
using System.Security.AccessControl;

namespace DotPay.RippleMonitor
{
    internal class RippleInboundTransferWatcher
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
            if (RippleInboundTransferWatcher.Started)
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
                    if (!RippleInboundTransferWatcher.Started)
                    {
                        Log.Info("Ripple转入交易监听器启动成功,监听交易中...");
                        RippleInboundTransferWatcher.Started = true;
                    }
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，Ripple转入交易监听器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }

                    if (!waitCallback)
                    {
                        var rippleClient = IoC.Resolve<IRippleClientAsync>();
                        var aviableLedgerIndexResult = rippleClient.GetClosedLedgerIndex().Result;
                        if (aviableLedgerIndexResult.Item1 == null)
                        {
                            var ledger_min = RippleInboundTransferWatcher.currentProcessLedgerIndex;
                            var ledgerIndex_max = ledger_min + Config.Step - 1; //因为ripple走的是一个 ≥和≤的区间，所以+99就是一次分析100个ledger
                            ledgerIndex_max = Math.Min(ledgerIndex_max, aviableLedgerIndexResult.Item2 - 12);//取小值

                            if (ledgerIndex_max < ledger_min)
                            {
                                Log.Warn("目前已到达最新的ledger-" + RippleInboundTransferWatcher.currentProcessLedgerIndex + ",等待ledger close后再进行解析");
                            }
                            else
                            {
                                lock (_lock)
                                {
                                    waitCallback = true;

                                    GetTxs(ledger_min, ledgerIndex_max, ProcessTxs);
                                }
                            }
                        }
                    }
                    else
                        Thread.Sleep(3 * 1000);
                }
            }));

            thread.Start();
        }

        #region 私有方法
        private static async void GetTxs(int ledgerIndex_min, int lederIndex_max, Action<List<TransactionRecord>> processTxAction)
        {
            try
            {
                var rippleClient = IoC.Resolve<IRippleClientAsync>();

                Log.Info("开始分析ledger{0}-{1}".FormatWith(ledgerIndex_min, lederIndex_max));

                var result = await rippleClient.GetTransactions(Config.RippleAccount, ledgerIndex_min, lederIndex_max);

                try
                {
                    var error = result.Item1;
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
                        if (result.Item2.Count > 0)
                        {
                            processTxAction(result.Item2);
                        }
                        RecordProcessLedgerIndex(lederIndex_max + 1);
                        Log.Info("ledger{0}-{1}解析完毕".FormatWith(ledgerIndex_min, lederIndex_max));
                    }
                }
                finally
                {
                    lock (_lock)
                    {
                        waitCallback = false;
                    }
                }
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

                    if ((amount.Issuer == Config.RippleGateway || amount.Issuer == Config.RippleAccount) && amount.Currency == "CNY")
                    {
                        Log.Info("发现新的转入交易...." + Environment.NewLine + IoC.Resolve<IJsonSerializer>().Serialize(tx));

                        //解析destinationtag，检查标志位
                        var _destinationtag = tx.TransactionDetail.DestinationTag;

                        if (_destinationtag > 0 && _destinationtag < int.MaxValue)
                        {
                            var destinationtag = Convert.ToInt32(_destinationtag);
                            var flg = Convert.ToInt32(destinationtag.ToString().Substring(0, 2));
                            var tagFlg = Utilities.ConvertDestinationTagFlg(flg);
                            destinationtag = Convert.ToInt32(destinationtag.ToString().Substring(2));//截取标志位后才是真正的destinationtag
                            try
                            {
                                if (tagFlg == DestinationTagFlg.Dotpay)
                                {
                                    var cmd = new CreateInboundTx(tx.TransactionDetail.Hash, destinationtag, amount.Value);
                                    IoC.Resolve<ICommandBus>().Send(cmd);
                                }

                                else if (tagFlg == DestinationTagFlg.Alipay || tagFlg == DestinationTagFlg.Tenpay)
                                {
                                    try
                                    {
                                        var cmd = new CompleteThirdPartyPaymentInboundTx(tx.TransactionDetail.Hash, destinationtag, amount.Value);
                                        IoC.Resolve<ICommandBus>().Send(cmd);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("在执行tpp pay 的complete命令时出现异常", ex);
                                    }
                                }
                                else if (tagFlg == DestinationTagFlg.AlipayRippleForm || tagFlg == DestinationTagFlg.TenpayRippleForm || tagFlg == DestinationTagFlg.BankRippleForm)
                                {
                                    try
                                    {
                                        var cmd = new CompleteThirdPartyPaymentInboundTx(tx.TransactionDetail.Hash, destinationtag, amount.Value, tx.TransactionDetail.InvoiceID);
                                        IoC.Resolve<ICommandBus>().Send(cmd);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error("在执行tpp pay 的complete命令时出现异常", ex);
                                    }
                                }
                                else
                                {
                                    Log.Warn("发现未知的转入交易标志位" + flg.ToString() + " tx:" + IoC.Resolve<IJsonSerializer>().Serialize(tx));
                                }
                            }
                            catch (NHibernate.Exceptions.GenericADOException ex)
                            {
                                var mysqlex = ex.InnerException as MySql.Data.MySqlClient.MySqlException;
                                if (mysqlex != null && mysqlex.Number == 1062)
                                {
                                    Log.Info("已接收该交易，重复的到帐消息" + IoC.Resolve<IJsonSerializer>().Serialize(tx) + "，做丢弃处理");
                                }
                            }
                            catch (CommandExecutionException ex)
                            {
                                Log.Error("创建新的转入交易指令时出现错误", ex);
                            }
                        }
                        else
                        {
                            Log.Warn("收到新的转入交易,但无任何的destinationtag" + IoC.Resolve<IJsonSerializer>().Serialize(tx));
                        }
                    }
                }
            });
        }

        #endregion
    }
}
