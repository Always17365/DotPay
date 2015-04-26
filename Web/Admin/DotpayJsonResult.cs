using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dotpay.Admin
{
    public class DotpayJsonResult
    {
        public DotpayJsonResult(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
        public int Code { get; protected set; }
        public string Message { get; protected set; }

        public static DotpayJsonResult Success { get { return new DotpayJsonResult(1, ""); } }
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

    public class DotpayJsonResult<T> : DotpayJsonResult
    {
        public DotpayJsonResult(int code, string message, T data)
            : base(code, message)
        {
            this.Data = data;
        }
        public T Data { get; private set; }

        public static DotpayJsonResult<T> CreateSuccessResult(T data, string message = "")
        {
            return new DotpayJsonResult<T>(1, message, data);
        }

        public static DotpayJsonResult<T> CreateFailResult(T data, string message = "")
        {
            return new DotpayJsonResult<T>(-1, message, data);
        }
    }
}