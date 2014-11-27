using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using System.Threading;
using DotPay.Tools.NXTClient;
using System.Net;
using DotPay.Common;
using FluentData;
using DotPay.QueryService;
using RabbitMQ.Client.Content;
using RabbitMQ.Client;

namespace DotPay.Tools.NXTMonitor
{
    internal class NXTBalanceWatcher
    {

        public static bool Started { get; private set; }
        private static CurrencyType _currency;
        private static IDbContext _dbContext;
        private static MQConnectionPool _mqpool;
        private static NXTClient4Net _nxtclient;

        internal static void Start(MQConnectionPool mqpool)
        {
            if (NXTBalanceWatcher.Started)
            {
                Log.Info("{0}余额监控器已启动", Config.CoinCode);
                return;
            }
            _nxtclient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase);
            _dbContext = new DbContext().ConnectionString(Config.DBConnectString, new MySqlProvider());
            _mqpool = mqpool;

            var currencies = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            StartNXTBalanceWatcher(_currency);
        }

        #region 私有方法

        private static void StartNXTBalanceWatcher(CurrencyType currency)
        {
            var currencyQuery = IoC.Resolve<ICurrencyQuery>();
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    var balance = 0M;
                    if (TryGetBalance(out balance))
                    {
                        UpdateBalanceRecord(currency, balance);
                        var currencySetting = currencyQuery.GetAllCurrencies().SingleOrDefault(c => c.Code == currency.ToString());
                        //如果小于该币提现日限额*3的数量，则发送预警消息
                        var warnLine = currencySetting.WithdrawOnceLimit * 3;
                        if (balance < warnLine)
                        {
                            SendBalanceWarnMessage("{0}已不足{1:#####},请及时补充".FormatWith(currency, warnLine));
                        }
                    }
                    else
                    {
                        Log.Warn("获取{0}钱包余额失败,flg={1}", currency, Config.StatisticsFlg);
                    }
                    var sleepTime = Config.Debug ? 1000 : 300 * 1000;
                    Thread.Sleep(sleepTime);
                }
            }));

            Log.Info("{0}钱包余额监控器正在启动...", currency);
            thread.Start();
            Log.Warn("{0}钱包余额监控器启动成功！", currency);
        }

        private static bool TryGetBalance(out decimal amount)
        {
            amount = 0;

            try
            {
                var walletInfo = _nxtclient.GetBalance(Config.NxtSumAccount).Result;
                amount = Convert.ToDecimal(walletInfo.BalanceNQT / 100000000);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("TryGetBalance RPC请求时出现错误", ex);
                return false;
            }
        }

        #region 更新虚拟币余额记录
        private static void UpdateBalanceRecord(CurrencyType currency, decimal balance)
        {
            try
            {
                if (!ExistStatisticsData(currency))
                    CreateStatisticsData(currency, balance, Config.StatisticsFlg);
                else
                    UpdateStatisticsData(currency, balance, Config.StatisticsFlg);
            }
            catch (Exception ex)
            {
                Log.Error("更新{0}的余额统计出错,flg={1}".FormatWith(currency, Config.StatisticsFlg), ex);
            }
        }
        #endregion

        #region 统计数据操作方法

        private static bool ExistStatisticsData(CurrencyType currency)
        {
            var sql = @"SELECT COUNT(*)
                             FROM  " + Config.Table_Prefix + @"virtualcoinstatistics
                            WHERE  Currency=@currency AND StatisticsFlg=@flg";


            var existStatistics = _dbContext.Sql(sql)
                                            .Parameter("@currency", currency)
                                            .Parameter("@flg", Config.StatisticsFlg)
                                            .QuerySingle<int>();

            return existStatistics > 0;
        }
        private static void CreateStatisticsData(CurrencyType currency, decimal amount, string statisticsFlg)
        {
            var sql_insert = @"INSERT INTO " + Config.Table_Prefix + "virtualcoinstatistics(Currency,Amount,StatisticsFlg,LastUpdateAt)" +
                                                                                                  " VALUES(@currency,@amount,@statisticsFlg,@lastUpdate)";

            _dbContext.Sql(sql_insert)
                      .Parameter("@currency", currency)
                      .Parameter("@amount", amount)
                      .Parameter("@statisticsFlg", statisticsFlg)
                      .Parameter("@lastUpdate", DateTime.Now.ToUnixTimestamp())
                      .Execute();
        }
        private static void UpdateStatisticsData(CurrencyType currency, decimal amount, string statisticsFlg)
        {
            var sql_update = @"UPDATE " + Config.Table_Prefix + "virtualcoinstatistics SET Amount=@amount,LastUpdateAt=@lastUpdate  " +
                               "WHERE Currency=@currency AND  StatisticsFlg=@statisticsFlg";

            _dbContext.Sql(sql_update)
                      .Parameter("@currency", currency)
                      .Parameter("@amount", amount)
                      .Parameter("@statisticsFlg", statisticsFlg)
                      .Parameter("@lastUpdate", DateTime.Now.ToUnixTimestamp())
                      .Execute();
        }
        #endregion

        #region 发送虚拟币余额预警消息
        private static void SendBalanceWarnMessage(string message)
        {
            var mqname = "SendBalanceWarnMessage";
            var channel = _mqpool.GetMQChannel(mqname);
            var exchangeAndQueueName = Utilities.GenerateBanlanceWarnExchangeAndQueueName();
            var build = new BytesMessageBuilder(channel);
            build.WriteBytes(Encoding.UTF8.GetBytes(message));
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
        #endregion

        #endregion

    }
}
