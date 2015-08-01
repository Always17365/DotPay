using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Common;
using FC.Framework;
using Top.Api;
using Top.Api.Util;

namespace DotPay.Web.Controllers
{
    public class TaobaoController : Controller
    {
        private const string TaobaoContainerUrl = "http://container.open.taobao.com/container?appkey=";
        private const string TaobaoDebugContainerUrl = "http://container.api.tbsandbox.com/container?appkey=";
        private const string ApiKey = "23089573";
        private const string ApiKeyDebug = "1023089573";
        private const string ApiSecret = "7b4cb6afdd716c3fb4a4d0dd98b8d593";
        private const string ApiSecretDebug = "sandboxfdd716c3fb4a4d0dd98b8d593";
        private const string TaobaoRestUrl = "http://gw.api.taobao.com/router/rest";
        private const string TaobaoDebugRestUrl = "http://gw.api.tbsandbox.com/router/rest";
        private const bool Debug = false;
        private const string TaobaoSessionKey = "_taobao_session_json";
        [AllowAnonymous]
        [Route("~/taobao/login")]
        public ActionResult Index()
        {
            var url = Debug ? TaobaoDebugContainerUrl + ApiKeyDebug : TaobaoContainerUrl + ApiKey;
            return Redirect(url);
        }

        [AllowAnonymous]
        [Route("~/taobao/callback")]
        public ActionResult TaobaoCallback()
        {
            var secket = Debug ? ApiSecretDebug : ApiSecret;
            if (!TopUtils.VerifyTopResponse(this.Request.Url.ToString(), secket))
                ViewBag.Message = "无效授权";
            else
            {
                var session = this.Request.QueryString["top_session"];
                var taobaoParamsString = this.Request.QueryString["top_parameters"];
                var taobaoParams = TopUtils.DecodeTopParams(taobaoParamsString);

                Log.Info(taobaoParams["visitor_nick"] + "淘宝授权成功");


                var nickName = taobaoParams["visitor_nick"];
                  
                var currentAuthList = Cache.Get<Dictionary<string, TaobaoSession>>(TaobaoSessionKey);

                currentAuthList = currentAuthList ?? new Dictionary<string, TaobaoSession>();
                Log.Info("更新前=" +IoC.Resolve<IJsonSerializer>().Serialize(currentAuthList));
                if (currentAuthList.ContainsKey(nickName))
                {
                    Log.Info("更新" + nickName + "淘宝授权成功:原-" + currentAuthList[nickName].Session + ",新:" + session);
                    currentAuthList[nickName].Session = session;
                    currentAuthList[nickName].AuthAt = DateTime.Now;
                }
                else
                {
                    currentAuthList.Add(nickName, new TaobaoSession()
                    {
                        NickName = nickName,
                        Session = session,
                        AuthAt = DateTime.Now
                    });
                }

                Log.Info("更新后=" + IoC.Resolve<IJsonSerializer>().Serialize(currentAuthList));

                Cache.Add(TaobaoSessionKey, currentAuthList, TimeSpan.FromDays(3));

                currentAuthList = Cache.Get<Dictionary<string, TaobaoSession>>(TaobaoSessionKey);

                Log.Info("更新取出最新数据=" + IoC.Resolve<IJsonSerializer>().Serialize(currentAuthList));
                ViewBag.Message = "授权成功";
            }
            return View();
        }

        public class TaobaoSession
        {
            public string NickName { get; set; }
            public string Session { get; set; }
            public DateTime AuthAt { get; set; }
        }
    }
}