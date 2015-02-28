using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;

namespace Dotpay.TaobaoMonitor
{
    internal class TaobaoUtils
    {
        private const string TaobaoSessionKey = "_taobao_session";
        private const string ApiKey = "23089573";
        private const string ApiSecret = "7b4cb6afdd716c3fb4a4d0dd98b8d593";
        private const string TaobaoRestUrl = "http://gw.api.taobao.com/router/rest";
        private static bool hasSession = false;
        private static DateTime? lastNoticeAt;
        private static ITopClient client = new DefaultTopClient(TaobaoRestUrl, ApiKey, ApiSecret);
        public static string GetTaobaoSession()
        {
            var taobaoSession = Cache.Get<string>(TaobaoSessionKey);

            if (string.IsNullOrWhiteSpace(taobaoSession)) hasSession = false;
            else hasSession = true;

            return taobaoSession;
        }


        public static List<Trade> GetCompletePaymentTrade(string sessionKey)
        {
            TradesSoldGetRequest req = new TradesSoldGetRequest();
            req.Fields = "tid,status,total_fee,has_buyer_message,orders.title";

            req.Status = "WAIT_SELLER_SEND_GOODS";
            req.Type = "fixed";
            req.ExtType = "service";
            req.PageNo = 1L;
            req.PageSize = 100L;
            req.UseHasNext = true;
            TradesSoldGetResponse response = client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("GetCompletePaymentOrder Error:" + response.ErrMsg + "--code=" + response.ErrCode);
            }

            return response.Trades;
        }

        public static Trade GetTradeFullInfo(long tid, string sessionKey)
        {
            TradeFullinfoGetRequest req = new TradeFullinfoGetRequest();
            req.Fields = "tid,total_fee,trade_memo,buyer_message";
            req.Tid = tid;
            TradeFullinfoGetResponse response = client.Execute(req, sessionKey);

            if (response.IsError)
            {
                Log.Error("GetTradeFullInfo Error:" + response.ErrMsg + "--code=" + response.ErrCode);
            }

            return response.Trade;
        }


        public static void NoticeWebMaster()
        {
            if (!hasSession && lastNoticeAt.HasValue && lastNoticeAt.Value.AddMinutes(10) > DateTime.Now)
            {
                Log.Info("taobao session time out ,notice webmaster");
                lastNoticeAt = DateTime.Now;
            }
        }
    }

    internal class TaobaoSessionNotExistOrTimeOutException : Exception
    {
        public TaobaoSessionNotExistOrTimeOutException()
            : base("淘宝Session不存在或已超时")
        {
        }
    }
}
