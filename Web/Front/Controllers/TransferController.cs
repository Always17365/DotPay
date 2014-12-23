using FC.Framework;
using DotPay.Common;
using DotPay.Command;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using QConnectSDK.Context;
using QConnectSDK;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using Newtonsoft.Json;
using RippleRPC.Net;
using DotPay.RippleCommand;

namespace DotPay.Web.Controllers
{
    public class TransferController : BaseController
    {
        private const string OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY = "OUTSIDE_GATEWAY_ACCOUNT_INFO";
        private const string OUTSIDE_GATEWAY_ACCOUNT_PATHS = "OUTSIDE_GATEWAY_ACCOUNT_PATHS";
        #region Views

        [Route("~/apps")]
        public ActionResult AppIndex()
        {
            return View("Index");
        }

        #region 内部转账页面

        [Route("~/transfer/{currency}/payment")]
        public ActionResult InsideTransfer(CurrencyType currency)
        {
            ViewBag.Currency = currency.ToString();
            return View("Inside");
        }

        [Route("~/transfer/{currency}/confirm")]
        public ActionResult InsideTransferConfirm(CurrencyType currency, string orderID)
        {
            var transfer = IoC.Resolve<IInsideTransferQuery>().GetInsideTransferBySequenceNo(orderID, TransactionState.Pending, currency);
            ViewBag.Transfer = transfer;

            if (transfer != null)
            {
                var user = IoC.Resolve<IUserQuery>().GetUserByID(transfer.ToUserID);
                ViewBag.Receiver = user;
            }
            return View("InsideConfirm");
        }

        [Route("~/transfer/{currency}/success")]
        public ActionResult InsideTransferSuccess(CurrencyType currency, string orderID)
        {
            var transfer = IoC.Resolve<IInsideTransferQuery>().GetInsideTransferBySequenceNo(orderID, TransactionState.Success, currency);
            var user = IoC.Resolve<IUserQuery>().GetUserByID(transfer.ToUserID);
            ViewBag.Transfer = transfer;
            ViewBag.Receiver = user;

            return View("InsideTransferSuccess");
        }
        #endregion

        #region 转账到第三方支付页面
        [Route("~/transfertpp/{payway}/payment")]
        public ActionResult TppTransfer(PayWay payway)
        {
            ViewBag.PayWay = payway;

            return View("OutboundTpp");
        }

        [Route("~/transfertpp/{payway}/confirm")]
        public ActionResult OutsideTransferConfirm(PayWay payway, string orderID)
        {
            var transfer = IoC.Resolve<IOutsideTransferQuery>().GetOutsideTransferBySequenceNo(orderID, TransactionState.Init);

            ViewBag.PayWay = payway;
            ViewBag.Transfer = transfer;

            return View("OutboundTppConfirm");
        }

        [Route("~/transfertpp/{payway}/success")]
        public ActionResult OutboundTppTransferSuccess(PayWay payway, string orderID)
        {
            var transfer = IoC.Resolve<IOutsideTransferQuery>().GetOutsideTransferBySequenceNo(orderID, TransactionState.Init);

            ViewBag.Transfer = transfer;

            return View("OutboundTppTransferSuccess");
        }

        [Route("~/query")]
        [Route("~/query/{txid}")]
        [AllowAnonymous]
        public ActionResult InBoundTppQueryByTxid(string txid)
        {
            ViewBag.TxId = txid; 

            if (!string.IsNullOrEmpty(txid))
            {
                var result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionByRippleTxid(txid, PayWay.Alipay);
                if (result == null)
                {
                    result = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionByRippleTxid(txid, PayWay.Tenpay);
                }

                ViewBag.Transaction = result; 
            }

            return View("InboundRippleQuery");
        }

        #endregion

        #region 外部（Ripple）转账页面

        [Route("~/transferout/ripple/payment")]
        public ActionResult OutsideTransfer()
        {
            return View("OutboundRipple");
        }

        [Route("~/transferout/ripple/confirm")]
        public ActionResult OutsideTransferConfirm(string orderID)
        {
            var transfer = IoC.Resolve<IOutsideTransferQuery>().GetOutsideTransferBySequenceNo(orderID, TransactionState.Pending);

            ViewBag.Transfer = transfer;
            return View("OutsideConfirm");
        }

