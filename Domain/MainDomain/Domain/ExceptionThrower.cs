using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    /// <summary>
    /// 用于需要更新数据内容后，抛出异常
    /// </summary>

    [Component]
    [AwaitCommitted]
    public class ExceptionThrower : IEventHandler<VerifyLoginPasswordFailed>,        //登录密码验证失败
                                    IEventHandler<VerifyTradePasswordFailed>         //资金密码验证失败
                                    //IEventHandler<VerifyGAPasswordFailed>,           //谷歌身份验证失败
                                    //IEventHandler<VerifySmsOTPFailed>                //手机短信验证失败
    {
        public void Handle(VerifyLoginPasswordFailed @event) { throw new UserVerifyLoginPasswordException(); }
        //public void Handle(VerifyGAPasswordFailed @event) { throw new GAPasswordErrorException(); }
        //public void Handle(VerifySmsOTPFailed @event) { throw new SMSPasswordErrorException(); }
        public void Handle(VerifyTradePasswordFailed @event) { throw new TradePasswordErrorException(); }
    }
}
