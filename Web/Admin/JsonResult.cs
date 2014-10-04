using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotPay.Web.Admin
{
    public class JsonResult
    {
        public JsonResult(int code)
        {
            this.Code = code;
        }
        public int Code { get; private set; }

        public static JsonResult Success { get { return new JsonResult(1); } }
    }
}