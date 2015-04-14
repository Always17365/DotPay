using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DFramework;
using Dotpay.Common;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dotpay.TaobaoMonitor
{
    internal class TrustLineMessageListener
    {
        private static bool started;
        private static readonly string MysqlConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("taobaodb");
        private static readonly string RabbitMqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectString");
        private static IModel _channel;
        private const string RIPPLE_TRUST_LINE_EXCHANGE_NAME = Constants.RippleTrustLineMQName + Constants.ExechangeSuffix;
        private const string RIPPLE_TRUST_LINE_QUEUE_NAME = Constants.RippleTrustLineMQName + Constants.QueueSuffix;
        public static void Start()
        {
            if (started) return;

            InitMessageExchangeAndQueueAndRegisterConsumer();

            Log.Info("-->用户Trust消息监听器启动成功...");
            started = true;
        }

        #region private method
        private static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(MysqlConnectionString);
            connection.Open();
            return connection;
        }

        private static IModel InitMessageExchangeAndQueueAndRegisterConsumer()
        {
            if (_channel == null)
            {
                var factory = new ConnectionFactory { Uri = RabbitMqConnectionString, AutomaticRecoveryEnabled = true };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
                _channel.ExchangeDeclare(RIPPLE_TRUST_LINE_EXCHANGE_NAME, ExchangeType.Direct, true, false, null);
                _channel.QueueDeclare(RIPPLE_TRUST_LINE_QUEUE_NAME, true, false, false, null);
                _channel.QueueBind(RIPPLE_TRUST_LINE_QUEUE_NAME, RIPPLE_TRUST_LINE_EXCHANGE_NAME, "");

                var consumer = new RippleTrustMessageConsumer(_channel);

                _channel.BasicQos(0, 1, false);
                _channel.BasicConsume(RIPPLE_TRUST_LINE_QUEUE_NAME, false, consumer);
            }
            return _channel;
        }
        //获取已确定失败了的交易，如果这个失败交易的原因是“未信任网关或信任额度不足”或“钱包未激活” 
        private static IEnumerable<TaobaoAutoDeposit> GetFailedTransactionWitchIsUntrustOrUnactive(string rippleAddress)
        {
            const string sql =
                "SELECT tid,amount,has_buyer_message,taobao_status,ripple_address,ripple_status,txid,memo,first_submit_at,tx_lastLedgerSequence,retry_Counter" +
                "  FROM taobao " +
                " WHERE taobao_status=@taobao_status AND ripple_status=@ripple_status AND ripple_address=@ripple_address";
            try
            {
                using (var conn = OpenConnection())
                {
                    var tradesInDb = conn.Query<TaobaoAutoDeposit>(sql, new
                    {
                        taobao_status = "WAIT_SELLER_SEND_GOODS",
                        ripple_status = RippleTransactionStatus.Failed,
                        ripple_address = rippleAddress
                    });

                    return tradesInDb;
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetFailedTransactionWitchIsUntrustOrUnactive Exception", ex);
                return null;
            }
        }


        //标记为初始状态，可自动进入下次处理过程
        private static int MarkFailedTxAsInitForNextProccesLoop(long tid)
        {
            const string sql =
              "UPDATE taobao SET ripple_status=@ripple_status_new,txid='',tx_lastLedgerSequence=0,first_submit_at=null" +
              " WHERE tid=@tid AND taobao_status=@taobao_status AND ripple_status=@ripple_status_old";
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, new { tid = tid, ripple_status_new = RippleTransactionStatus.Init, taobao_status = "WAIT_SELLER_SEND_GOODS", ripple_status_old = RippleTransactionStatus.Failed });
                }
            }
            catch (Exception ex)
            {
                Log.Error("MarkFailedTxAsInitForNextProccesLoop Exception", ex);
                return 0;
            }
        }

        //标记为初始状态，可自动进入下次处理过程
        private static int UpsertTrustLine(RippleTrustMessage message)
        {
            const string insertSql =
              "INSERT INTO  truststatistics(rippleaddress,trustamount,ledgerindex)" +
              "values(@rippleaddress,@trustamount,@ledgerindex)";

            const string updateSql =
              "UPDATE truststatistics SET trustamount=@trustamount,ledgerindex=@ledgerindex" +
              " WHERE rippleaddress=@rippleaddress";
            var result = 0;
            try
            {
                using (var conn = OpenConnection())
                {
                    Log.Info("UpsertTrustLine:" + message.RippleAddress + " trust amount=" + message.TrustAmount);
                    result = conn.Execute(updateSql, new
                  {
                      rippleaddress = message.RippleAddress,
                      trustamount = message.TrustAmount,
                      ledgerindex = message.Ledgerindex
                  });
                    //数据库不存在该数据，则执行插入
                    if (result == 0)
                    {
                        result = conn.Execute(insertSql, new
                        {
                            rippleaddress = message.RippleAddress,
                            trustamount = message.TrustAmount,
                            ledgerindex = message.Ledgerindex
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("UpsertTrustLine Exception", ex);
            }
            return result;
        }

        #endregion

        #region consumer
        public class RippleTrustMessageConsumer : DefaultBasicConsumer
        {
            //private Action<RippleTxMessage> action;
            //private Action<Exception> errorCallback;
            public RippleTrustMessageConsumer(IModel model)
                : base(model) { }
            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(body);
                    var trustMessage = IoC.Resolve<IJsonSerializer>().Deserialize<RippleTrustMessage>(messageBody);

                    try
                    {
                        UpsertTrustLine(trustMessage);
                        var failedTxs = GetFailedTransactionWitchIsUntrustOrUnactive(trustMessage.RippleAddress);

                        if (failedTxs.Any())
                        {
                            failedTxs.ForEach(lt =>
                            {
                                if (lt.amount < trustMessage.TrustAmount)
                                {

                                    Log.Info("发现已失败的自动充值，失败原因为-" + lt.memo + " ,用户调整了trust=" + trustMessage.TrustAmount +
                                             ": txid=" + lt.txid + ",tid=" + lt.tid + ",amount=" +
                                             lt.amount + ",address=" + lt.ripple_address);

                                    var result = MarkFailedTxAsInitForNextProccesLoop(lt.tid);

                                    Log.Info("更新为初始状态，等待下次重新自动处理该笔充值=" + result + "，失败原因为-" + lt.memo + " : txid=" +
                                             lt.txid + ",tid=" + lt.tid + ",amount=" +
                                             lt.amount + ",address=" + lt.ripple_address);
                                }
                                else
                                {
                                    Log.Info("发现已失败的自动充值，失败原因为-" + lt.memo + " ,用户调整了trust=" + trustMessage.TrustAmount +
                                            ": txid=" + lt.txid + ",tid=" + lt.tid + ",amount=" +
                                            lt.amount + ",address=" + lt.ripple_address + "但trust额度不足");
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("UnTrustOrNotEnoughTrustRetryMonitor  Exception", ex);
                    }


                    Model.BasicAck(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error("process ripple tx message error", ex);
                    Model.BasicNack(deliveryTag, false, true);
                }
            }
        }
        #endregion


        #region Message Class

        internal class RippleTrustMessage
        {
            public RippleTrustMessage(string rippleAddress, long trustAmount, long ledgerindex)
            {
                this.RippleAddress = rippleAddress;
                this.TrustAmount = trustAmount;
                this.Ledgerindex = ledgerindex;
            }

            public string RippleAddress { get; set; }
            public long TrustAmount { get; set; }
            public long Ledgerindex { get; set; }
        }
        #endregion
    }
}
