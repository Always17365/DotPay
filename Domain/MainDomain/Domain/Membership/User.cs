using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
namespace DotPay.MainDomain
{

    public class User : DomainBase, IAggregateRoot,
                 IEventHandler<UserRegisted>,                  //注册
                 IEventHandler<UserRegistedByOpenAuth>,        //使用OpenAuth注册
                 IEventHandler<UserLoginSuccess>,              //使用用户登录成功 
                 IEventHandler<UserPasswordChanged>,           //密码修改 
                 IEventHandler<UserFirstSetTradePassword>,     //第一次设置资金密码 
                 IEventHandler<UserTradePasswordChanged>,      //资金密码修改 
        //IEventHandler<UserNickNameChanged>,           //修改昵称
                 IEventHandler<UserSetGoogleAuthentication>,   //设置谷歌身份验证
                 IEventHandler<UserCloseGoogleAuthentication>, //关闭谷歌身份验证
                 IEventHandler<OpenLoginGAVerify>,             //开启/关闭登录谷歌身份验证
                 IEventHandler<OpenLoginSMSVerify>,            //开启/关闭登录短信身份验证
                 IEventHandler<OpenTwoFactorGAVerify>,         //开启/关闭谷歌双重身份验证
                 IEventHandler<OpenTwoFactorSMSVerify>,        //开启/关闭短信双重身份验证
                 IEventHandler<UserPasswordResetByEmail>,      //登陆密码重置_email
                 IEventHandler<UserPasswordResetByMobile>,     //登陆密码重置_mobile
                 IEventHandler<UserTradePasswordReset>,        //资金密码重置
                 IEventHandler<RealNameAuthenticated>,         //实名认证    
                 IEventHandler<UserScoreIncrease>,             //用户积分增长
                 IEventHandler<UserScoreDecrease>,             //用户积分减少(撤销充值)
                 IEventHandler<UserScoreUsed>,                 //用户积分被使用掉
                 IEventHandler<UserLocked>,                    //锁定用户
                 IEventHandler<UserUnlocked>,                  //解除锁定用户
                 IEventHandler<UserModifiedMobile>,            //修改绑定手机号
                 IEventHandler<UserAssignedRole>,              //用户分配管理角色
                 IEventHandler<UserUnsignedRole>               //用户分配管理角色
    {
        #region ctor
        protected User() { }

        public User(int commendBy, string loginName, string email, string password, string tradePassword, int timezone)
        {
            this.RaiseEvent(new UserRegisted(commendBy, loginName, email, password, tradePassword, timezone, this));
            if (commendBy > 0) this.Apply(new UserCommendSuccess(commendBy));
        }

        public User(int commendBy, string nickName, string password, /*string rippleAddress, string rippleSecret,*/ OpenAuthType openAuthType)
        {
            this.RaiseEvent(new UserRegistedByOpenAuth(commendBy, nickName, /*rippleAddress, rippleSecret,*/ password));
            if (commendBy > 0) this.Apply(new UserCommendSuccess(commendBy));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int CommendBy { get; protected set; }
        public virtual int CommendCounter { get; protected set; }
        public virtual string LoginName { get; protected set; }
        public virtual string Email { get; protected set; }
        public virtual UserVipLevel VipLevel { get; protected set; }
        public virtual int Role { get; protected set; }
        public virtual string Mobile { get; protected set; }
        public virtual int TimeZone { get; protected set; }
        //5个二进制位，低位至高位含义
        //1-->1是否绑定了GA设备 
        //2-->2是否开启了登录GA验证
        //3-->4是否开启了登录SMS验证
        //4-->8是否开启了双重身份验证GA
        //5-->16是否开启了双重身份验证SMS
        public virtual int TwoFactorFlg { get; protected set; }
        public virtual bool IsOpenAuth { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int ScoreBalance { get; protected set; }
        public virtual int ScoreUsed { get; protected set; }
        public virtual int UpdateAt { get; protected set; }

        public virtual Membership Membership { get; protected set; }
        public virtual GoogleAuthentication GoogleAuthentication { get; protected set; }
        public virtual SmsAuthentication SmsAuthentication { get; protected set; }
        #endregion

        #region public method


        #region CommendIncrease
        public virtual void CommendIncrease()
        {
            this.CommendCounter += 1;
        }
        #endregion

        #region login
        public virtual void Login(string password, string ip)
        {
            this.VerifyLoginPassword(password);

            this.RaiseEvent(new UserLoginSuccess(this.ID, this.Email, ip));
        }
        #endregion

        #region open auth login
        public virtual void OpenAuthLogin(string ip)
        {
            this.RaiseEvent(new UserLoginSuccess(this.ID, this.Email, ip));
        }
        #endregion

        //#region set nick name
        //public virtual void SetNickName(string nickName)
        //{
        //    this.RaiseEvent(new UserNickNameChanged(this.ID, nickName));
        //}
        //#endregion

        #region change password
        public virtual void ChangePassword(string oldPassword, string newPassword)
        {
            var passwordValidate = this.Membership.VerifyLoginPassword(oldPassword);
            if (!passwordValidate)
            {
                throw new OldPasswordErrorException();
            }
            //else if (((this.TwoFactorFlg & 8) != 8) || this.VerifyGAPassword(ga_otp))
            //{
            //    if (((this.TwoFactorFlg & 16) != 16) || this.VerifySMSPassword(sms_otp))
            //      this.RaiseEvent(new UserPasswordChanged(this.ID, newPassword, this.Email));
            //}
            else
                this.RaiseEvent(new UserPasswordChanged(this.ID, newPassword, this.Email));
        }
        #endregion

        #region change trade password
        public virtual void ChangeTradePassword(string oldTradePassword, string newTradePassword /*, string ga_otp, string sms_otp*/)
        {
            //if (((this.TwoFactorFlg & 9) != 9) || this.VerifyGAPassword(ga_otp))
            //    if (((this.TwoFactorFlg & 16) != 16) || this.VerifySMSPassword(sms_otp))
            if (string.IsNullOrEmpty(this.Membership.TradePassword) || this.VerifyTradePassword(oldTradePassword))
                this.RaiseEvent(new UserTradePasswordChanged(this.ID, newTradePassword));
        }
        #endregion

        #region set new password after reset
        public virtual void SetNewPasswordBy2FA(string newPassword, string ga_otp, string sms_otp)
        {
            if ((this.TwoFactorFlg & 8) != 8 && (this.TwoFactorFlg & 16) != 16)
                throw new UserHasNo2FAException();

            if (((this.TwoFactorFlg & 8) != 8) || this.VerifyGAPassword(ga_otp))
            {
                if (((this.TwoFactorFlg & 16) != 16) || this.VerifySMSPassword(sms_otp))
                    this.RaiseEvent(new UserPasswordChanged(this.ID, newPassword, this.Email));
            }
        }

        public virtual void SetNewPasswordWithEmailToken(string newPassword, string email_token)
        {
            if (!email_token.Equals(this.Membership.PasswordResetToken))
                throw new UserIDOfTokenNotMatchException();
            else
                this.Membership.ChangePassword(newPassword);
        }
        #endregion

        #region reset password by email
        public virtual void ResetPasswordByMobile()
        {
            if (this.Mobile != string.Empty)
                this.RaiseEvent(new UserPasswordResetByMobile(this.ID, this.Mobile));
        }
        #endregion

        #region reset password by email
        public virtual void ResetPasswordByEmail()
        {
            this.RaiseEvent(new UserPasswordResetByEmail(this.ID, this.Email));
        }
        #endregion

        #region reset trade password
        public virtual void ResetTradePassword()
        {
            this.RaiseEvent(new UserTradePasswordReset(this.ID, this.Email));
        }
        #endregion

        #region set new trade password after reset
        public virtual void FirstSetTradePassword(string newPassword, string ga_otp, string sms_otp)
        {
            if (((this.TwoFactorFlg & 9) != 9) || this.VerifyGAPassword(ga_otp))
                if (this.SmsAuthentication == null || ((this.TwoFactorFlg & 16) != 16) || this.VerifyGAPassword(sms_otp))
                {
                    if (string.IsNullOrEmpty(this.Membership.TradePassword))
                        this.RaiseEvent(new UserFirstSetTradePassword(this.ID, newPassword));
                }
        }
        #endregion

        #region set new trade password after reset
        public virtual void SetNewTradePasswordWithTwoFactor(string newPassword, string sms_otp)
        {
            //if (((this.TwoFactorFlg & 9) != 9) || this.VerifyGAPassword(ga_otp))
            if (this.SmsAuthentication == null || ((this.TwoFactorFlg & 16) != 16) || this.VerifySMSPassword(sms_otp))
                this.RaiseEvent(new UserTradePasswordChanged(this.ID, newPassword));
        }
        public virtual void SetNewTradePasswordWithEmailToken(string newPassword, string email_token)
        {
            if (!email_token.Equals(this.Membership.TradePasswordResetToken))
                throw new UserIDOfTokenNotMatchException();
            else
                this.RaiseEvent(new UserTradePasswordChanged(this.ID, newPassword));
        }
        #endregion

        #region verify password
        public virtual bool VerifyLoginPassword(string password)
        {
            var passwordValidate = this.Membership.VerifyLoginPassword(password);
            if (!passwordValidate)
            {
                this.RaiseEvent(new VerifyLoginPasswordFailed(this.ID, this.Email));
            }

            return passwordValidate;
        }
        #endregion

        #region verify email
        public virtual void VerifyEmail(string token)
        {
            this.RaiseEvent(new VerifiedEmail(this.ID, this.Email, token));
        }
        #endregion

        #region verify trade password
        public virtual bool VerifyTradePassword(string tradePassword)
        {
            var verifyResult = this.Membership.VerifyTradePassword(tradePassword);
            if (verifyResult != true)
                this.RaiseEvent(new VerifyTradePasswordFailed(this.ID, this.Email));

            return verifyResult;
        }
        #endregion

        #region verify two factor password
        protected virtual bool VerifyGAPassword(string oneTimePassword)
        {
            var verifyResult = this.GoogleAuthentication.Verify(oneTimePassword);

            if (verifyResult != true)
                throw new GAPasswordErrorException();

            return verifyResult;
        }

        protected virtual bool VerifySMSPassword(string oneTimePassword)
        {
            var verifyResult = this.SmsAuthentication.Verify(oneTimePassword);

            if (verifyResult != true)
                throw new SMSPasswordErrorException();

            return verifyResult;
        }
        #endregion

        #region Open GA/SMS Login Verify and Two Factor Verify
        public virtual void OpenLoginGAVerify(bool open, string ga_otp)
        {
            if ((this.TwoFactorFlg & 1) == 1)
            {
                if (this.VerifyGAPassword(ga_otp))
                    this.RaiseEvent(new OpenLoginGAVerify(this.ID, open));
            }
        }

        public virtual void OpenTwoFactorGAVerify(bool open, string ga_otp)
        {
            if (!string.IsNullOrEmpty(this.Mobile))
                if (this.VerifyGAPassword(ga_otp))
                    this.RaiseEvent(new OpenTwoFactorGAVerify(this.ID, open));
        }

        public virtual void OpenLoginSMSVerify(bool open, string sms_otp)
        {
            if (this.VerifySMSPassword(sms_otp))
                this.RaiseEvent(new OpenLoginSMSVerify(this.ID, open));
        }
        public virtual void OpenTwoFactorSMSVerify(bool open, string sms_otp)
        {
            if (this.VerifySMSPassword(sms_otp))
                this.RaiseEvent(new OpenTwoFactorSMSVerify(this.ID, open));
        }
        #endregion

        #region real name authentication
        public virtual void RealNameAuthentication(string realName, string idno, IdNoType idType)
        {
            this.RaiseEvent(new RealNameAuthenticated(this.ID, realName, idno, idType));
        }
        #endregion

        #region set/close google authentication
        public virtual void SetGoogleAuthentication(string otpSecret, string otp)
        {
            if ((this.TwoFactorFlg & 1) == 1)
                throw new GoogleAuthenticationIsSettedException();
            else if (Utilities.GenerateGoogleAuthOTP(otpSecret) != otp)
            {
                throw new GAPasswordErrorException();
            }
            else
                this.RaiseEvent(new UserSetGoogleAuthentication(this.ID, otpSecret, otp));
        }
        public virtual void CloseGoogleAuthentication(string ga_otp)
        {
            if ((this.TwoFactorFlg & 1) != 1)
                throw new GoogleAuthenticationIsNotSetException();
            else
                this.RaiseEvent(new UserCloseGoogleAuthentication(this.ID, ga_otp));
        }
        #endregion

        #region set mobile
        public virtual void SetMobile(string mobile, string newOTPSecret, string oneTimePassword)
        {
            if (!string.IsNullOrEmpty(this.Mobile))
                throw new MobileHasSetException();
            else
                this.RaiseEvent(new UserModifiedMobile(this.ID, mobile, newOTPSecret, oneTimePassword));
        }
        #endregion

        #region lock
        public virtual void Lock(string reason, int byUserID)
        {
            this.RaiseEvent(new UserLocked(this.ID, reason, byUserID));
        }
        #endregion

        #region unlock
        public virtual void Unlock(string reason, int byUserID)
        {
            this.RaiseEvent(new UserUnlocked(this.ID, reason, byUserID));
        }
        #endregion

        #region assign user role
        public virtual void Assign(ManagerType roleType, int byUserID)
        {
            this.RaiseEvent(new UserAssignedRole(this.ID, roleType, byUserID));
        }
        #endregion

        #region unsign user role
        public virtual void Unsign(ManagerType roleType, int byUserID)
        {
            this.RaiseEvent(new UserUnsignedRole(this.ID, roleType, byUserID));
        }
        #endregion

        #region  score increase
        public virtual void ScoreIncrease(int increase)
        {
            this.RaiseEvent(new UserScoreIncrease(this.ID, increase));
        }

        public virtual void ScoreDecrease(int decrease)
        {
            this.RaiseEvent(new UserScoreDecrease(this.ID, decrease));
        }
        #endregion

        #region  score decrease
        public virtual void ScoreUse(int useScoreQuantity)
        {
            this.RaiseEvent(new UserScoreUsed(this.ID, useScoreQuantity));
        }
        #endregion

        #endregion

        #region inner event handlers

        #region User Registed event handler
        void IEventHandler<UserRegisted>.Handle(UserRegisted @event)
        {
            this.CommendBy = @event.CommendBy;
            this.LoginName = @event.LoginName;
            this.Email = @event.Email;
            this.VipLevel = UserVipLevel.Vip0;
            this.IsOpenAuth = false;
            this.TimeZone = @event.TimeZone;
            //this.RippleAddress = @event.RippleAddress;
            //this.RippleSecret = @event.RippleSecret;
            this.Role = 0;
            this.TwoFactorFlg = 0;
            this.Mobile = string.Empty;
            this.Membership = new Membership(this, @event.Email, @event.Password, @event.TradePassword);
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }

        void IEventHandler<UserRegistedByOpenAuth>.Handle(UserRegistedByOpenAuth @event)
        {
            this.CommendBy = @event.CommendBy;
            this.LoginName = @event.LoginName;
            this.TwoFactorFlg = 0;
            this.TimeZone = 8;
            this.IsOpenAuth = true;
            //this.RippleAddress = @event.RippleAddress;
            //this.RippleSecret = @event.RippleSecret;
            this.Email = string.Empty;
            this.VipLevel = UserVipLevel.Vip0;
            this.Role = 0;
            this.Mobile = string.Empty;
            this.Membership = new Membership(this, string.Empty, @event.Password, string.Empty);
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
        }
        #endregion

        #region Set Google Authentication
        void IEventHandler<UserSetGoogleAuthentication>.Handle(UserSetGoogleAuthentication @event)
        {
            this.TwoFactorFlg |= 11;//01011打开谷歌身份验证，保留原有位置的作用，保持兼容
            if (this.GoogleAuthentication != null)
                this.GoogleAuthentication.ChangeSecret(@event.OTPSecret);
            else
                this.GoogleAuthentication = new GoogleAuthentication(this.ID, @event.OTPSecret);
        }
        #endregion

        #region  Close Google Authentication
        void IEventHandler<UserCloseGoogleAuthentication>.Handle(UserCloseGoogleAuthentication @event)
        {
            this.TwoFactorFlg &= 20;
        }
        #endregion

        #region User Password Changed event handler
        void IEventHandler<UserPasswordChanged>.Handle(UserPasswordChanged @event)
        {
            this.Membership.ChangePassword(@event.NewPassword);
        }
        #endregion

        #region User Trade Password Changed event handler
        void IEventHandler<UserTradePasswordChanged>.Handle(UserTradePasswordChanged @event)
        {
            this.Membership.ChangeTradePassword(@event.NewTradePassword);
        }
        #endregion

        #region User Set NickName event handler
        //void IEventHandler<UserNickNameChanged>.Handle(UserNickNameChanged @event)
        //{
        //    this.LoginName = @event.NickName;
        //    this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        //}
        #endregion

        #region User Reset Password By Email event handler
        void IEventHandler<UserPasswordResetByEmail>.Handle(UserPasswordResetByEmail @event)
        {
            this.Membership.ResetPassword();

            this.RaiseEvent(new UserPasswordResetedByEmail(@event.UserID, this.LoginName, @event.Email, this.Membership.PasswordResetToken));
        }
        #endregion

        #region User Reset Password By Mobile event handler
        void IEventHandler<UserPasswordResetByMobile>.Handle(UserPasswordResetByMobile @event)
        {
            this.Membership.ResetPassword();

            this.RaiseEvent(new UserPasswordResetedByMobile(@event.UserID, @event.Mobile, this.Membership.PasswordResetToken));
        }
        #endregion

        #region User First Set Trade Password event handler

        void IEventHandler<UserFirstSetTradePassword>.Handle(UserFirstSetTradePassword @event)
        {
            this.Membership.ChangeTradePassword(@event.NewTradePassword);
        }
        #endregion
        #region User Reset Trade Password event handler
        void IEventHandler<UserTradePasswordReset>.Handle(UserTradePasswordReset @event)
        {
            this.Membership.ResetTradePassword();
            this.RaiseEvent(new UserTradePasswordReseted(@event.UserID, @event.Email, this.Membership.TradePasswordResetToken));
        }
        #endregion

        #region RealName Authenticated event handler
        void IEventHandler<RealNameAuthenticated>.Handle(RealNameAuthenticated @event)
        {
            this.Membership.RealNameAuthentication(@event.RealName, @event.IdNo, @event.IdType);
        }
        #endregion

        #region User Locked event handler
        void IEventHandler<UserLocked>.Handle(UserLocked @event)
        {
            this.Membership.Lock();
        }
        #endregion

        #region User Unlocked event handler
        void IEventHandler<UserUnlocked>.Handle(UserUnlocked @event)
        {
            this.Membership.Unlock();
        }
        #endregion

        #region User Assigned Role event handler
        void IEventHandler<UserAssignedRole>.Handle(UserAssignedRole @event)
        {
            this.Role |= ((int)@event.ManagerType);
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion

        #region User Modified Mobile event handler
        void IEventHandler<UserModifiedMobile>.Handle(UserModifiedMobile @event)
        {
            this.Mobile = @event.Mobile;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.TwoFactorFlg |= 20;

            if (this.SmsAuthentication == null)
                this.SmsAuthentication = new SmsAuthentication(this.ID, @event.NewOTPSecret);
            else
                this.SmsAuthentication.SetOTPSecret(@event.NewOTPSecret);

            this.VerifySMSPassword(@event.OneTimePassword);
        }
        #endregion

        #region Open GA/SMS Login Verify and Two Factor Verify event handler
        void IEventHandler<OpenLoginGAVerify>.Handle(OpenLoginGAVerify @event)
        {
            if (@event.Open)
                this.TwoFactorFlg |= 2;
            else
                this.TwoFactorFlg &= 29;
        }

        void IEventHandler<OpenLoginSMSVerify>.Handle(OpenLoginSMSVerify @event)
        {
            if (@event.Open)
                this.TwoFactorFlg |= 4;
            else
                this.TwoFactorFlg &= 27;
        }

        void IEventHandler<OpenTwoFactorGAVerify>.Handle(OpenTwoFactorGAVerify @event)
        {
            if (@event.Open)
                this.TwoFactorFlg |= 8;
            else
                this.TwoFactorFlg &= 23;
        }

        void IEventHandler<OpenTwoFactorSMSVerify>.Handle(OpenTwoFactorSMSVerify @event)
        {
            if (@event.Open)
                this.TwoFactorFlg |= 16;
            else
                this.TwoFactorFlg &= 15;
        }
        #endregion

        #region User Score Change
        void IEventHandler<UserScoreIncrease>.Handle(UserScoreIncrease @event)
        {
            this.ScoreBalance += @event.Increase;
            this.Membership.UpdateLastReceiveScoreDate();
            OnScoreChange();
        }

        void IEventHandler<UserScoreDecrease>.Handle(UserScoreDecrease @event)
        {
            this.ScoreBalance -= @event.Decrease;
            if (this.ScoreBalance < 0) this.ScoreBalance = 0;

            OnScoreChange();
        }

        void IEventHandler<UserScoreUsed>.Handle(UserScoreUsed @event)
        {
            this.ScoreBalance -= @event.UseQuantity;
            this.ScoreUsed += @event.UseQuantity;
        }

        private void OnScoreChange()
        {
            var score = this.ScoreBalance + this.ScoreUsed;
            if (score >= 1000 && score < 5000) this.VipLevel = UserVipLevel.Vip0;
            else if (score >= 5000 && score < 10000) this.VipLevel = UserVipLevel.Vip1;
            else if (score >= 10000 && score < 50000) this.VipLevel = UserVipLevel.Vip2;
            else if (score >= 50000 && score < 100000) this.VipLevel = UserVipLevel.Vip3;
            else if (score >= 100000 && score < 300000) this.VipLevel = UserVipLevel.Vip4;
            else if (score >= 300000 && score < 500000) this.VipLevel = UserVipLevel.Vip5;
            else if (score >= 500000 && score < 1000000) this.VipLevel = UserVipLevel.Vip6;
            else if (score >= 1000000 && score < 2000000) this.VipLevel = UserVipLevel.Vip7;
            else if (score >= 2000000 && score < 5000000) this.VipLevel = UserVipLevel.Vip8;
            else if (score >= 50000000) this.VipLevel = UserVipLevel.Vip9;
        }
        #endregion

        #region user login success
        void IEventHandler<UserLoginSuccess>.Handle(UserLoginSuccess @event)
        {
            var lastReceiveLoginScoreTime = this.Membership.LastReceiveScoreAt.ToLocalDateTime();
            if (this.Membership.LastReceiveScoreAt == 0 ||
                (DateTime.Now > lastReceiveLoginScoreTime &&
                 DateTime.Now.Date != lastReceiveLoginScoreTime.Date))
            {
                this.Apply(new ReceiveScoreDaliyLogin(this.ID));
            }
        }
        #endregion

        #region 取消用户角色
        void IEventHandler<UserUnsignedRole>.Handle(UserUnsignedRole @event)
        {
            this.Role &= (2047 ^ (int)@event.ManagerType);
        }
        #endregion

        #endregion
    }
}
