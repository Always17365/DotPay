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

namespace DotPay.Web.Controllers
{
    public class FCAPIController : ApiController
    {
        public readonly static string GATEWAY_ADDRESS = Config.GatewayAccount;
        public const string GATEWAY_DOMAIN = "dotpay.co";
        public const string GATEWAY_ACCEPT_CURRENCY = "CNY";
        public const string GATEWAY_SPLIT = ":";

        [HttpGet]
        [CROS]
        [Route("api/v1/bridge")]
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
                    destination = tppBridge.Bridge;
                    domain = tppBridge.Domain;
                    var payway = Utilities.GetPayway(tppBridge.Bridge);

                    if (payway != default(PayWay))
                    {
                        try
                        {
                            //创建一个交易
                            var cmd = new CreateThirdPartyPaymentInboundTx(payway, tppBridge.Account);
                            IoC.Resolve<ICommandBus>().Send(cmd);
                            var userInfo = new OutsideGatewayUserInfo
                            {
                                Type = "federation_record",
                                Destination = tppBridge.Account + GATEWAY_SPLIT + tppBridge.Bridge,
                                DestinationAddress = GATEWAY_ADDRESS,
                                DestinationTag = int.Parse(Utilities.ConvertPaywayFlg(payway).ToString() + cmd.Result.ToString()),
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
                }
                else
                {
                    var repos = IoC.Resolve<IUserQuery>();
                    var user = default(LoginUser);

                    if (destination.IsEmail())
                        user = repos.GetUserByEmail(destination);
                    else
                        user = repos.GetUserByLoginName(destination);

                    if (user == null)
                        result = FederationErrorResult.NoSuchUser(req);
                    else
                    {
                        var userInfo = new OutsideGatewayUserInfo
                        {
                            Type = "federation_record",
                            Destination = destination,
                            DestinationAddress = GATEWAY_ADDRESS,
                            DestinationTag = int.Parse(Utilities.ConvertPaywayFlg(PayWay.Ripple).ToString() + user.UserID.ToString()),
                            Domain = domain,
                            AcceptCurrencys = new List<RippleCurrency> { new RippleCurrency { Issuer = GATEWAY_ADDRESS, Symbol = GATEWAY_ACCEPT_CURRENCY } }
                        };
                        result = FederationErrorResult.Success(req, userInfo);

                    }
                }
            }
            return Json(result);
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

            public static FederationResponse Success(FederationRequest req, OutsideGatewayUserInfo userInfo)
            {
                return new FederationResponse { Result = "success", UserAccount = userInfo, OriginRequest = req };
            }
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
                    var strs = destinationWithDomain.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

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