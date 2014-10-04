using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Exceptions
{
    public class UserVerifyLoginPasswordException : DomainException
    {
        public UserVerifyLoginPasswordException() : base((int)ErrorCode.LoginNameOrPasswordError) { }
    }

    public class UserIsLockedException : DomainException
    {
        public UserIsLockedException() : base((int)ErrorCode.UserAccountIsLocked) { }
    }
    public class OldPasswordErrorException : DomainException
    {
        public OldPasswordErrorException() : base((int)ErrorCode.OldPasswordError) { }
    }

    public class OldTradePasswordErrorException : DomainException
    {
        public OldTradePasswordErrorException() : base((int)ErrorCode.OldTradePasswordError) { }
    }

    public class GAPasswordErrorException : DomainException
    {
        public GAPasswordErrorException() : base((int)ErrorCode.GAPasswordError) { }
    }
    public class MobileHasSetException : DomainException
    {
        public MobileHasSetException() : base((int)ErrorCode.MobileHasSet) { }
    }
    public class SMSPasswordErrorException : DomainException
    {
        public SMSPasswordErrorException() : base((int)ErrorCode.SMSPasswordError) { }
    }

    public class TradePasswordErrorException : DomainException
    {
        public TradePasswordErrorException() : base((int)ErrorCode.TradePasswordError) { }
    }
    public class RealNameAuthenticationIsPassedException : DomainException
    {
        public RealNameAuthenticationIsPassedException() : base((int)ErrorCode.RealNameAuthenticationIsPassed) { }
    }

    public class GoogleAuthenticationIsSettedException : DomainException
    {
        public GoogleAuthenticationIsSettedException() : base((int)ErrorCode.GoogleAuthenticationIsSetted) { }
    }

    public class GoogleAuthenticationIsNotSetException : DomainException
    {
        public GoogleAuthenticationIsNotSetException() : base((int)ErrorCode.GoogleAuthenticationIsNotSet) { }
    }

    public class UserMobileIsNotSetException : DomainException
    {
        public UserMobileIsNotSetException() : base((int)ErrorCode.UserMobileIsNotSet) { }
    }
    public class UserHasNo2FAException : DomainException
    {
        public UserHasNo2FAException() : base((int)ErrorCode.UserHasNo2FA) { }
    }
}
