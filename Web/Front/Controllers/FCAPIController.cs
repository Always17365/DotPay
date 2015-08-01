using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FC.Framework;
using DotPay.QueryService;
using System.Threading.Tasks;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.RippleCommand;
using DotPay.Web.Filters;
using System.Security.Cryptography;
using System.Text;

namespace DotPay.Web.Controllers
{
    public class FCAPIController : ApiController
    {
        private readonly static string GATEWAY_ADDRESS = Config.GatewayAccount;
        private const string GATEWAY_DOMAIN = "dotpay.co";
        private const string GATEWAY_ACCEPT_CURRENCY = "CNY";
        private const string GATEWAY_SPLIT = ":";
        private const string QUOTE_URL = "https://www.dotpay.co/api/v1/quote";
        private const string QUERY_URL = "https://www.dotpay.co/query";
        private const decimal minAcceptAmount = 1M;
        private const decimal maxAcceptAmount = 20000M;

        #region Federation
        [HttpGet]
        [CROS]
        [Route("~/api/v1/bridge")]
        public System.Web.Http.Results.JsonResult<FederationResponse> RippleBridge(string type, string destination, string domain)
        {
            var result = default(FederationResponse);

            var req = new FederationRequest { Type = type, Destination = destination, Domain = domain };

            type = type.NullSafe().Trim();
            destination = destination.NullSafe().Trim();
            domain = domain.NullSafe().Trim();

            #region 错误处理
            if (!type.ToLower().Equals("federation"))
                result = FederationErrorResult.NoSupportedType(req, type);
            else if (!CheckDomain(domain))
                result = FederationErrorResult.NoSuchDomain(req);
            else if (string.IsNullOrEmpty(destination))
                result = FederationErrorResult.NoSuchUser(req);

            #endregion
            else
            {
                var tppBridge = default(ThirdPartyPaymentBridgeDestination);

                if (ThirdPartyPaymentBridgeDestination.TryParse(destination + "@" + domain, out tppBridge))
                {
                    #region 如果是第三方支付直转
                    destination = tppBridge.Bridge;
                    domain = tppBridge.Domain;
                    var tagFlg = Utilities.GetDestinationTagFlgByBridgeName(tppBridge.Bridge);
                    var payway = Utilities.GetPayWayByBridgeName(tppBridge.Bridge);

                    if (payway != default(PayWay))
                    {
                        try
                        {
                            //创建一个交易
                            var cmd = new CreateThirdPartyPaymentInboundTx(payway, tppBridge.Account);
                            IoC.Resolve<ICommandBus>().Send(cmd);
                            var userInfo = new OutsideGatewayFederationInfo
                            {
                                Type = "federation_record",
                                Destination = tppBridge.Account + GATEWAY_SPLIT + tppBridge.Bridge,
                                DestinationAddress = GATEWAY_ADDRESS,
                                DestinationTag = int.Parse(tagFlg.ToString("D") + cmd.ResultDestinationTag.ToString()),
                                Domain = domain,
                                AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                            };
                            result = FederationErrorResult.Success(req, userInfo);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("在ripple federation protocol解析时出现异常" + destination + "@" + domain, ex);

                            result = FederationErrorResult.NoSuchUser(req);
                        }
                    }
                    else
                    {
                        result = FederationErrorResult.NoSuchUser(req);
                    }
                    #endregion
                }
                else
                {
                    #region 如果是普通用户或者是第三方支付表单方式
                    var repos = IoC.Resolve<IUserQuery>();
                    var user = default(LoginUser);
                    //如果用户要做扩展表单直转 to alipay
                    if (destination.Equals("alipay", StringComparison.OrdinalIgnoreCase))
                    {
                        var federationInfo = new OutsideGatewayFederationInfo
                        {
                            Type = "federation_record",
                            Destination = destination,
                            ExtraFields = new List<ExtraFiled>
                            {
                                new ExtraFiled{Type="text", Hint="支付宝账户" ,Label="Destination alipay account",Required=true, Name="alipay_account"},
                                new ExtraFiled{Type="text", Hint="支付宝账户实名（可选)" ,Label="Real name of the destination alipay account (optional)", Name="alipay_username"}, 
                                new ExtraFiled{Type="text", Hint="留言(可选)，可填联系方式，收货地址等信息，您可在支付完成后在 "+QUERY_URL+"查询交易状态",Label="Comments(optional), for contacts, shipping address etc., you can check the transaction status at "+QUERY_URL+" after confirming the payment",Name="memo"}
                            },
                            Domain = domain,
                            QuoteUrl = QUOTE_URL,
                            AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                        };
                        result = FederationErrorResult.Success(req, federationInfo);
                    }
                    //如果用户要做扩展表单直转 to tenpay
                    else if (destination.Equals("tenpay", StringComparison.OrdinalIgnoreCase))
                    {
                        var federationInfo = new OutsideGatewayFederationInfo
                        {
                            Type = "federation_record",
                            Destination = destination,
                            ExtraFields = new List<ExtraFiled>
                            {
                                new ExtraFiled{Type="text", Hint="财付通账户" ,Label="Destination alipay account",Required=true, Name="alipay_account"},
                                new ExtraFiled{Type="text", Hint="财付通账户实名（可选)" ,Label="Real name of the destination alipay account (optional)",  Name="tenpay_username"}, 
                                new ExtraFiled{Type="text", Hint="留言(可选)，可填联系方式，收货地址等信息，您可在支付完成后在 "+QUERY_URL+"查询交易状态",Label="Comments(optional), for contacts, shipping address etc., you can check the transaction status at "+QUERY_URL+" after confirming the payment",Name="memo"}
                          },
                            Domain = domain,
                            QuoteUrl = QUOTE_URL,
                            AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                        };
                        result = FederationErrorResult.Success(req, federationInfo);
                    }
                    //如果用户要做扩展表单直转 to bank
                    else if (destination.Equals("bank", StringComparison.OrdinalIgnoreCase))
                    {
                        var bank = new List<ExtraSelectFiledOption>();
                        var federationInfo = new OutsideGatewayFederationInfo
                        {
                            Type = "federation_record",
                            Destination = destination,
                            ExtraFields = new List<ExtraFiled>
                            {
                                new ExtraFiled{
                                    Type="select", 
                                    Hint="开户行(如果列表没有所需银行名称，请选择其它银行，并在留言中填写银行名称)" ,
                                    Label="Bank Name(if the bank is not listed, please select \"Other Bank\" and give the bank name in comments below.)",
                                    Required=true, 
                                    Name="bank",
                                    Options=GetBankFiledsList()
                                },
                                new ExtraFiled{Type="text", Hint="银行卡号",Label="Bank Account Number",Required=true,Name="bank_account"},
                                new ExtraFiled{Type="text", Hint="开户人姓名",Label="Account Name",Required=true,Name="bank_username"},
                                new ExtraFiled{Type="text", Hint="留言(可选)，可填联系方式，收货地址等信息，您可在支付完成后在 "+QUERY_URL+"查询交易状态",Label="Comments(optional), for contacts, shipping address etc., you can check the transaction status at "+QUERY_URL+" after confirming the payment",Name="memo"}
                        },
                            Domain = domain,
                            QuoteUrl = QUOTE_URL,
                            AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                        };
                        result = FederationErrorResult.Success(req, federationInfo);
                    }
                    //如果用户要做扩展表单来支付324活动
                    else if (destination.Equals("huodong", StringComparison.OrdinalIgnoreCase))
                    {
                        var bank = new List<ExtraSelectFiledOption>();
                        var federationInfo = new OutsideGatewayFederationInfo
                        {
                            Type = "federation_record",
                            Destination = destination,
                            ExtraFields = new List<ExtraFiled>
                            { 
                                new ExtraFiled{Type="text", Hint="参与人姓名",Label="Full Name",Required=true,Name="alipay_username"},
                                new ExtraFiled{Type="text", Hint="手机号",Label="Mobile",Required=true,Name="alipay_account"},
                                new ExtraFiled{Type="text", Hint="留言(可选)",Label="Comments(optional)",Name="memo"}
                            },
                            Domain = domain,
                            QuoteUrl = QUOTE_URL,
                            AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                        };
                        result = FederationErrorResult.Success(req, federationInfo);
                    }
                    else
                    {
                        if (destination.IsEmail())
                            user = repos.GetUserByEmail(destination);
                        else
                            user = repos.GetUserByLoginName(destination);

                        if (user == null)
                            result = FederationErrorResult.NoSuchUser(req);
                        else
                        {
                            var userInfo = new OutsideGatewayFederationInfo
                            {
                                Type = "federation_record",
                                Destination = destination,
                                DestinationAddress = GATEWAY_ADDRESS,
                                DestinationTag = int.Parse(DestinationTagFlg.Dotpay.ToString("D") + user.UserID.ToString()),
                                Domain = domain,
                                AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                            };
                            result = FederationErrorResult.Success(req, userInfo);

                        }
                    }
                    #endregion
                }
            }
            return Json(result);
        }
        #endregion

