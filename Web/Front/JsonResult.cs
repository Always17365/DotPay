using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotPay.Web
{
    public class FCJsonResult
    {
        public FCJsonResult(int code, string message = "")
        {
            this.Code = code;
            this.Message = message;
        }
        public int Code { get; private set; }
        public string Message { get; private set; }

        public static FCJsonResult Success { get { return new FCJsonResult(1); } }
        public static FCJsonResult UnknowFail
        {
            get
            {
                return FCJsonResult.CreateFailResult(Language.LangHelpers.Lang("Unknow Exception,Please refresh the page and try again"));
            }
        }
        public static FCJsonResult CreateSuccessResult(string message = "")
        {
            return new FCJsonResult(1, message);
        }

        public static FCJsonResult CreateFailResult(string message = "")
        {
            return new FCJsonResult(-1, message);
        }
    }
}