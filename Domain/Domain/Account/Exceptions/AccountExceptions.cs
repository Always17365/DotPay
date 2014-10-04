using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Exceptions
{
    public class AccountBalanceNotEnoughException : DomainException
    {
        public AccountBalanceNotEnoughException() : base((int)ErrorCode.AccountBalanceNotEnough) { }
    }

    public class AccountLockedNotEnoughException : DomainException
    {
        public AccountLockedNotEnoughException() : base((int)ErrorCode.AccountLockedNotEnough) { }
    }
}
