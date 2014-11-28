using FC.Framework;
using DotPay.Command;
using DotPay.Common;
using DotPay.Tools.NXTClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentData;
using System.Collections.Generic;

namespace DotPay.Tools.NXTMonitor
{
    internal class NXTTransactionConfirmationValidator
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static IModel _channel;
        private static CurrencyType _currency;
        private static IDbContext _dbContext;
        private object _connectLock = new object();
        private static NXTClient4Net nxtClient;

        internal static void Start(MQConnectionPool mqpool)
        {
            if (NXTTransactionConfirmationValidator.Started)
            {
                Log.Info("{0}转账确认验证器已启动", Config.CoinCode);
                return;
            }

            nxtClient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase);
            _dbContext = new DbContext().ConnectionString(Config.DBConnectString, new MySqlProvider());
            _mqpool = mqpool;

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (!NXTTransactionConfirmationValidator.Started)
                    {
                        Log.Info("{0}转账确认验证器启动成功", Config.CoinCode);
                        NXTTransactionConfirmationValidator.Started = true;
                    }

                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，虚拟币转账确认验证器停止运行");
                        break;
                    }

                    var unconfirmTxs = GetUnconfirmPaymentTransactions().ToList();

                    if (unconfirmTxs != null && unconfirmTxs.Count() > 0)
                    {
                        var needConfirmations = GetNeedConfirmationCount(_currency);

                        for (int i = 0; i < unconfirmTxs.Count(); i++)
                        {
                            var tx = unconfirmTxs[i];

                            var txRepos = default(GetTransactionResponse);

                            if (TryGetTransaction(tx.TxID, out txRepos))
                            {
                                if (txRepos.Confirmations >= needConfirmations)
                                {
                                    ProcessTx(txRepos, _currency);
                                }
                                else
                                {
                                    Log.Info("txid={0}的交易确认不足{1}个,等待下次处理", tx.TxID, needConfirmations);
                                }
                            }
                        }
                    }

                    Thread.Sleep(Config.LoopInterval * 1000);
                }
            }));

            thread.Start();
        }

        #region 私有方法

        #region try get transaction
        private static bool TryGetTransaction(string txid, out GetTransactionResponse tx)
        {
            tx = default(GetTransactionResponse);
            try
            {
                tx = nxtClient.GetTransactionAsync(txid).Result;
                return true;
            }
            catch (NXTCallException ex)
            {
                Log.Warn("TryGetTransaction访问出错:" + ex.Message);
                return false;
            }
        }
        #endregion

        private static void ProcessTx(GetTransactionResponse tx, CurrencyType currency)
        {
            if (!ExistConfirmTx(tx, currency))
            {
                Log.Info("txid={0}的交易确认已达到{1}个,开始发送确认消息..", tx.Transaction, tx.Confirmations);
                var mqname = "NXTTransactionConfirmationValidator";
                _channel = _mqpool.GetMQChannel(mqname);
                var build = new BytesMessageBuilder(_channel);
                var exchangeName = Utilities.GenerateVirtualCoinReceivePaymentExchangeAndQueueName(currency).Item1;

                try
                {
                    var amount = tx.AmountNQT / 100000000;
                    var cmd = new ConfirmReceivePaymentTransaction(tx.Transaction, tx.RecipientRS, tx.Confirmations, amount, currency);
                    build.WriteBytes(Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(cmd)));
                    ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

                    _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());

                    var cacheKey = currency.ToString() + tx.Transaction + "_" + amount + "confirm";

                    Cache.Add(cacheKey, tx.Confirmations);

                    Log.Info("txid={0}的交易确认消息发送完毕", tx.Transaction, tx.Confirmations);
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ex)
                {
                    Log.Error("发送{0}新交易创建指令时发现消息队列服务器已关闭".FormatWith(Config.CoinCode), ex);

                    try
                    {
                        _channel = _mqpool.GetMQChannel(mqname);
                        _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());

                    }
                    catch (Exception eex)
                    {
                        Log.Fatal("NXTTransactionConfirmationValidator消息队列服务器连接重试后仍无法连接成功，可能消息队列服务器已经挂掉，请尽快处理！", eex);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("发送{0}新交易创建指令时出现错误".FormatWith(Config.CoinCode), ex);
                }

            }
        }

        private static bool ExistConfirmTx(GetTransactionResponse tx, CurrencyType currency)
        {
            bool isExist = false;
            var cacheKey = currency.ToString() + tx.Transaction + "_" + (tx.AmountNQT / 100000000) + "confirm";
            var tmp = new object();


            if (Cache.TryGet(cacheKey, out tmp))
            {
                isExist = true;
            }

            return isExist;
        }

        private static IEnumerable<PaymentTransaction> GetUnconfirmPaymentTransactions()
        {

            var sql = @"SELECT ID,TxID,State,Amount,Address,Confirmation 
                          FROM " + Config.Table_Prefix + Config.TxTableName +
                       " WHERE State=@unconfirm ";


            var ptxs = _dbContext.Sql(sql).Parameter("@unconfirm", VirtualCoinTxState.WaitConfirmation).QueryMany<PaymentTransaction>();

            return ptxs;

        }

        private static int GetNeedConfirmationCount(CurrencyType currency)
        {
            var needConfirmationCount = Config.DefaultConfirmations; ;

            var cacheKey = currency.ToString() + "_" + "NXTTransactionConfirmationValidator".GetHashCode().ToString() + "_needConfirm";

            var cacheCount = Cache.Get(cacheKey);

            if (cacheCount != null)
                needConfirmationCount = (int)cacheCount;
            else
            {
                var sql = @"SELECT NeedConfirm 
                              FROM " + Config.Table_Prefix + Config.CurrencyTableName +
                           " WHERE ID=@id";

                var db_needConfirmations = _dbContext.Sql(sql).Parameter("@id", currency).QuerySingle<int>();

                if (db_needConfirmations > 0)
                {
                    needConfirmationCount = db_needConfirmations;
                    Cache.Add(cacheKey, needConfirmationCount, new TimeSpan(0, 0, 600));
                }
            }

            return needConfirmationCount;
        }
         
        #endregion

        #region 数据实体类
        public class PaymentTransaction
        {
            public int ID { get; protected set; }
            public string TxID { get; protected set; }
            public VirtualCoinTxState State { get; protected set; }
            public decimal Amount { get; protected set; }
            public string Address { get; protected set; }
            public int Confirmation { get; protected set; }
        }
        #endregion
    }
}
