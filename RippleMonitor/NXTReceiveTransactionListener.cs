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
    internal class NXTReceiveTransactionListener
    {
        public static bool Started { get; private set; }
        private static MQConnectionPool _mqpool;
        private static IModel _channel;
        private static CurrencyType _currency;
        private static IDbContext _dbContext;
        private object _connectLock = new object();
        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private static NXTClient4Net nxtClient;

        internal static void Start(MQConnectionPool mqpool)
        {
            if (NXTReceiveTransactionListener.Started)
            {
                Log.Info("NXT到款交易监听器已启动");
                return;
            }

            _mqpool = mqpool;
            nxtClient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase);
            _dbContext = new DbContext().ConnectionString(Config.DBConnectString, new MySqlProvider());

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (!NXTReceiveTransactionListener.Started)
                    {
                        Log.Info("{0}到款交易监听器启动成功,监听交易中...", Config.CoinCode);
                        NXTReceiveTransactionListener.Started = true;
                    }
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，虚拟币交易监听器停止运行");
                        _cancelTokenSource.Cancel();
                        break;
                    }

                    var isProcessToEnd = false;
                    var offset = 0;

                    while (!isProcessToEnd)
                    {
                        var nxtAccounts = GetNXTAccounts(offset);

                        if (nxtAccounts != null && nxtAccounts.Count() > 0)
                        {
                            offset = nxtAccounts.Last().ID;

                            nxtAccounts.AsParallel().ForAll(nc =>
                            {
                                var txIds = GetTxIds(nc.UserID, nc.NxtAccountID, 0);

                                if (txIds != null && txIds.TransactionIds.Length > 0)
                                {
                                    txIds.TransactionIds.ForEach(txid =>
                                    {
                                        if (!ExistTx(nc.Address, txid, _currency))
                                        {
                                            var tx = default(GetTransactionResponse);

                                            if (TryGetTransaction(txid, out tx)) ProcessTx(nc.UserID, tx, nc.NxtAccountID, _currency);
                                        }
                                    });
                                }
                            });
                        }
                        else isProcessToEnd = true;
                    }

                    Thread.Sleep(Config.LoopInterval * 1000);
                }
            }));

            thread.Start();
        }

        #region 私有方法

        #region GetNXTAccounts
        private static IEnumerable<NXTAccountModel> GetNXTAccounts(int offset)
        {
            var sql = @"SELECT ID,UserID,NXTAccountID,Address 
                          FROM " + Config.Table_Prefix + Config.NXTAddressTableName +
                       " WHERE ID>@lastId ORDER BY ID LIMIT 100 ";

            var nxtAccounts = _dbContext.Sql(sql).Parameter("@lastId", offset).QueryMany<NXTAccountModel>();

            return nxtAccounts;
        }
        #endregion

        #region GetTxIds
        private static GetAccountTransactionIdsResponse GetTxIds(int userID, UInt64 nxtAccountID, int timestamp)
        {
            var result = default(GetAccountTransactionIdsResponse);
            var cacheKey = CacheKey.USER_LAST_ACTIVE_TIME_PREFIX + userID;
            var lastActiveTime = DateTime.UtcNow;

            if (Cache.TryGet(cacheKey, out lastActiveTime))
            {
                //如果用户超过3天未登录，则不在监测用户的NXT充值
                if ((DateTime.UtcNow - lastActiveTime).TotalSeconds > 3 * 24 * 60 * 60)
                    result = default(GetAccountTransactionIdsResponse);
                else
                {
                    try
                    {
                        result = nxtClient.GetAccountTransactionIdsAsync(nxtAccountID, timestamp).Result;
                    }
                    catch (Exception ex)
                    {
                        if (!(ex.InnerException != null && ex.InnerException.Message.Equals("Unknown account", StringComparison.OrdinalIgnoreCase)))
                            Log.Warn("GetAccountTransactionIds访问出错:" + ex.Message, ex);
                    }
                }
            }

            return result;
        }
        #endregion

        #region process tx
        private static void ProcessTx(int userID, GetTransactionResponse tx, UInt64 nxtAccountID, CurrencyType currency)
        {
            var cacheKey = currency.ToString() + tx.SenderRS + tx.Transaction + "create";

            if (tx.Type != TransactionType.Ordinary || tx.Recipient != nxtAccountID)
            {
                //如果交易类型不是普通交易或者收款账户非当前账户(非收款),忽略，并加入忽略名单，防止多次请求交易明细，浪费资源
                Cache.Add(cacheKey, tx.Confirmations);
                return;
            }
            object obj = null;
            if (Cache.TryGet(cacheKey, out obj)) return;

            Log.Info("发现新的交易....");


            var actualAmount = tx.AmountNQT / 100000000;

            #region 发送交易到处理消息队列
            var mqname = "NXTReceiveTransactionListener";
            _channel = _mqpool.GetMQChannel(mqname);
            var exchangeName = Utilities.GenerateVirtualCoinReceivePaymentExchangeAndQueueName(currency).Item1;
            var build = new BytesMessageBuilder(_channel);


            var cmd = new CreateReceivePaymentTransaction(tx.Transaction, tx.RecipientRS, actualAmount, currency);
            build.WriteBytes(Encoding.UTF8.GetBytes(IoC.Resolve<IJsonSerializer>().Serialize(cmd)));
            ((IBasicProperties)build.GetContentHeader()).DeliveryMode = 2;

            try
            {
                Log.Info("交易将被发送到Exchange->{0}", exchangeName);

                _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());

                Log.Info("交易成功发送到Exchange->{0}", exchangeName);

                Cache.Add(cacheKey, tx.Confirmations);
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException ex)
            {
                Log.Error("发送{0}新交易创建指令时发现消息队列服务器已关闭".FormatWith(Config.CoinCode), ex);

                try
                {
                    _channel = _mqpool.GetMQChannel(mqname);
                    _channel.BasicPublish(exchangeName, string.Empty, ((IBasicProperties)build.GetContentHeader()), build.GetContentBody());
                    Log.Info("交易成功发送到Exchange->{0}", exchangeName);
                }
                catch (Exception eex)
                {
                    Log.Fatal("重新链接消息队列发送NXT新交易仍然出错了".FormatWith(Config.CoinCode), eex);
                }
            }
            catch (Exception ex)
            {
                Log.Error("发送{0}新交易创建指令时出现错误".FormatWith(Config.CoinCode), ex);
            }
            #endregion

            #region 提取币到总账户中
            try
            {
                var nxtclient = new NXTClient4Net(Config.NXTServer, Config.SecretPhrase + userID);
                var result = nxtclient.SendMoneyAsnc(Config.NxtSumAccount, actualAmount - 1).Result;
                Log.Info("用户充值的币发送到总帐户成功");
            }
            catch (Exception ex)
            {
                Log.Fatal("用户ID={0}充值的{1}NXT发送到总帐户失败了,请及时处理".FormatWith(userID, actualAmount), ex);
            }
            #endregion
        }
        #endregion

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

        #region exist Txid
        private static bool ExistTx(string accountRS, string txid, CurrencyType currency)
        {
            bool isExist = false;
            var cacheKey = currency.ToString() + accountRS + txid + "create";
            var tmp = new object();

            if (Cache.TryGet(cacheKey, out tmp))
            {
                isExist = true;
            }
            else
            {
                var sql = @"SELECT COUNT(*) 
                              FROM " + Config.Table_Prefix + Config.TxTableName +
                           " WHERE TxID=@txid AND Address=@address";

                var count = _dbContext.Sql(sql).Parameter("@txid", txid)
                                               .Parameter("@address", accountRS)
                                               .QuerySingle<int>();

                if (count > 0) isExist = true;
            }

            return isExist;
        }
        #endregion

        #endregion

        #region 数据实体类
        public class NXTAccountModel
        {
            public int ID { get; set; }
            public int UserID { get; set; }
            public UInt64 NxtAccountID { get; set; }
            public string Address { get; set; }
        }
        #endregion
    }
}
