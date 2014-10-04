using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Tools.SmsInterface
{
    public class SmsInterfaceException : ApplicationException
    {
        public SmsInterfaceException(int code, string message)
            : base(message)
        {
            this.Code = code;
        }
        public int Code { get; private set; }
    }
}
