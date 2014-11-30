using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{

    #region User PreRegister Command
    [ExecuteSync]
    public class UserPreRegister : FC.Framework.Command
    {
        public UserPreRegister(string email)
        {
            Check.Argument.IsNotInvalidEmail(email, "email");

            this.Email = email;
        }

        public string Email { get; private set; }
    }
    #endregion

    #region User Register Command
    [ExecuteSync]
    public class UserRegister : FC.Framework.Command
    {
        public UserRegister(string loginName,string email, string password, string tradePassword, int timezone, string token, int commendBy = 0)
        {
            Check.Argument.IsNotNegative(commendBy, "commendBy");
            //Check.Argument.IsNotEmpty(rippleAddress, "rippleAddress");
            Check.Argument.IsNotInvalidEmail(email, "email");
            Check.Argument.IsNotEmpty(loginName, "loginName");
            Check.Argument.IsNotEmpty(password, "password");

            this.LoginName = loginName;
            this.Email = email;
            this.Password = password;
            this.TradePassword = tradePassword;
            this.RegisterToken = token;
            //this.RippleAddress = rippleAddress;
            //this.RippleSecret = rippleSecret;
            this.TimeZone = timezone;
            this.CommendBy = commendBy;
        }

        public string LoginName { get; private set; }
        public string Email { get; private set; }
        public string RegisterToken { get; private set; }
        public string Password { get; private set; }
        public string TradePassword { get; private set; }
        //public string RippleAddress { get; private set; }
        //public string RippleSecret { get; private set; }
        public int TimeZone { get; private set; }
        public int CommendBy { get; private set; }
    }
    #endregion

    #region User Login Command
    [ExecuteSync]
    public class UserLogin : FC.Framework.Command
    {
        public UserLogin(string loginName, string password, string ip)
        {
            Check.Argument.IsNotEmpty(loginName, "loginName");
            Check.Argument.IsNotEmpty(password, "password");
            Check.Argument.IsNotEmpty(ip, "ip");

            this.LoginName = loginName;
            this.Password = password;
            this.IP = ip;
        }

        public string LoginName { get; private set; }
        public string Password { get; private set; }
        public string IP { get; private set; }
    }
    #endregion

    #region User QQ Open Login Command
    [ExecuteSync]
    public class UserQQLogin : FC.Framework.Command
    {
        public UserQQLogin(string openID, string nickName, string rippleAddress, string rippleSecret, string ip, int commendBy = 0)
        {
            Check.Argument.IsNotEmpty(openID, "openID");
            Check.Argument.IsNotEmpty(rippleAddress, "rippleAddress");
            Check.Argument.IsNotEmpty(rippleSecret, "rippleSecret");
            Check.Argument.IsNotEmpty(ip, "ip");
            Check.Argument.IsNotNegative(commendBy, "commendBy");

            this.OpenID = openID;
            this.IP = ip;
            this.RippleAddress = rippleAddress;
            this.RippleSecret = rippleSecret;
            this.NickName = nickName;
            this.CommendBy = commendBy;
        }

        public string OpenID { get; private set; }
        public string IP { get; private set; }
        public string NickName { get; private set; }
        public string RippleAddress { get; private set; }
        public string RippleSecret { get; private set; }
        public int CommendBy { get; private set; }
    }
    #endregion

    #region User Weibo Open Login Command
    [ExecuteSync]
    public class UserWeiboLogin : FC.Framework.Command
    {
        public UserWeiboLogin(string openID, string nickName, string rippleAddress, string rippleSecret, string ip, int commendBy = 0)
        {
            Check.Argument.IsNotEmpty(openID, "openID");
            Check.Argument.IsNotEmpty(rippleAddress, "rippleAddress");
            Check.Argument.IsNotEmpty(rippleSecret, "rippleSecret");
            Check.Argument.IsNotEmpty(ip, "ip");
            Check.Argument.IsNotNegative(commendBy, "commendBy");

            this.OpenID = openID;
            this.NickName = nickName;
            this.RippleAddress = RippleAddress;
            this.RippleSecret = rippleSecret;
            this.IP = ip;
            this.CommendBy = commendBy;
        }

        public string OpenID { get; private set; }
        public string NickName { get; private set; }
        public string RippleAddress { get; private set; }
        public string RippleSecret { get; private set; }
        public string IP { get; private set; }
        public int CommendBy { get; private set; }
    }
    #endregion

    //#region User  Sms Counter Command
    ////[ExecuteSync]
    ////public class UserVerifyGAPassword : FC.Framework.Command
    ////{
    ////    public UserVerifyGAPassword(int userID, string oneTimePassword)
    ////    {
    ////        Check.Argument.IsNotNegativeOrZero(userID, "userID");
    ////        Check.Argument.IsNotEmpty(oneTimePassword, "oneTimePassword");

    ////        this.UserID = userID;
    ////        this.OneTimePassword = oneTimePassword;
    ////    }
    ////    public int UserID { get; private set; }
    ////    public string OneTimePassword { get; private set; }
    ////}

    //[ExecuteSync]
    //public class UserVerifySMSPassword : FC.Framework.Command
    //{
    //    public UserVerifySMSPassword(int userID, string oneTimePassword)
    //    {
    //        Check.Argument.IsNotNegativeOrZero(userID, "userID");
    //        Check.Argument.IsNotEmpty(oneTimePassword, "oneTimePassword");

    //        this.UserID = userID;
    //        this.OneTimePassword = oneTimePassword;
    //    }
    //    public int UserID { get; private set; }
    //    public string OneTimePassword { get; private set; }
    //}
    //#endregion

    #region User Set NickName
    [ExecuteSync]
    public class UserSetNickName : FC.Framework.Command
    {
        public UserSetNickName(int userID, string nickName)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(nickName, "nickName");

            this.UserID = userID;
            this.NickName = nickName;
        }

        public int UserID { get; private set; }
        public string NickName { get; private set; }
    }
    #endregion

    #region User Real Name Auth Command
    [ExecuteSync]
    public class UserRealNameAuth : FC.Framework.Command
    {
        public UserRealNameAuth(int userID, string realName, IdNoType idType, string idNo)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(realName, "realName");
            Check.Argument.IsNotNull(idType, "idType");
            Check.Argument.IsNotEmpty(idNo, "idNo");

            this.UserID = userID;
            this.RealName = realName;
            this.IdType = idType;
            this.IdNo = idNo;
        }

        public int UserID { get; private set; }
        public string RealName { get; private set; }
        public IdNoType IdType { get; private set; }
        public string IdNo { get; private set; }
    }
    #endregion

    #region User Open/Close Google Authentication Command
    [ExecuteSync]
    public class UserOpenGoogleAuthentication : FC.Framework.Command
    {
        public UserOpenGoogleAuthentication(int userID, string googleSecret, string ga_otp)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(googleSecret, "googleSecret");

            this.UserID = userID;
            this.GoogleSecret = googleSecret;
            this.OneTimePassword = ga_otp;
        }

        public int UserID { get; private set; }
        public string GoogleSecret { get; private set; }
        public string OneTimePassword { get; private set; }
    }
    [ExecuteSync]
    public class UserCloseGoogleAuthentication : FC.Framework.Command
    {
        public UserCloseGoogleAuthentication(int userID, string google_otp)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(google_otp, "google_otp");

            this.UserID = userID;
            this.OneTimePassword = google_otp;
        }

        public int UserID { get; private set; }
        public string OneTimePassword { get; private set; }
    }
    #endregion

    #region User Set Mobile  Command
    /// <summary>
    /// 设置用户的手机号，并自动开启双重身份验证
    /// </summary>
    [ExecuteSync]
    public class UserSetMobile : FC.Framework.Command
    {
        public UserSetMobile(int userID, string mobile, string newOTPSecret, string oneTimePassword)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(mobile, "mobile");
            Check.Argument.IsNotEmpty(oneTimePassword, "oneTimePassword");

            this.UserID = userID;
            this.Mobile = mobile;
            this.NewOTPSecret = newOTPSecret;
            this.OneTimePassword = oneTimePassword;
        }

        public int UserID { get; private set; }
        public string Mobile { get; private set; }
        public string NewOTPSecret { get; private set; }
        public string OneTimePassword { get; private set; }
    }
    #endregion

    #region User Open/Close Two-Factor  Command
    [ExecuteSync]
    public class UserOpenLoginTwoFactor : FC.Framework.Command
    {
        public UserOpenLoginTwoFactor(int userID, string sms_otp, string ga_otp)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
            this.SmsOtp = sms_otp;
            this.GaOtp = ga_otp;
        }

        public int UserID { get; private set; }
        public string SmsOtp { get; private set; }
        public string GaOtp { get; private set; }
    }
    [ExecuteSync]
    public class UserCloseLoginTwoFactor : FC.Framework.Command
    {
        public UserCloseLoginTwoFactor(int userID, string sms_otp, string ga_otp)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
            this.SmsOtp = sms_otp;
            this.GaOtp = ga_otp;
        }

        public int UserID { get; private set; }
        public string SmsOtp { get; private set; }
        public string GaOtp { get; private set; }
    }
    #endregion

    #region User Modify Password Command
    [ExecuteSync]
    public class UserModifyPassword : FC.Framework.Command
    {
        public UserModifyPassword(int userID, string oldPassword,
            string newPassword)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(oldPassword, "oldPassword");
            Check.Argument.IsNotEmpty(newPassword, "newPassword");

            this.UserID = userID;
            this.OldPassword = oldPassword;
            this.NewPassword = newPassword;
        }

        public int UserID { get; private set; }
        public string OldPassword { get; private set; }
        public string NewPassword { get; private set; }
    }

    [ExecuteSync]
    public class UserResetPasswordByTwoFactor : FC.Framework.Command
    {
        public UserResetPasswordByTwoFactor(int userID, string newPassword, string oneTimePassword_GA, string oneTimePassword_sms)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(newPassword, "newPassword");

            this.UserID = userID;
            this.NewPassword = newPassword;
            this.OneTimePassword_GA = oneTimePassword_GA;
            this.OneTimePassword_Sms = oneTimePassword_sms;
        }

        public int UserID { get; private set; }
        public string NewPassword { get; private set; }
        public string OneTimePassword_GA { get; private set; }
        public string OneTimePassword_Sms { get; private set; }
    }

    [ExecuteSync]
    public class UserResetPasswordByEmailToken : FC.Framework.Command
    {
        public UserResetPasswordByEmailToken(int userID, string newPassword, string email_Token)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(newPassword, "newPassword");
            Check.Argument.IsNotEmpty(email_Token, "email_Token");

            this.UserID = userID;
            this.NewPassword = newPassword;
            this.EmailToken = email_Token;
        }

        public int UserID { get; private set; }
        public string NewPassword { get; private set; }
        public string EmailToken { get; private set; }
    }
    #endregion

    #region User Modify Trade Password Command
    [ExecuteSync]
    public class UserModifyTradePassword : FC.Framework.Command
    {
        public UserModifyTradePassword(int userID, string oldTradePassword,
            string newTradePassword, string oneTimePassword_GA, string oneTimePassword_SMS)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            //Check.Argument.IsNotEmpty(oldTradePassword, "oldTradePassword");
            Check.Argument.IsNotEmpty(newTradePassword, "newTradePassword");

            this.UserID = userID;
            this.OldTradePassword = oldTradePassword;
            this.NewTradePassword = newTradePassword;
            this.OneTimePassword_GA = oneTimePassword_GA;
            this.OneTimePassword_SMS = oneTimePassword_SMS;
        }

        public int UserID { get; private set; }
        public string OldTradePassword { get; private set; }
        public string NewTradePassword { get; private set; }
        public string OneTimePassword_GA { get; private set; }
        public string OneTimePassword_SMS { get; private set; }
    }
    [ExecuteSync]
    public class UserResetTradePasswordByTwoFactor : FC.Framework.Command
    {
        public UserResetTradePasswordByTwoFactor(int userID, string newPassword, string oneTimePassword_GA, string oneTimePassword_SMS)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(newPassword, "newPassword");

            this.UserID = userID;
            this.NewPassword = newPassword;
            this.OneTimePassword_GA = oneTimePassword_GA;
            this.OneTimePassword_SMS = oneTimePassword_SMS;
        }

        public int UserID { get; private set; }
        public string NewPassword { get; private set; }
        public string OneTimePassword_GA { get; private set; }
        public string OneTimePassword_SMS { get; private set; }
    }

    [ExecuteSync]
    public class UserResetTradePasswordByEmailToken : FC.Framework.Command
    {
        public UserResetTradePasswordByEmailToken(int userID, string newPassword, string email_Token)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(newPassword, "newPassword");
            Check.Argument.IsNotEmpty(email_Token, "email_Token");

            this.UserID = userID;
            this.NewTradePassword = newPassword;
            this.EmailToken = email_Token;
        }

        public int UserID { get; private set; }
        public string NewTradePassword { get; private set; }
        public string EmailToken { get; private set; }
    }
    #endregion

    #region User First Time Set Trade Password Command

    [ExecuteSync]
    public class UserFirstTimeSetTradePassword : FC.Framework.Command
    {
        public UserFirstTimeSetTradePassword(int userID,
            string tradePassword, string oneTimePassword_GA, string oneTimePassword_SMS)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            //Check.Argument.IsNotEmpty(oldTradePassword, "oldTradePassword");
            Check.Argument.IsNotEmpty(tradePassword, "newTradePassword");

            this.UserID = userID;
            this.TradePassword = tradePassword;
            this.OneTimePassword_GA = oneTimePassword_GA;
            this.OneTimePassword_SMS = oneTimePassword_SMS;
        }

        public int UserID { get; private set; }
        public string TradePassword { get; private set; }
        public string OneTimePassword_GA { get; private set; }
        public string OneTimePassword_SMS { get; private set; }
    }
    #endregion

    #region User Forget Password Command
    [ExecuteSync]
    public class UserForgetPassword : FC.Framework.Command
    {
        public UserForgetPassword(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
        }

        public int UserID { get; private set; }
    }
    #endregion

    #region User Forget Trade Password Command
    [ExecuteSync]
    public class UserForgetTradePassword : FC.Framework.Command
    {
        public UserForgetTradePassword(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
        }

        public int UserID { get; private set; }
    }
    #endregion

    #region Lock  User
    [ExecuteSync]
    public class LockUser : FC.Framework.Command
    {
        public LockUser(int userID, string reason, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = userID;
            this.Reason = reason;
            this.CurrentUserID = currentUserID;
        }

        public int UserID { get; private set; }
        public string Reason { get; private set; }
        public int CurrentUserID { get; private set; }
    }
    #endregion

    #region Unlock  User
    [ExecuteSync]
    public class UnlockUser : FC.Framework.Command
    {
        public UnlockUser(int userID, string reason, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            this.UserID = userID;
            this.Reason = reason;
            this.CurrentUserID = currentUserID;
        }

        public int UserID { get; private set; }
        public string Reason { get; private set; }
        public int CurrentUserID { get; private set; }
    }
    #endregion

    #region Send Message
    public class UserSendMessage : FC.Framework.Command
    {
        public UserSendMessage(int userID, string message)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotEmpty(message, "message");

            this.UserID = userID;
            this.Message = message;
        }

        public int UserID { get; private set; }
        public string Message { get; private set; }
    }
    #endregion

    #region User Score Increase
    public class UserScoreIncrease : FC.Framework.Command
    {
        public UserScoreIncrease(int userID, int increase)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero(increase, "increase");

            this.UserID = userID;
            this.Increase = increase;
        }

        public int UserID { get; private set; }
        public int Increase { get; private set; }
    }
    #endregion

    #region User Assign Role
    public class UserAssignRole : FC.Framework.Command
    {
        public UserAssignRole(int userID, ManagerType managerType, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNull(managerType, "managerType");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = userID;
            this.ManagerType = managerType;
            this.CurrentUserID = currentUserID;
        }

        public int UserID { get; private set; }
        public ManagerType ManagerType { get; private set; }
        public int CurrentUserID { get; private set; }
    }
    #endregion

    #region Remove Manager
    public class RemoveManager : FC.Framework.Command
    {
        public RemoveManager(int userID, int managerID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNegativeOrZero(managerID, "managerID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = userID;
            this.ManagerID = managerID;
            this.CurrentUserID = currentUserID;
        }

        public int UserID { get; private set; }
        public int ManagerID { get; private set; }
        public int CurrentUserID { get; private set; }
    }
    #endregion

    #region Sms Counter Command
    [ExecuteAsync]
    public class SmsCounterCommand : FC.Framework.Command
    {
        public SmsCounterCommand(int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = currentUserID;
        }

        public int UserID { get; set; }
    }
    #endregion


    #region Authorize CustomerService User Deposit Amount
    public class AuthorizeCustomerServiceUserDepositAmount : FC.Framework.Command
    {
        public AuthorizeCustomerServiceUserDepositAmount(int authTo, CurrencyType currency, decimal amount, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(authTo, "authTo");
            Check.Argument.IsNotNegativeOrZero((int)currency, "currency");
            Check.Argument.IsNotNegativeOrZero(amount, "amount");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.UserID = authTo;
            this.Currency = currency;
            this.AuthrizeAmount = amount;
            this.AuthrizeBy = currentUserID;
        }

        public int UserID { get; private set; }
        public CurrencyType Currency { get; private set; }
        public Decimal AuthrizeAmount { get; private set; }
        public int AuthrizeBy { get; private set; }
    }
    #endregion

}
