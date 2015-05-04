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

        public static DotpayJsonResult Success
        {
            get { return new DotpayJsonResult(1); }
        }

        public static DotpayJsonResult SystemError
        {
            get { return new DotpayJsonResult(-100, LangHelpers.Lang("systemProcessError")); }
        }

        //public static DotpayJsonResult SystemError
        //{
        //    get
        //    {
        //        return
        //            DotpayJsonResult.CreateFailResult(LangHelpers.Lang("Unkown Exception,Please refresh page and retry."));
        //    }
        //}

        public static DotpayJsonResult CreateSuccessResult(string message = "")
        {
            return new DotpayJsonResult(1, message);
        }

        public static DotpayJsonResult CreateFailResult(string message = "")
        {
            return new DotpayJsonResult(-1, message);
        }

        public static DotpayJsonResult CreateFailResult(int code, string message = "")
        {
            return new DotpayJsonResult(code, message);
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