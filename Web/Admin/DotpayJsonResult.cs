using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dotpay.Admin
{
    public class DotpayJsonResult
    {
        public DotpayJsonResult(int code, string message = "")
        {
            this.Code = code;
            this.Message = message;
        }
        public int Code { get; private set; }
        public string Message { get; private set; }

        public static DotpayJsonResult Success { get { return new DotpayJsonResult(1); } }
        public static DotpayJsonResult UnknowFail
        {
            get
            {
                return DotpayJsonResult.CreateFailResult("未知异常，请刷新页面后重试");
            }
        }
        public static DotpayJsonResult CreateSuccessResult(string message = "")
        {
            return new DotpayJsonResult(1, message);
        }

        public static DotpayJsonResult CreateFailResult(string message = "")
        {
            return new DotpayJsonResult(-1, message);
        }
    }
}