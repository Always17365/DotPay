using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using DotPay.Tools.SmsInterface;

namespace DotPay.MainDomain
{
    [Component]
    [AwaitCommitted]
    public class SMSSender : IEventHandler<UserPasswordResetedByMobile>,
                             IEventHandler<UserSetNewPassword>
    {
        public void Handle(UserPasswordResetedByMobile @event)
        {
            string templet = "您找回密码的验证是{0},为了您的账户安全，请勿泄露您的验证码(30分钟内有效)";

            var randomNum = new Random().Next(5, 6);
            string code = @event.PasswordResetToken.Substring(0, randomNum);
            string content = templet.FormatWith(@event.PasswordResetToken);

            if (Config.Debug)
                Log.Debug("DEBUG环境下,模拟发送了短信:" + content);
            else
                SmsHelper.Send(@event.Mobile, content);
        }

        public void Handle(UserSetNewPassword @event)
        {
            string content = "您的账户重置密码成功!如果非您本人操作，请尽快联系客服，以确保您的资金安全"; 

            if (Config.Debug)
                Log.Debug("DEBUG环境下,模拟发送了短信:" + content);
            else
                SmsHelper.Send(@event.Mobile, content);
        }
    }
}
