using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Exceptions
{
    public class TokenIsUsedOrTimeOutException : DomainException
    {
        public TokenIsUsedOrTimeOutException() : base((int)ErrorCode.TokenIsUsedOrTimeOut) { }
    }

    public class UserIDOfTokenNotMatchException : DomainException
    {
        public UserIDOfTokenNotMatchException() : base((int)ErrorCode.UserIDOfTokenNotMatch) { }
    }
}
