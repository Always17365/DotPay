//#define TAOBAODEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DFramework;
using DFramework.Utilities;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;
using ConfigurationManagerWrapper = DFramework.ConfigurationManagerWrapper;
using Task = System.Threading.Tasks.Task;

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoUtils
    {
        private const string TAOBAO_SESSION_KEY = "_taobao_session_json";
        private const string API_KEY = "23089573";
        private const string API_SECRET = "7b4cb6afdd716c3fb4a4d0dd98b8d593";
        private const string TAOBAO_REST_URL = "http://gw.api.taobao.com/router/rest";
        private static DateTime? _lastNoticeAt;
        private static readonly ITopClient Client = new DefaultTopClient(TAOBAO_REST_URL, API_KEY, API_SECRET);
        private static readonly object NoticeLocker = new object();

#if TAOBAODEBUG
        private static string _debugSession;
        private static List<Trade> DebugTrades = new List<Trade>();
        private static object locker = new object();
        private static long tidSeed = 885168847931951;
        private static int counter = 0;
#endif

#if TAOBAODEBUG
        public static void StartGenerateTaobaoTrade()
        {
            new Thread(() =>
            {
                while (true)
                {
                    var random = new Random();
                    var randomNum = random.Next(80, 120);

                    lock (locker)
                    {

                        var randomNumAmount = random.Next(1, 100);
                        var randomNumHasMessage = random.Next(1, 100);
                        //tid,status,total_fee,has_buyer_message,orders.title
                        ++counter;
                        var totalFee = (randomNumAmount * 10).ToString();
                        var buyerMessage = string.Empty;
                        if (randomNumHasMessage >= 10 && randomNumHasMessage < 20)
                            buyerMessage = "一个错误的用户留言";
                        else if (randomNumHasMessage >= 20 && randomNumHasMessage < 30)
                            buyerMessage = "r3iQifspCXQXrBsaTKy3vWw6zWTB3VWSTL";
                        else if (randomNumHasMessage >= 30)
                            buyerMessage = "rUSweLuRhP8xWd11FsnEjbDo8hJPcvnuLm";

                        var trade = new Trade()
                        {
                            Tid = tidSeed + counter,
                            Status = "WAIT_SELLER_SEND_GOODS",
                            BuyerNick = "test" + randomNumHasMessage,
                            TotalFee = totalFee,
                            PayTime = DateTime.Now.ToString(),
                            HasBuyerMessage = !string.IsNullOrEmpty(buyerMessage),
                            BuyerMessage = buyerMessage,
                            Orders = new List<Order>()
                            {
                               //tid,total_fee,buyer_message
                               new Order()
                               {
                                    TotalFee = totalFee
                               }
                            }
                        };

                        DebugTrades.Add(trade);
                        Log.Info("GenerateTaobaoTrade-->amount=" + trade.TotalFee + ",message=" + trade.BuyerMessage + ",tid=" + trade.Tid + ",hasmsg=" + !string.IsNullOrEmpty(buyerMessage));

                    }
                    Log.Debug("-->已生成{0}条淘宝交易,其中目前Dic中持有{1}条,已成功处理{2}", counter, DebugTrades.Count, counter - DebugTrades.Count);
                    Task.Delay(randomNum * 1000).Wait();
                }
            }).Start();

            Log.Info("AutoGenerateTaobaoTrade Startd");
        }
#endif


        public static IEnumerable<TaobaoSession> GetTaobaoSessionList()
        {
#if TAOBAODEBUG
            if (string.IsNullOrEmpty(_debugSession))
            {
                _debugSession = Guid.NewGuid().ToString();
                hasSession = true;
            }

            return _debugSession;
#else
            var taobaoSession = Cache.Get<Dictionary<string, TaobaoSession>>(TAOBAO_SESSION_KEY);
            var sessionList = new List<TaobaoSession>();
            if (taobaoSession != null && taobaoSession.Any())
            {
                sessionList = taobaoSession.Select(dic => dic.Value).Where(s => s.AuthAt.AddDays(1) > DateTime.Now).ToList();
            }

            if (sessionList.Count(s => s.AuthAt.AddHours(23) > DateTime.Now) == 0)
            {
                lock (NoticeLocker)
                {
                    if (((_lastNoticeAt.HasValue && _lastNoticeAt.Value.AddMinutes(10) < DateTime.Now) ||
                         !_lastNoticeAt.HasValue))
                    {
                        var mails = ConfigurationManagerWrapper.AppSettings["noticeMails"];
                        var mailList = mails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        Log.Info("taobao session time out ,notice webmaster");
                        _lastNoticeAt = DateTime.Now;
                        var title = "taobao session 超时或即将超时";
                        var msg = "";
                        sessionList.ForEach(s =>
                        {
                            if (s.AuthAt.AddDays(1) > DateTime.Now)
                            {
                                var timespan = DateTime.Now - s.AuthAt.AddDays(1);
                                msg += s.NickName + "session即将超时,还剩余" + timespan.TotalMinutes + "<br>";
                            }
                            else
                            {
                                msg += s.NickName + "session已超时<br>";
                            }
                        });

                        msg += "点击<a href='https://www.dotpay.co/taobao/login' >https://www.dotpay.co<a/>进行授权";
                        if (mailList.Any())
                        {
                            mailList.ForEach(m =>
                            {
                                EmailHelper.SendMailAsync(m, title, msg);
                            });
                        }
                    }
                }
            }

            return sessionList;
#endif
        }

        public class TaobaoSession
        {
            public string NickName { get; set; }
            public string Session { get; set; }
            public DateTime AuthAt { get; set; }
        }

        /// <summary>
        /// 获取最近一个小时内，订单状态发生了变化的订单
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static List<Trade> GetIncrementTaobaoTrade(string sessionKey)
        {
#if TAOBAODEBUG
            lock (locker)
            {
                return DebugTrades.Where(t => t.Status == "WAIT_SELLER_SEND_GOODS").ToList();
            }
#else
            TradesSoldIncrementGetRequest req = new TradesSoldIncrementGetRequest();
            req.Fields = "tid,status,buyer_nick,pay_time,has_buyer_message,orders.title,orders.price,orders.num";

            DateTime start = DateTime.Now.AddHours(-3);
            req.StartModified = start;
            DateTime end = DateTime.Now;
            req.EndModified = end;
            req.Type = "fixed";
            req.ExtType = "service";
            req.PageNo = 1L;
            req.PageSize = 100L;
            req.UseHasNext = true;

            var response = Client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("GetCompletePaymentOrder Error:" + response.ErrMsg + "--code=" + response.ErrCode);
            }

            return response.Trades;
#endif
        }

        public static Trade GetTradeFullInfo(long tid, string sessionKey)
        {
#if TAOBAODEBUG
            lock (locker)
            {
                return DebugTrades.SingleOrDefault(t => t.Tid == tid);
            }
#else
            TradeFullinfoGetRequest req = new TradeFullinfoGetRequest();
            req.Fields = "tid,buyer_message, orders.price, orders.num";
            req.Tid = tid;
            TradeFullinfoGetResponse response = Client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("GetTradeFullInfo Error:" + response.ErrMsg + "--code=" + response.ErrCode);
                return null;
            }

            return response.Trade;