        [Route("~/transferout/success")]
        public ActionResult OutboundTransferSuccess(string orderID)
        {
            var transfer = IoC.Resolve<IOutsideTransferQuery>().GetOutsideTransferBySequenceNo(orderID, TransactionState.Success);

            ViewBag.Transfer = transfer;

            return View("OutboundTransferSuccess");
        }

        [Route("~/transferripplesuccess")]
        public ActionResult OutboundRippleSubmitSuccess(string orderID)
        {
            var transfer = IoC.Resolve<IOutsideTransferQuery>().GetOutsideTransferBySequenceNo(orderID, TransactionState.Success);

            ViewBag.Transfer = transfer;

            return View("OutboundRippleSubmitSuccess");
        }
        #endregion

        #endregion

        #region Posts

        #region 内部转账
        #region 查询账号
        [Route("~/transfer/queryacct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QueryAccount(string account)
        {
            account = account.NullSafe().Trim();

            if (!string.IsNullOrEmpty(account))
            {
                LoginUser user;

                if (account.IsEmail())
                    user = IoC.Resolve<IUserQuery>().GetUserByEmail(account);
                else
                    user = IoC.Resolve<IUserQuery>().GetUserByLoginName(account);

                if (user != null)
                {
                    return Json(new { Account = account, RealName = user.RealName, valid = true });
                }
                else
                {
                    return Json(new { message = "该账户未注册，不能转账", valid = false });
                }
            }
            return Json(new { Valid = false });
        }
        #endregion

        #region 内部转账创建

        [Route("~/transfer/{currency}/payment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsideTransfer(string account, decimal amount, CurrencyType currency, string description)
        {
            account = account.NullSafe().Trim();
            description = description.NullSafe().Trim();
            amount = amount.ToFixed(2);


            LoginUser user;

            if (account.IsEmail())
                user = IoC.Resolve<IUserQuery>().GetUserByEmail(account);
            else
                user = IoC.Resolve<IUserQuery>().GetUserByLoginName(account);

            if (user == null)

                return Redirect("/Error");

            var transferCMD = new CreateInsideTransfer(this.CurrentUser.UserID, user.UserID, currency, amount, description);

            this.CommandBus.Send(transferCMD);

            return Redirect("~/transfer/{0}/confirm?orderid={1}".FormatWith(currency, transferCMD.Result));



        }
        #endregion

        #region 内部转账确认

        [Route("~/transfer/{currency}/confirm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsideTransferConfirmPost(CurrencyType currency, string orderId, string paypassword)
        {
            var result = FCJsonResult.UnknowFail;
            try
            {
                var cmd = new SubmitInsideTransfer(orderId, paypassword, currency);
                this.CommandBus.Send(cmd);

                result = FCJsonResult.Success;
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.AccountBalanceNotEnough)
                    result = FCJsonResult.CreateFailResult("余额不足，无法完成支付");
                else if (ex.ErrorCode == (int)ErrorCode.TradePasswordError)
                    result = FCJsonResult.CreateFailResult("支付密码错误");
                else
                {
                    Log.Error("InsideTransferConfirmPost Action Error", ex);
                }
            }
            return Json(result);
        }
        #endregion
        #endregion

        #region 外部转账
        #region 第三方转账提交
        [HttpPost]
        [Route("~/transfertpp/{payway}/payment")]
        public ActionResult SubmitOutboundPayment(PayWay payway, string account, decimal amount, string description)
        {
            var emailReg = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            var mobileReg = new Regex("^1[3|5|7|8|][0-9]{9}$");
            var qqReg = new Regex(@"^\d{5,10}$");

            var result = false;
            var message = "无效的收款账户";
            var accountInfo = default(FederationResponse);
            account = account.NullSafe().Trim();


            if ((payway == PayWay.Alipay && (emailReg.IsMatch(account) || mobileReg.IsMatch(account))) ||
                (payway == PayWay.Tenpay && (emailReg.IsMatch(account) || mobileReg.IsMatch(account) || qqReg.IsMatch(account))))
            {
                var cmd = new CreateOutboundTransfer(payway, account, CurrencyType.CNY.ToString(), amount, amount, description.NullSafe().Trim(), this.CurrentUser.UserID);

                this.CommandBus.Send(cmd);

                return Redirect("~/transfertpp/{0}/confirm?orderid={1}".FormatWith(payway, cmd.Result));
            }

            return View();
        }
        #endregion

        #region 第三方转账确认
        [Route("~/transfertpp/confirm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OutboundTransferConfirmPost(string orderId, string paypassword)
        {
            var result = FCJsonResult.UnknowFail;
            try
            {
                var cmd = new ConfirmOutboundTransfer(orderId, paypassword, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                result = FCJsonResult.Success;
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.AccountBalanceNotEnough)
                    result = FCJsonResult.CreateFailResult("余额不足，无法完成支付");
                else if (ex.ErrorCode == (int)ErrorCode.TradePasswordError)
                    result = FCJsonResult.CreateFailResult("支付密码错误");
                else
                {
                    Log.Error("InsideTransferConfirmPost Action Error", ex);
                }
            }
            return Json(result);
        }
        #endregion

        #region 验证收款机构是否支持ripple协议
        [Route("~/transferout/verifyaccount")]
        public async Task<ActionResult> VerifyAccount(string account)
        {
            string emailReg = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var reg = new Regex(emailReg);

            var result = false;
            var message = "无效的收款账户";
            var accountInfo = default(FederationResponse);
            account = account.NullSafe().Trim();

            if (reg.IsMatch(account))
            {
                var email = new MailAddress(account);
                //email.Host
                var federationUrlResult = await TryGetFederationUrl(email.Host);

                if (federationUrlResult.Item1 == true)
                {
                    accountInfo = await TryGetOutsideAccountInfo(email.User, email.Host, federationUrlResult.Item2);

                    if (accountInfo.Result == "success")
                    {
                        Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account] = accountInfo;
                        result = true;
                    }
                }

            }

            if (result == true)
                return Json(new { valid = true, currencies = accountInfo.UserAccount.AcceptCurrencys });
            else
                return Json(new { valid = false, message = message });
        }
        #endregion

        #region Ripple Path Find
        [Route("~/transferout/calc")]
        [HttpPost]
        public async Task<ActionResult> CalcPayAmount(string account, decimal amount, string currency)
        {
            string emailReg = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var reg = new Regex(emailReg);

            var result = false;
            var message = "无效的收款账户";
            var accountInfo = default(FederationResponse);
            account = account.NullSafe().Trim();

            if (reg.IsMatch(account))
            {

                if (Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account] != null)
                {
                    var rippleAccountInfo = (FederationResponse)Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account];
                    if (rippleAccountInfo != null)
                    {
                        var rippleClient = IoC.Resolve<IRippleClientAsync>();
                        var pathsum = await rippleClient.RipplePathFind(Config.RippleHotWallet, rippleAccountInfo.UserAccount.DestinationAddress,
                                            new RippleRPC.Net.Model.RippleCurrencyValue()
                                            {
                                                _Value = amount.ToString(),
                                                Currency = currency,
                                                Issuer = rippleAccountInfo.UserAccount.DestinationAddress
                                            });

                        if (pathsum.Item1 == null && (pathsum.Item2.Alternatives != null))
                        {
                            var needSendAmount = pathsum.Item2.Alternatives.SingleOrDefault(an => an.SourceAmount.Currency == "CNY");
                            if (needSendAmount != null)
                            {
                                Session[OUTSIDE_GATEWAY_ACCOUNT_PATHS + account] = needSendAmount.ComputedPaths;
                                return Json(new { valid = true, amount = needSendAmount.SourceAmount.Value });
                            }
                            return Json(new { valid = false, message = "市场深度不足以支持本次汇兑" });
                        }
                        else
                        {
                            Log.Error("计算Ripple汇率是出现错误:" + pathsum.Item1.Message);
                            return Json(new { valid = false, message = "网络错误，请稍后重试" });
                        }
                    }
                }
            }

            return Json(new { valid = false });
        }
        #endregion

        #region Ripple转账提交
        [Route("~/transferout/submit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTransferToRipple(string account, decimal targetamount, string currency, decimal sourceAmount, string paypassword)
        {
            account = account.NullSafe().Trim();

            if (Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account] != null && Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account] != null)
            {
                var rippleAccountInfo = (FederationResponse)Session[OUTSIDE_GATEWAY_ACCOUNT_INFO_KEY + account];
                var pathInfos = Session[OUTSIDE_GATEWAY_ACCOUNT_PATHS + account] as List<List<object>>;

                try
                {
                    var cmd = new CreateOutboundTransfer(PayWay.Ripple, rippleAccountInfo.UserAccount.Destination, currency, sourceAmount, targetamount, string.Empty, this.CurrentUser.UserID);
                    this.CommandBus.Send(cmd);

                    var cmd_ripple = new CreateRippleOutboundTx(rippleAccountInfo.UserAccount.Destination, rippleAccountInfo.UserAccount.DestinationTag, currency, targetamount, sourceAmount, pathInfos);
                    this.CommandBus.Send(cmd_ripple);
                    return View("transferripplesuccess");
                }
                catch (Exception ex)
                {
                    Log.Error("submit to ripple error", ex);
                }
            }

            return View("Error");
        }
        #endregion

        #endregion

        #endregion

        #region 私有方法

        #region 根据FederationUrl和账号查找对方网关用户的相关信息

        private async Task<FederationResponse> TryGetOutsideAccountInfo(string account, string domain, string federationUrl)
        {
            var httpclient = new HttpClient();
            var cacheKey = CacheKey.DOMAIN_RIPPLE_GATEWAY_USER + account + "@" + domain;
            federationUrl = federationUrl + "?type=federation&destination={0}&domain={1}".FormatWith(account, domain);

            var accountInfo = default(FederationResponse);

            if (!Cache.TryGet<FederationResponse>(cacheKey, out accountInfo))
            {
                var rep = await httpclient.GetAsync(federationUrl);
                var result = await rep.Content.ReadAsStringAsync();

                accountInfo = IoC.Resolve<IJsonSerializer>().Deserialize<FederationResponse>(result);

                if (accountInfo.Result == "success")
                {
                    Cache.Add(cacheKey, accountInfo, new TimeSpan(1, 0, 0, 0));
                }
                else
                {
                    Cache.Add(cacheKey, accountInfo, new TimeSpan(0, 1, 0));
                }
            }

            return accountInfo;
        }
        #endregion

        #region 获取对方网关的FederationUrl
        private async Task<Tuple<bool, string>> TryGetFederationUrl(string domain)
        {
            var ripple = "/ripple.txt";
            var protocol = "https://";
            var federationUrl = string.Empty;
            var domainCacheKey = CacheKey.DOMAIN_RIPPLE_FEDERATION + domain;
            federationUrl = Cache.Get<string>(domainCacheKey);
            var result = false;

            if (string.IsNullOrEmpty(federationUrl))
            {
                var httpclient = new HttpClient();
                var rep = await httpclient.GetAsync(string.Concat(protocol + domain + ripple));

                if (rep.StatusCode != HttpStatusCode.OK)
                {
                    rep = await httpclient.GetAsync(string.Concat(protocol + "www." + domain + ripple));
                }

                if (rep.StatusCode == HttpStatusCode.OK)
                {
                    var rippletxt = await rep.Content.ReadAsStringAsync();
                    result = true;
                    federationUrl = GetFederationUrlFromRippleTxt(rippletxt);

                    if (!string.IsNullOrWhiteSpace(federationUrl))
                    {
                        Cache.Add(domainCacheKey, federationUrl, new TimeSpan(1, 0, 0));
                    }
                }
            }
            else
            {
                result = true;
            }

            return new Tuple<bool, string>(result, federationUrl);
        }
        #endregion

        #region 根据ripple.txt内容，找出其中的FederationUrl
        private string GetFederationUrlFromRippleTxt(string rippletxt)
        {
            string federation_url = string.Empty;

            if (rippletxt.Contains("federation_url"))
            {
                var configs = rippletxt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var addressReg = new Regex(@"https://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");

                for (int itemIndex = 0; itemIndex < configs.Count(); itemIndex++)
                {
                    if (configs[itemIndex] == "[federation_url]")
                    {
                        while (itemIndex < configs.Count())
                        {
                            ++itemIndex;
                            if (configs[itemIndex].IndexOf("#") > -1)
                                continue;
                            else
                            {
                                if (addressReg.IsMatch(configs[itemIndex].Trim()))
                                {
                                    federation_url = configs[itemIndex].Trim();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return federation_url;
        }
        #endregion
        #endregion
    }
}
