using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Exceptions
{
    public class CurrencyIsLockedException : DomainException
    {
        public CurrencyIsLockedException() : base((int)ErrorCode.CurrencyIsLocked) { }
    }

    public class CurrencyIsDisabledException : DomainException
    {
        public CurrencyIsDisabledException() : base((int)ErrorCode.CurrencyIsDisabled) { }
    }
}
