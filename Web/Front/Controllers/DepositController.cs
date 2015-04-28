using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DFramework;
using Dotpay.Command;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Dotpay.Front.Validators;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dotpay.Front.YiSheng;

namespace Dotpay.Front.Controllers
{
    public class DepositController : BaseController
    {
        [Route("~/deposit")]
        public ActionResult Index()
        {
            return View();
        }
        #region 易生支付充值接口

        #region 易生充值跳转
        [HttpGet]
        [Route("~/deposit/epredirect")]
        public ActionResult YiShengRedirect(int? errorCode, string bank, string orderBizNo, decimal amount)
        {
            if (errorCode.HasValue || string.IsNullOrEmpty(bank) || string.IsNullOrEmpty(orderBizNo) || amount <= 0)
            {
                ViewBag.ErrorMessage = DotpayJsonResult.UnknowFail.Message;
            }
            else
            {
                var con = new EasypayConfig();
                var partner = con.Partner;
                var key = con.Key;
                var sellerEmail = con.SellerEmail;
                var inputCharset = con.InputCharset;
                var notifyUrl = con.NotifyUrl;
                var returnUrl = con.ReturnUrl;
                var signType = con.SignType;
                //必填参数
                var outTradeNo = orderBizNo;                                  //请与贵网站订单系统中的唯一订单号匹配
                var subject = "点付充值";                                     //订单名称，显示在易生支付收银台里的“商品名称”里。
                var body = "Dotpayco";                                        //订单描述、订单详细、订单备注，显示在易生支付收银台里的“商品描述”里
                var totalFee = amount.ToString("0.##");                       //订单总金额，显示在易生支付收银台里的“应付总额”里


                var paymethod = "bankDirect";                                 //默认支付方式，三个值可选：bankPay(网银); directPay(账户余额); bankDirect(银行直联) 
                var buyerEmail = "";
                //构造请求函数，无需修改
                var easyService = new EasypayService(partner, sellerEmail, returnUrl, notifyUrl, outTradeNo, subject, body,
                                                     totalFee, paymethod, bank, buyerEmail, key, inputCharset, signType);
                ViewBag.FromHtml = easyService.BuildForm();
            }
            return View();
        }
        #endregion

        #region 易生Callback
        #region easy pay return
        [HttpGet]
        [Route("~/deposit/epreturn")]
        public async Task<ActionResult> EasypayReturn()
        {
            var @params = new SortedDictionary<string, string>();
             
            ViewBag.Result = DotpayJsonResult.CreateFailResult(this.Lang("easypayInvlidResult"));
            foreach (KeyValuePair<string, object> item in Request.Form)
            {
                @params.Add(item.Key, item.Value.ToString());
            }
            foreach (KeyValuePair<string, object> item in Request.QueryString)
            {
                @params.Add(item.Key, item.Value.ToString());
            }

            EasypayConfig con = new EasypayConfig();
            string partner = con.Partner;
            string key = con.Key;
            string input_charset = con.InputCharset;
            string sign_type = con.SignType;
            string transport = con.Transport;

            if (@params.Count > 0)
            {
                EasypayNotify easyNotify = new EasypayNotify(@params, @params["notify_id"], partner, key, input_charset, sign_type, transport);
                string responseTxt = easyNotify.ResponseTxt; //获取远程服务器ATN结果，验证是否是易生支付服务器发来的请求
                string sign = @params["notify_id"];          //获取易生支付反馈回来的sign结果
                string mysign = easyNotify.Mysign;           //获取通知返回后计算后（验证）的签名结果

                //写日志记录（若要调试，请取消下面两行注释）
                string sWord = "easy pay return:responseTxt=" + responseTxt + "\n return_url_log:sign=" + Request.Params["sign"] + "&mysign=" + mysign + "\n return回来的参数：" + easyNotify.PreSignStr;
                Log.Debug(sWord);

                //判断responsetTxt是否为true，生成的签名结果mysign与获得的签名结果sign是否一致
                //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
                //mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
                //判断responsetTxt是否为ture，生成的签名结果mysign与获得的签名结果sign是否一致
                //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
                //mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关

                if (responseTxt == "true" && sign == mysign)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取易生支付的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                    var trade_no = Request.Params["trade_no"];      //易生支付交易号
                    var order_no = Request.Params["out_trade_no"];	//获取订单号
                    var totalAmount = Convert.ToDecimal(Request.Params["total_fee"]);	//获取总金额
                    //var subject = Request.Params["subject"];        //商品名称、订单名称
                    //var body = Request.Params["body"];              //商品描述、订单备注、描述
                    //var buyer_email = Request.Params["buyer_email"];//买家易生支付买家账号
                    var trade_status = Request.Params["trade_status"];//交易状态

                    //如果交易完成
                    if (trade_status == "TRADE_FINISHED")
                    {
                        //查询我们的订单号对应的充值记录，得到txid后执行cmd
                        //执行cmd前
                        Guid txid = new Guid();
                        try
                        {
                            var cmd = new ConfirmDepositCommand(txid, trade_no, totalAmount);
                            await this.CommandBus.SendAsync(cmd);

                            if (cmd.CommandResult == ErrorCode.None)
                            {
                                ViewBag.Result = DotpayJsonResult.CreateSuccessResult(this.Lang("depositSuccessNotice", totalAmount.ToString("0.##")));
                            }
                            else if (cmd.CommandResult == ErrorCode.DepositAmountNotMatch)
                            {
                                Log.Debug("easy pay订单金额不匹配失败了" + order_no + trade_status);
                                ViewBag.Result = DotpayJsonResult.CreateFailResult("order amount not match.");
                                //充值的金额不匹配
                                //ViewBag.Success = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Result = DotpayJsonResult.SystemError;
                            Log.Debug("订单失败了" + order_no + trade_status);
                        }
                    }
                    else
                    {
                        ViewBag.Result = DotpayJsonResult.SystemError;
                        //处理支付失败的订单
                        Log.Debug("订单失败了" + order_no + trade_status);
                    }
                }
                else
                {
                    Log.Debug("易生支付返回结果验证失败"); 
                }
            }
            else
            {
                Log.Debug("易生支付返回参数无效");
            }
            return View("DepositResult");
        }
        #endregion