        #region Quote
        [HttpGet]
        [CROS]
        [Route("~/api/v1/quote")]
        public System.Web.Http.Results.JsonResult<QuoteResponse> RippleQuote(string type)
        {
            var result = default(QuoteResponse);
            var query_params = this.Request.GetQueryNameValuePairs();

            var amount = query_params.SingleOrDefault(item => item.Key.Equals("amount", StringComparison.OrdinalIgnoreCase)).Value;
            var destination = query_params.SingleOrDefault(item => item.Key.Equals("destination", StringComparison.OrdinalIgnoreCase)).Value;
            var address = query_params.SingleOrDefault(item => item.Key.Equals("address", StringComparison.OrdinalIgnoreCase)).Value;
            var alipay_account = query_params.SingleOrDefault(item => item.Key.Equals("alipay_account", StringComparison.OrdinalIgnoreCase)).Value;
            var alipay_username = query_params.SingleOrDefault(item => item.Key.Equals("alipay_username", StringComparison.OrdinalIgnoreCase)).Value;
            var tenpay_account = query_params.SingleOrDefault(item => item.Key.Equals("tenpay_account", StringComparison.OrdinalIgnoreCase)).Value;
            var tenpay_username = query_params.SingleOrDefault(item => item.Key.Equals("tenpay_username", StringComparison.OrdinalIgnoreCase)).Value;
            var bank_account = query_params.SingleOrDefault(item => item.Key.Equals("bank_account", StringComparison.OrdinalIgnoreCase)).Value;
            var bank_username = query_params.SingleOrDefault(item => item.Key.Equals("bank_username", StringComparison.OrdinalIgnoreCase)).Value;
            var bank = query_params.SingleOrDefault(item => item.Key.Equals("bank", StringComparison.OrdinalIgnoreCase)).Value;
            var memo = query_params.SingleOrDefault(item => item.Key.Equals("memo", StringComparison.OrdinalIgnoreCase)).Value.NullSafe().Trim();
            memo = memo.Length > 250 ? memo.Substring(0, 250) : memo;
            var req = new QuoteRequest { Type = type, __dot_use_this_amount = amount, Address = address, Destination = destination, AlipayAccount = alipay_account, TenpayAccount = tenpay_account, Memo = memo };

            type = type.NullSafe().Trim();
            destination = destination.NullSafe().Trim();

            #region 错误处理
            if (!type.ToLower().Equals("quote"))
                result = QuoteResult.NoSupportedType(req, type);
            else if (string.IsNullOrEmpty(destination))
                result = QuoteResult.ErrorDetail(req, "destination is empty");

            #endregion
            else
            {
                var sendAmount = req.Amount.Value + CalcFee(req.Amount.Value);

                //如果用户要做扩展表单直转 to alipay,且支付宝账号不为空
                if (destination.Equals("alipay", StringComparison.OrdinalIgnoreCase) && req.Amount.Value <= maxAcceptAmount && req.Amount.Value >= minAcceptAmount)
                {
                    if (string.IsNullOrEmpty(alipay_account))
                        result = QuoteResult.ErrorDetail(req, "alipay account empty;");
                    else
                    {
                        //创建一个交易
                        var cmd = new CreateThirdPartyPaymentInboundTx(PayWay.Alipay, alipay_account, alipay_username.NullSafe().Trim(), req.Amount.Value, sendAmount, memo);

                        try
                        {
                            IoC.Resolve<ICommandBus>().Send(cmd);
                            var quoteInfo = new QuoteInfo
                            {
                                Type = "quote",
                                Destination = destination,
                                DestinationAddress = GATEWAY_ADDRESS,
                                Domain = GATEWAY_DOMAIN,
                                DestinationTag = Convert.ToInt32(DestinationTagFlg.AlipayRippleForm.ToString("D") + cmd.ResultDestinationTag.ToString()),
                                Amount = req.Amount.Value,
                                Send = new List<RippleAmount> { new RippleAmount(sendAmount, GATEWAY_ADDRESS, req.Amount.Currency) },
                                InvoiceId = cmd.ResultInvoiceID,
                                Source = req.Address
                            };
                            result = QuoteResult.Success(req, quoteInfo);
                        }
                        catch (Exception ex) { Log.Error("在执行Ripple Alipay Quote时报错", ex); }
                    }
                }

                //如果用户要做扩展表单直转 to huodong, 
                else if (destination.Equals("huodong", StringComparison.OrdinalIgnoreCase) && req.Amount.Value <= maxAcceptAmount && req.Amount.Value >= minAcceptAmount)
                {
                    if (string.IsNullOrEmpty(alipay_account))
                        result = QuoteResult.ErrorDetail(req, "huodong account empty;");
                    else
                    {
                        var huodongAmount = req.Amount.Value;
                        //创建一个交易
                        var cmd = new CreateThirdPartyPaymentInboundTx(PayWay.Alipay, alipay_account, alipay_username.NullSafe().Trim(), huodongAmount, huodongAmount, "324活动" +
                            (string.IsNullOrEmpty(memo) ? "" : ":" + memo));

                        try
                        {
                            IoC.Resolve<ICommandBus>().Send(cmd);
                            var quoteInfo = new QuoteInfo
                            {
                                Type = "quote",
                                Destination = destination,
                                DestinationAddress = GATEWAY_ADDRESS,
                                Domain = GATEWAY_DOMAIN,
                                DestinationTag = Convert.ToInt32(DestinationTagFlg.AlipayRippleForm.ToString("D") + cmd.ResultDestinationTag.ToString()),
                                Amount = huodongAmount,
                                Send = new List<RippleAmount> { new RippleAmount(huodongAmount, GATEWAY_ADDRESS, req.Amount.Currency) },
                                InvoiceId = cmd.ResultInvoiceID,
                                Source = req.Address
                            };
                            result = QuoteResult.Success(req, quoteInfo);
                        }
                        catch (Exception ex) { Log.Error("在执行Ripple 活动 Quote时报错", ex); }
                    }
                }
                //如果用户要做扩展表单直转 to tenpay
                else if (destination.Equals("tenpay", StringComparison.OrdinalIgnoreCase) && req.Amount.Value <= maxAcceptAmount && req.Amount.Value >= minAcceptAmount)
                {
                    if (string.IsNullOrEmpty(tenpay_account))
                        result = QuoteResult.ErrorDetail(req, "tenpay account empty;");
                    else
                    {
                        var cmd = new CreateThirdPartyPaymentInboundTx(PayWay.Tenpay, tenpay_account, tenpay_username.NullSafe().Trim(), req.Amount.Value, sendAmount, memo);

                        try
                        {
                            IoC.Resolve<ICommandBus>().Send(cmd);

                            var quoteInfo = new QuoteInfo
                            {
                                Type = "quote",
                                Destination = destination,
                                DestinationAddress = GATEWAY_ADDRESS,
                                Domain = GATEWAY_DOMAIN,
                                DestinationTag = Convert.ToInt32(DestinationTagFlg.TenpayRippleForm.ToString("D") + cmd.ResultDestinationTag.ToString()),
                                Amount = req.Amount.Value,
                                Send = new List<RippleAmount> { new RippleAmount(sendAmount, GATEWAY_ADDRESS, req.Amount.Currency) },
                                InvoiceId = cmd.ResultInvoiceID,
                                Source = req.Address
                            };
                            result = QuoteResult.Success(req, quoteInfo);
                        }
                        catch (Exception ex) { Log.Error("在执行Ripple Tenpay Quote时报错", ex); }
                    }
                }
                //如果用户要做扩展表单直转 to bank
                else if (destination.Equals("bank", StringComparison.OrdinalIgnoreCase) && req.Amount.Value <= maxAcceptAmount && req.Amount.Value >= minAcceptAmount)
                {
                    PayWay tobank = default(PayWay);

                    if (string.IsNullOrEmpty(bank_username))
                        result = QuoteResult.ErrorDetail(req, "User name of bank account empty;");
                    else if (string.IsNullOrEmpty(bank_account))
                        result = QuoteResult.ErrorDetail(req, "Bank account empty;");
                    else if (Enum.TryParse<PayWay>(bank, out tobank) && !GetBankList().Contains(tobank))
                    {
                        result = QuoteResult.ErrorDetail(req, "not support this bank.");
                    }
                    else if (tobank == PayWay.OtherBank && string.IsNullOrEmpty(memo))
                    {
                        result = QuoteResult.ErrorDetail(req, "please tell us the bank name in [Comments]");
                    }
                    else
                    {
                        var cmd = new CreateThirdPartyPaymentInboundTx(tobank, bank_account, bank_username, req.Amount.Value, sendAmount, memo.NullSafe().Trim());

                        try
                        {
                            IoC.Resolve<ICommandBus>().Send(cmd);
                            var quoteInfo = new QuoteInfo
                            {
                                Type = "quote",
                                Destination = destination,
                                DestinationAddress = GATEWAY_ADDRESS,
                                Domain = GATEWAY_DOMAIN,
                                Amount = req.Amount.Value,
                                DestinationTag = Convert.ToInt32(DestinationTagFlg.BankRippleForm.ToString("D") + cmd.ResultDestinationTag.ToString()),
                                Send = new List<RippleAmount> { new RippleAmount(sendAmount, GATEWAY_ADDRESS, req.Amount.Currency) },
                                InvoiceId = cmd.ResultInvoiceID,
                                Source = req.Address
                            };
                            result = QuoteResult.Success(req, quoteInfo);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("在执行Ripple Tenpay Quote时报错", ex);
                        }
                    }
                }
                else
                {
                    if (req.Amount.Value < minAcceptAmount)
                        result = QuoteResult.ErrorDetail(req, "amount don't less than  " + minAcceptAmount);
                    else if (req.Amount.Value > maxAcceptAmount)
                        result = QuoteResult.ErrorDetail(req, "amount don't greate than  " + maxAcceptAmount);
                    else
                        result = QuoteResult.ErrorDetail(req, "data invlid");

                }

            }
            return Json(result);
        }


