using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using DotPay.Language;
using System.Web;
using FC.Framework.Repository;

namespace DotPay.MainDomain
{
    [Component]
    [AwaitCommitted]
    public class EmailSender : IEventHandler<PreRegistrationCreated>,   //用户预注册
                       IEventHandler<PreRegistrationRefreshed>,         //用户预注册-刷新
                       IEventHandler<ResendActiveEmail>,                //重发邮箱激活邮件
                       IEventHandler<UserSetNewPassword>,               //用户设置了新的密码(通过密码重置)
                       IEventHandler<UserPasswordResetedByEmail>,       //用户登录密码重置
                       IEventHandler<UserTradePasswordReseted>          //用户资金密码重置
    {
        public void Handle(UserRegisted @event)
        {
            //var token = "" + @event.RegistUser.Membership.EmailValidateToken;
            ////需定制email内容模板
            //string emailTitle = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_TITLE);
            //string emailBody = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_BODY).FormatWith(
            //                               HttpUtility.UrlEncode(@event.Email), token, DateTime.Now.ToShortDateString());

            //if (Config.Debug)
            //    Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            //else
            //    EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }

        public void Handle(UserPasswordResetedByEmail @event)
        {
            //需定制email内容模板
            string emailTitle = LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_TITLE);
            string emailBody = LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_BODY).FormatWith(
                                                @event.NickName, @event.UserID, @event.PasswordResetToken,
                                                DateTime.Now.ToShortDateString());

            if (Config.Debug)
                Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            else
                EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }

        public void Handle(UserTradePasswordReseted @event)
        {
            //需定制email内容模板
            string emailTitle = string.Empty; //LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_TITLE);
            string emailBody = string.Empty; //LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_BODY.FormatWith(@event.Email, @event.PasswordResetToken, DateTime.Now));

            //if (Config.Debug)
            //    Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            //else
            //    EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }

        public void Handle(UserSetNewPassword @event)
        {
            //需定制email内容模板
            string emailTitle = string.Empty; //LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_TITLE);
            string emailBody = string.Empty; //LangHelpers.Lang(LangKey.DomainEmail.USER_RESET_PASSWORD_EMAIL_BODY.FormatWith(@event.Email, @event.PasswordResetToken, DateTime.Now));

            if (Config.Debug)
                Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            else
                EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }

        public void Handle(ResendActiveEmail @event)
        {
            var token = "" + @event.EmailActiveToken;
            //需定制email内容模板
            string emailTitle = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_TITLE);
            string emailBody = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_BODY).FormatWith(
                                           HttpUtility.UrlEncode(@event.Email), token, DateTime.Now.ToShortDateString(),
                                           LangHelpers.Lang(LangKey.DomainEmail.OUR_TEAM_NAME));

            if (Config.Debug)
                Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            else
                EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }



        public void Handle(PreRegistrationCreated @event)
        {
            //需定制email内容模板
            string emailTitle = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_TITLE);
            string emailBody = "test=" + @event.Token; /*LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_BODY).FormatWith(
                                                        HttpUtility.UrlEncode(@event.Email), token, DateTime.Now.ToShortDateString());*/

            if (Config.Debug)
                Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            else
                EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }

        public void Handle(PreRegistrationRefreshed @event)
        {
            var token = @event.Token;
            //需定制email内容模板
            string emailTitle = LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_TITLE);
            string emailBody = "test=" + @event.Token; /*LangHelpers.Lang(LangKey.DomainEmail.WELCOME_REGISTER_EMAIL_BODY).FormatWith(
                                           HttpUtility.UrlEncode(@event.Email), token, DateTime.Now.ToShortDateString());*/

            if (Config.Debug)
                Log.Debug("DEBUG模式,这个邮件不会真的发送出去,邮件内容:" + emailBody);
            else
                EmailHelper.SendMail(@event.Email, emailTitle, emailBody);
        }
    }
}