#endif
        }
        public static bool SendGoods(long tid, string sessionKey)
        {
#if TAOBAODEBUG
            lock (locker)
            {
                var exist = DebugTrades.SingleOrDefault(t => t.Tid == tid);
                if (exist != null)
                {
                    DebugTrades.Remove(exist);
                    Log.Info(tid + "淘宝订单发货完毕");
                }

                return true;
            }
#else
            LogisticsDummySendRequest req = new LogisticsDummySendRequest();
            req.Tid = tid;
            LogisticsDummySendResponse response = Client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("SendGoods Error:" + response.ErrMsg + "--code=" + response.ErrCode);
            }
            else
            {
                Log.Info(tid + "淘宝订单发货完毕");
            }
            return !response.IsError;
#endif
        }
        public static bool CloseOrder(long tid, string sessionKey)
        {
#if TAOBAODEBUG
            lock (locker)
            {
                var exist = DebugTrades.SingleOrDefault(t => t.Tid == tid);
                if (exist != null)
                {
                    DebugTrades.Remove(exist);
                    Log.Info(tid + "淘宝订单由于留言错误，被卖家主动关闭");
                }

                return true;
            }
#else
            var req = new TradeCloseRequest();
            req.Tid = tid;
            req.CloseReason = "买家信息填写错误，重新拍";
            TradeCloseResponse response = Client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("SendGoods Error:" + response.ErrMsg + "--code=" + response.ErrCode);
            }
            else
            {
                Log.Info(tid + "淘宝订单由于留言错误，被卖家主动关闭");
            }
            return !response.IsError;
#endif
        }

        public static void NoticeWebMaster(string title, string message)
        {
            var mails = ConfigurationManagerWrapper.AppSettings["noticeMails"];
            var mailList = mails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (mailList.Any())
                mailList.ForEach(m =>
                {
                    EmailHelper.SendMailAsync(m, title, message);
                });
        }
    }
}
