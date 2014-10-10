using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    #region user events
    #region UserReigisted event
    public class UserRegisted : DomainEvent
    {
        public UserRegisted(int commendBy, string email, string password, string rippleAddress, string rippleSecret, int timezone, User registUser)
        {
            this.CommendBy = commendBy;
            this.TimeZone = timezone;
            this.RippleAddress = rippleAddress;
            this.RippleSecret = rippleSecret;
            this.Email = email; 
            this.Password = password;
            this.RegistUser = registUser;
        }
        public int CommendBy { get; private set; }
        public int TimeZone { get; private set; }
        public string RippleAddress { get; private set; }
        public string RippleSecret { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public User RegistUser { get; private set; }
    }

    public class UserRegistedByOpenAuth : DomainEvent
    {
        public UserRegistedByOpenAuth(int commendBy, string nickName, string rippleAddress, string rippleSecret, string password)
        {
            this.CommendBy = commendBy;
            this.NickName = nickName;
            this.RippleAddress = rippleAddress;
            this.RippleSecret = rippleSecret;
            this.Password = password;
        }
        public int CommendBy { get; private set; }
        public string NickName { get; private set; }
        public string RippleAddress { get; private set; }
        public string RippleSecret { get; private set; }
        public string Password { get; private set; }
    }
    #endregion


    #region User Commend Success event
    public class UserCommendSuccess : DomainEvent
    {
        public UserCommendSuccess(int commendBy)
        {
            this.CommendBy = commendBy;
        }
        public int CommendBy { get; private set; }
    }
    #endregion

    #region Resend Active Email event
    public class ResendActiveEmail : DomainEvent
    {
        public ResendActiveEmail(int userID, string email, string emailActiveToken)
        {
            this.UserID = userID;
            this.Email = email;
            this.EmailActiveToken = emailActiveToken;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string EmailActiveToken { get; private set; }
    }
    #endregion

    #region VerifiedEmail event

    public class VerifiedEmail : DomainEvent
    {
        public VerifiedEmail(int userID, string email, string verifyToken)
        {
            this.UserID = userID;
            this.Email = email;
            this.VerifyToken = verifyToken;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string VerifyToken { get; private set; }
    }
    #endregion

    #region VerifyLoginPassword event

    public class VerifyLoginPasswordFailed : DomainEvent
    {
        public VerifyLoginPasswordFailed(int userID, string email)
        {
            this.UserID = userID;
            this.Email = email;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
    }
    #endregion

    #region UserPasswordChanged event
    public class UserNickNameChanged : DomainEvent
    {
        public UserNickNameChanged(int userID, string nickName)
        {
            this.UserID = userID;
            this.NickName = nickName;
        }
        public int UserID { get; private set; }
        public string NickName { get; private set; }
    }
    #endregion

    #region VerifyTradePassword event
    public class VerifyTradePassword : DomainEvent
    {
        public VerifyTradePassword(int userID, string tradePassword)
        {
            this.UserID = userID;
            this.TradePassword = tradePassword;
        }
        public int UserID { get; private set; }
        public string TradePassword { get; private set; }
    }

    public class VerifyTradePasswordFailed : DomainEvent
    {
        public VerifyTradePasswordFailed(int userID, string email)
        {
            this.UserID = userID;
            this.Email = email;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
    }
    #endregion

    #region OpenLoginGAVerify event
    public class OpenLoginGAVerify : DomainEvent
    {
        public OpenLoginGAVerify(int userID, bool open)
        {
            this.UserID = userID;
            this.Open = open;
        }
        public int UserID { get; private set; }
        public bool Open { get; private set; }
    }
    #endregion

    #region OpenLoginSMSVerify event
    public class OpenLoginSMSVerify : DomainEvent
    {
        public OpenLoginSMSVerify(int userID, bool open)
        {
            this.UserID = userID;
            this.Open = open;
        }
        public int UserID { get; private set; }
        public bool Open { get; private set; }
    }
    #endregion

    #region OpenTwoFactorGAVerify event
    public class OpenTwoFactorGAVerify : DomainEvent
    {
        public OpenTwoFactorGAVerify(int userID, bool open)
        {
            this.UserID = userID;
            this.Open = open;
        }
        public int UserID { get; private set; }
        public bool Open { get; private set; }
    }
    #endregion

    #region OpenTwoFactorSMSVerify event
    public class OpenTwoFactorSMSVerify : DomainEvent
    {
        public OpenTwoFactorSMSVerify(int userID, bool open)
        {
            this.UserID = userID;
            this.Open = open;
        }
        public int UserID { get; private set; }
        public bool Open { get; private set; }
    }
    #endregion

    #region VerifyTwoFactorPasswordFailed event
    public class VerifyGAPasswordFailed : DomainEvent
    {
        public VerifyGAPasswordFailed(int userID)
        {
            this.UserID = userID;
        }
        public int UserID { get; private set; }
    }

    public class VerifySmsOTPFailed : DomainEvent
    {
        public VerifySmsOTPFailed(int userID)
        {
            this.UserID = userID;
        }
        public int UserID { get; private set; }
    }
    #endregion

    #region UserPasswordChanged event
    public class UserPasswordChanged : DomainEvent
    {
        public UserPasswordChanged(int userID, string newPassword, string email)
        {
            this.UserID = userID;
            this.NewPassword = newPassword;
            this.Email = email;
        }
        public int UserID { get; private set; }
        public string NewPassword { get; private set; }
        public string Email { get; private set; }
    }
    #endregion

    #region UserTradePasswordChanged event
    public class UserFirstSetTradePassword : DomainEvent
    {
        public UserFirstSetTradePassword(int userID, string newTradePassword)
        {
            this.UserID = userID;
            this.NewTradePassword = newTradePassword;
        }
        public int UserID { get; private set; }
        public string NewTradePassword { get; private set; }
    }
    #endregion

    #region UserTradePasswordChanged event
    public class UserTradePasswordChanged : DomainEvent
    {
        public UserTradePasswordChanged(int userID, string newTradePassword)
        {
            this.UserID = userID;
            this.NewTradePassword = newTradePassword;
        }
        public int UserID { get; private set; }
        public string NewTradePassword { get; private set; }
    }
    #endregion

    #region UserResetPassword event
    public class UserPasswordResetByEmail : DomainEvent
    {
        public UserPasswordResetByEmail(int userID, string email)
        {
            this.UserID = userID;
            this.Email = email;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
    }

    public class UserPasswordResetByMobile : DomainEvent
    {
        public UserPasswordResetByMobile(int userID, string mobile)
        {
            this.UserID = userID;
            this.Mobile = mobile;
        }
        public int UserID { get; private set; }
        public string Mobile { get; private set; }
    }

    public class UserPasswordResetedByEmail : DomainEvent
    {
        public UserPasswordResetedByEmail(int userID, string nickName, string email, string resetToken)
        {
            this.UserID = userID;
            this.Email = email;
            this.NickName = nickName;
            this.PasswordResetToken = resetToken;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string PasswordResetToken { get; private set; }
        public string NickName { get; private set; }
    }

    public class UserPasswordResetedByMobile : DomainEvent
    {
        public UserPasswordResetedByMobile(int userID, string mobile, string resetToken)
        {
            this.UserID = userID;
            this.Mobile = mobile;
            this.PasswordResetToken = resetToken;
        }
        public int UserID { get; private set; }
        public string Mobile { get; private set; }
        public string PasswordResetToken { get; private set; }
    }
    #endregion


    #region UserSetNewPassword event
    public class UserSetNewPassword : DomainEvent
    {
        public UserSetNewPassword(int userID, string newPassword, string email, string mobile)
        {
            this.UserID = userID;
            this.NewPassword = newPassword;
            this.Email = email;
            this.Mobile = mobile;
        }

        public int UserID { get; private set; }
        public string NewPassword { get; private set; }
        public string Email { get; private set; }
        public string Mobile { get; private set; }
    }
    #endregion

    #region UserResetTradePassword event
    public class UserTradePasswordReset : DomainEvent
    {
        public UserTradePasswordReset(int userID, string email)
        {
            this.UserID = userID;
            this.Email = email;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
    }

    public class UserTradePasswordReseted : DomainEvent
    {
        public UserTradePasswordReseted(int userID, string email, string resetToken)
        {
            this.UserID = userID;
            this.Email = email;
            this.ResetTradePasswordToken = resetToken;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string ResetTradePasswordToken { get; private set; }
    }
    #endregion

    #region OneTimePasswordIsUsed event
    public class OneTimePasswordIsUsedOrError : DomainEvent
    {
        public OneTimePasswordIsUsedOrError(int userID)
        {
            this.UserID = userID;
        }
        public int UserID { get; private set; }
    }
    #endregion

    #region RealNameAuthentication event
    public class RealNameAuthenticated : DomainEvent
    {
        public RealNameAuthenticated(int userID, string realName, string idNo, IdNoType idType)
        {
            this.UserID = userID;
            this.RealName = realName;
            this.IdNo = idNo;
            this.IdType = idType;
        }
        public int UserID { get; private set; }
        public string RealName { get; private set; }
        public string IdNo { get; private set; }
        public IdNoType IdType { get; private set; }
    }
    #endregion

    #region UserLocked event
    public class UserLocked : DomainEvent
    {
        public UserLocked(int userID, string reason, int byUserID)
        {
            this.UserID = userID;
            this.ByUserID = byUserID;
            this.Reason = reason;
        }
        public int UserID { get; private set; }
        public string Reason { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region UserUnlocked event
    public class UserUnlocked : DomainEvent
    {
        public UserUnlocked(int userID, string reason, int byUserID)
        {
            this.UserID = userID;
            this.ByUserID = byUserID;
            this.Reason = reason;
        }
        public int UserID { get; private set; }
        public string Reason { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region UserOpenGoogleAuthentication event
    public class UserSetGoogleAuthentication : DomainEvent
    {
        public UserSetGoogleAuthentication(int userID, string otpSecret, string otp)
        {
            this.UserID = userID;
            this.OTPSecret = otpSecret;
            this.OneTimePassword = otp;
        }
        public int UserID { get; private set; }
        public string OTPSecret { get; private set; }
        public string OneTimePassword { get; private set; }
    }
    #endregion

    #region UserCloseGoogleAuthentication event
    public class UserCloseGoogleAuthentication : DomainEvent
    {
        public UserCloseGoogleAuthentication(int userID, string gaotp)
        {
            this.UserID = userID;
            this.GaOtp = gaotp;
        }
        public int UserID { get; private set; }
        public string GaOtp { get; private set; }
    }
    #endregion
    #region UserModifyMobile event
    public class UserModifiedMobile : DomainEvent
    {
        public UserModifiedMobile(int userID, string mobile, string newOTPSecret, string smsOTP)
        {
            this.UserID = userID;
            this.Mobile = mobile;
            this.NewOTPSecret = newOTPSecret;
            this.OneTimePassword = smsOTP;
        }
        public int UserID { get; private set; }
        public string Mobile { get; private set; }
        public string NewOTPSecret { get; private set; }
        public string OneTimePassword { get; private set; }
    }
    #endregion

    #region UserAssignedRole event
    public class UserAssignedRole : DomainEvent
    {
        public UserAssignedRole(int userID, ManagerType managerType, int byManagerUserID)
        {
            this.UserID = userID;
            this.ManagerType = managerType;
            this.ByManagerUserID = byManagerUserID;
        }
        public int UserID { get; private set; }
        public ManagerType ManagerType { get; private set; }
        public int ByManagerUserID { get; set; }
    }
    #endregion

    #region UserUnsignedRole event
    public class UserUnsignedRole : DomainEvent
    {
        public UserUnsignedRole(int userID, ManagerType managerType, int byManagerUserID)
        {
            this.UserID = userID;
            this.ManagerType = managerType;
            this.ByManagerUserID = byManagerUserID;
        }
        public int UserID { get; private set; }
        public ManagerType ManagerType { get; private set; }
        public int ByManagerUserID { get; set; }
    }
    #endregion

    #region UserLoginSuccess
    public class UserLoginSuccess : DomainEvent
    {
        public UserLoginSuccess(int userID, string email, string ip)
        {
            this.UserID = userID;
            this.Email = email;
            this.IP = ip;
        }
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string IP { get; private set; }
    }
    #endregion

    #region Score Change  event
    public class UserScoreIncrease : DomainEvent
    {
        public UserScoreIncrease(int userID, int increase)
        {
            this.UserID = userID;
            this.Increase = increase;
        }
        public int UserID { get; private set; }
        public int Increase { get; private set; }
    }

    public class UserScoreDecrease : DomainEvent
    {
        public UserScoreDecrease(int userID, int decrease)
        {
            this.UserID = userID;
            this.Decrease = decrease;
        }
        public int UserID { get; private set; }
        public int Decrease { get; private set; }
    }

    public class UserScoreUsed : DomainEvent
    {
        public UserScoreUsed(int userID, int useQuantity)
        {
            this.UserID = userID;
            this.UseQuantity = useQuantity;
        }
        public int UserID { get; private set; }
        public int UseQuantity { get; private set; }
    }
    #endregion

    #region ReceiveScoreDaliyLogin
    public class ReceiveScoreDaliyLogin : DomainEvent
    {
        public ReceiveScoreDaliyLogin(int userID)
        {
            this.UserID = userID;
        }
        public int UserID { get; private set; }
    }
    #endregion
    #endregion

    #region manager
    public class ManagerCreated : DomainEvent
    {
        public ManagerCreated(int userID, ManagerType managerType)
        {
            this.UserID = userID;
            this.ManagerType = managerType;
        }
        public int UserID { get; protected set; }
        public ManagerType ManagerType { get; protected set; }
    }

    public class ManagerRemoved : DomainEvent
    {
        public ManagerRemoved(int userID, int removeBy)
        {
            this.UserIDOfManager = UserIDOfManager;
            this.RemoveBy = removeBy;
        }
        public int RemoveBy { get; protected set; }
        public int UserIDOfManager { get; set; }
    }
    #endregion
}