        #endregion

        private decimal CalcFee(decimal amount)
        {
            var minFee = 5;
            var feeRate = 5 / 1000M;

            var result = Math.Max(amount * feeRate, minFee);

            return result;
        }

        #region 检查域名是否合格
        private bool CheckDomain(string domain)
        {
            var result = false;

            if (!string.IsNullOrEmpty(domain))
            {
                if (domain.Contains("@") && domain.Count(s => s == '@') == 1)
                {
                    var strs = domain.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs != null && strs.Count() == 2)
                    {
                        if (strs[1].Equals(GATEWAY_DOMAIN, StringComparison.OrdinalIgnoreCase))
                        {
                            result = true;
                        }
                    }
                }
                else if (domain.Equals(GATEWAY_DOMAIN, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region 数据类
        public class FederationErrorResult
        {
            public static FederationResponse NoSuchUser(FederationRequest req)
            {
                return new FederationResponse { Result = "error", Error = "noSuchUser", ErrorMessage = "The supplied user was not found.", OriginRequest = req };
            }
            public static FederationResponse NoSuchDomain(FederationRequest req)
            {
                return new FederationResponse { Result = "error", Error = "noSuchDomain", ErrorMessage = "The supplied domain is not served here.", OriginRequest = req };
            }
            public static FederationResponse NoSupportedType(FederationRequest req, string type)
            {
                return new FederationResponse { Result = "error", Error = "noSupported", ErrorMessage = "not support request type:" + type, OriginRequest = req };
            }

            public static FederationResponse Success(FederationRequest req, OutsideGatewayFederationInfo userInfo)
            {
                return new FederationResponse { Result = "success", UserAccount = userInfo, OriginRequest = req };
            }
        }

        public class QuoteResult
        {
            public static QuoteResponse NoSupportedType(QuoteRequest req, string type)
            {
                return new QuoteResponse { Result = "error", Error = "noSupported", ErrorMessage = "not support request type:" + type, OriginRequest = req };
            }

            public static QuoteResponse ErrorDetail(QuoteRequest req, string message)
            {
                return new QuoteResponse { Result = "error", Error = "noSupported", ErrorMessage = message, OriginRequest = req };
            }

            public static QuoteResponse Success(QuoteRequest req, QuoteInfo quoteInfo)
            {
                return new QuoteResponse { Result = "success", QuoteInfo = quoteInfo, OriginRequest = req };
            }
        }
        private IEnumerable<ExtraSelectFiledOption> GetBankFiledsList()
        {
            var banks = new List<ExtraSelectFiledOption>();

            var enumValus = Enum.GetValues(typeof(PayWay));
            var filterVals = new List<PayWay> { PayWay.Alipay, PayWay.Tenpay, PayWay.VirutalTransfer, PayWay.Ripple, PayWay.Inside, PayWay.Bank };

            foreach (var val in enumValus)
            {
                var payway = (PayWay)val;
                if (!filterVals.Contains(payway))
                {
                    banks.Add(new ExtraSelectFiledOption(payway.ToString("F").PadRight(8, ' ') + payway.GetDescription(), payway.ToString("F")));
                }
            }

            return banks;
        }

        private IEnumerable<PayWay> GetBankList()
        {
            var banks = new List<PayWay>();

            var enumValus = Enum.GetValues(typeof(PayWay));
            var filterVals = new List<PayWay> { PayWay.Alipay, PayWay.Tenpay, PayWay.VirutalTransfer, PayWay.Ripple, PayWay.Inside, PayWay.Bank };

            foreach (var val in enumValus)
            {
                var payway = (PayWay)val;
                if (!filterVals.Contains(payway))
                {
                    banks.Add(payway);
                }
            }

            return banks;
        }
        public class ThirdPartyPaymentBridgeDestination
        {
            public string Account { get; set; }
            public string Bridge { get; set; }
            public string Domain { get; set; }

            public static bool TryParse(string destinationWithDomain, out ThirdPartyPaymentBridgeDestination result)
            {
                var success = false;
                result = null;

                if (destinationWithDomain.Contains(GATEWAY_SPLIT))
                {
                    var strs = destinationWithDomain.Split(GATEWAY_SPLIT.ToArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (strs.Length == 2)
                    {
                        var tppAccount = strs[0];
                        var bridgeWithDomain = strs[1];
                        var bridge = string.Empty;
                        var domain = string.Empty;

                        var strs2 = bridgeWithDomain.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                        if (strs2.Length == 2)
                        {
                            bridge = strs2[0];
                            domain = strs2[1];
                        }

                        if (!string.IsNullOrEmpty(tppAccount) &&
                            !string.IsNullOrEmpty(bridge) &&
                            !string.IsNullOrEmpty(domain) &&
                            domain.Equals(GATEWAY_DOMAIN, StringComparison.OrdinalIgnoreCase))
                        {
                            success = true;
                            result = new ThirdPartyPaymentBridgeDestination { Account = tppAccount, Bridge = bridge, Domain = domain };
                        }
                    }
                }

                return success;
            }

        }
        #endregion
    }
}