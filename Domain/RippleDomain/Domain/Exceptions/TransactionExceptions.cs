using DotPay.Common;
using FC.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleDomain.Exceptions
{
    public class RippleTransactionNotInitException : DomainException
    {
        public RippleTransactionNotInitException() : base((int)ErrorCode.RippleTransactionNotInit) { }
    }
    public class RippleTransactionNotPendingException : DomainException
    {
        public RippleTransactionNotPendingException() : base((int)ErrorCode.RippleTransactionNotPending) { }
    }
}
