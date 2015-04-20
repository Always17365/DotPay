using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dotpay.Language;

namespace Dotpay.Front
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
                return DotpayJsonResult.CreateFailResult(LangHelpers.Lang("Unkown Exception,Please refresh page and retry."));
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
        public static DotpayJsonResult CreateFailResult(int code,string message = "")
        {
            return new DotpayJsonResult(code, message);
        }
    }
}