        #region easy pay return
        [HttpGet]
        [Route("~/deposit/epnotice")]
        public async Task<ActionResult> EasypayNotice()
        {
            var @params = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, object> item in Request.Form)
            {
                @params.Add(item.Key, item.Value.ToString());
            }
            foreach (KeyValuePair<string, object> item in Request.QueryString)
            {
                @params.Add(item.Key, item.Value.ToString());
            }

            EasypayConfig con = new EasypayConfig();
            string partner = con.Partner;
            string key = con.Key;
            string input_charset = con.InputCharset;
            string sign_type = con.SignType;
            string transport = con.Transport;

            if (@params.Count > 0)
            {
                EasypayNotify easyNotify = new EasypayNotify(@params, @params["notify_id"], partner, key, input_charset, sign_type, transport);
                string responseTxt = easyNotify.ResponseTxt; //获取远程服务器ATN结果，验证是否是易生支付服务器发来的请求
                string sign = @params["notify_id"];          //获取易生支付反馈回来的sign结果
                string mysign = easyNotify.Mysign;           //获取通知返回后计算后（验证）的签名结果

                //写日志记录（若要调试，请取消下面两行注释）
                string sWord = "easy pay notice:responseTxt=" + responseTxt + "\n return_url_log:sign=" + Request.Params["sign"] + "&mysign=" + mysign + "\n return回来的参数：" + easyNotify.PreSignStr;
                Log.Debug(sWord);

                //判断responsetTxt是否为true，生成的签名结果mysign与获得的签名结果sign是否一致
                //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
                //mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
                //判断responsetTxt是否为ture，生成的签名结果mysign与获得的签名结果sign是否一致
                //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
                //mysign与sign不等，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关

                if (responseTxt == "true" && sign == mysign)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取易生支付的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                    var trade_no = Request.Params["trade_no"];      //易生支付交易号
                    var order_no = Request.Params["out_trade_no"];	//获取订单号
                    var totalAmount = Convert.ToDecimal(Request.Params["total_fee"]);	//获取总金额
                    //var subject = Request.Params["subject"];        //商品名称、订单名称
                    //var body = Request.Params["body"];              //商品描述、订单备注、描述
                    //var buyer_email = Request.Params["buyer_email"];//买家易生支付买家账号
                    var trade_status = Request.Params["trade_status"];//交易状态

                    //如果交易完成
                    if (trade_status == "TRADE_FINISHED")
                    {
                        //查询我们的订单号对应的充值记录，得到txid后执行cmd
                        //执行cmd前
                        Guid txid = new Guid();
                        try
                        {
                            //无需担心重复执行cmd的问题，后端做了幂等，重复处理不影响最终结果
                            var cmd = new ConfirmDepositCommand(txid, trade_no, totalAmount);
                            await this.CommandBus.SendAsync(cmd);

                            if (cmd.CommandResult == ErrorCode.None)
                            {
                                //成功了
                            }
                            else if (cmd.CommandResult == ErrorCode.DepositAmountNotMatch)
                            {
                                Log.Debug("easy pay订单金额不匹配失败了" + order_no + trade_status);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("订单失败了" + order_no + trade_status);
                        }
                    }
                    else
                    {   //处理支付失败的订单
                        Log.Debug("订单失败了" + order_no + trade_status);
                    }

                    return Content("success");
                }
                else
                {
                    ViewBag.Text = "验证失败";
                    return Content("fail");
                }
            }
            else
            {
                Log.Debug("易生支付返回参数无效");
                return Content("none");
            }

        }
        #endregion
        #endregion

        #region 提交在线支付接口
        //可作为通用的，跳转到什么支付，以后都可以扩展
        [HttpPost]
        [Route("~/deposit/epsubmit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> YiShengDeposit(string bank, decimal amount)
        {
            try
            {
                var cmd = new CreateDepositCommand(this.CurrentUser.AccountId, CurrencyType.Cny, amount.ToFixed(2), Payway.EasyPay);

                await this.CommandBus.SendAsync(cmd);

                if (cmd.CommandResult.Item1 == ErrorCode.None)
                {
                    return Redirect("~/deposit/epredirect?bank=" + bank + "&orderBizNo=" + cmd.CommandResult.Item2 + "&amount=" + amount);
                }
                else if (cmd.CommandResult.Item1 == ErrorCode.InvalidAccount)
                {
                    return Redirect("~/deposit/epredirect?error=1");
                }
                else
                    return Redirect("~/deposit/epredirect?error=2");
            }
            catch (Exception ex)
            {
                Log.Error("YiShengDeposit Exception", ex);
                return Redirect("~/deposit/epredirect?error=2");
            }
        }
        #endregion
        #endregion
    }
}