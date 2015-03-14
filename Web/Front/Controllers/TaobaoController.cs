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
        private const string TaobaoSessionKey = "_taobao_session";
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
                Cache.Add(TaobaoSessionKey, session, TimeSpan.FromHours(24));
                ViewBag.Message = "授权成功";
            }
            return View();
        } 
    }
}