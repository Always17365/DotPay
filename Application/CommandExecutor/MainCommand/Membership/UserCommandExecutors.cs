using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class UserCommandExecutors : ICommandExecutor<UserRegister>,                            //用户注册
                                        ICommandExecutor<UserLogin>,                               //用户登录   
                                        ICommandExecutor<UserQQLogin>,                             //用户使用QQ登录
                                        ICommandExecutor<UserWeiboLogin>,                          //用户使用微博登录
        //ICommandExecutor<UserSetNickName>,                         //修改昵称   
                                        ICommandExecutor<UserRealNameAuth>,                        //实名认证
                                        ICommandExecutor<UserModifyPassword>,                      //修改密码
                                        ICommandExecutor<UserResetPasswordByTwoFactor>,            //修改密码 --通过2FA  
                                        ICommandExecutor<UserResetPasswordByEmailToken>,           //修改密码 --通过Email Token
                                        ICommandExecutor<UserFirstTimeSetTradePassword>,           //用户第一次设置交易密码
                                        ICommandExecutor<UserResetTradePasswordByTwoFactor>,       //修改交易密码 --通过双重身份验证   
                                        ICommandExecutor<UserResetTradePasswordByEmailToken>,      //修改交易密码 --通过Email Token
                                        ICommandExecutor<UserModifyTradePassword>,                 //修改交易密码
                                        ICommandExecutor<UserForgetPassword>,                      //忘记密码
                                        ICommandExecutor<UserForgetTradePassword>,                 //忘记交易密码
                                        ICommandExecutor<UserOpenGoogleAuthentication>,            //设置谷歌双重身份验证
                                        ICommandExecutor<UserCloseGoogleAuthentication>,           //关闭谷歌双重身份验证
                                        ICommandExecutor<UserSetMobile>,                           //设置手机号 
                                        ICommandExecutor<UserOpenLoginTwoFactor>,                  //打开用户登录时的双重身份验证
                                        ICommandExecutor<UserCloseLoginTwoFactor>,                 //关闭用户登录时的双重身份验证
                                        ICommandExecutor<UserScoreIncrease>,                       //用户积分增加
                                        ICommandExecutor<UserPreRegister>,                         //用户预注册               
        //ICommandExecutor<UserVerifySMSPassword>,                //验证短信验证码                
                                        ICommandExecutor<UserAssignRole>,                          //给用户分配管理角色
                                        ICommandExecutor<RemoveManager>,                           //删除管理员（取消用户的管理角色）
                                        ICommandExecutor<LockUser>,                                //锁定用户
                                        ICommandExecutor<UnlockUser>,                              //解除用户锁定
                                        ICommandExecutor<SmsCounterCommand>                        //短信计数器
    {
        public void Execute(UserRegister cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");
            var preRegistration = IoC.Resolve<IPreRegistrationRepository>().FindByEmail(cmd.Email);

            preRegistration.Verify(cmd.RegisterToken);

            var user = new User(cmd.CommendBy, cmd.LoginName.ToLower(), cmd.Email.ToLower(), PasswordHelper.EncryptMD5(cmd.Password), PasswordHelper.EncryptMD5(cmd.TradePassword), cmd.TimeZone);

            IoC.Resolve<IUserRepository>().Add(user);
        }

        public void Execute(UserLogin cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = default(User);

            if (cmd.LoginName.IsEmail())
                user = userRepos.FindByEmail(cmd.LoginName.ToLower());
            else
                user = userRepos.FindByLoginName(cmd.LoginName.ToLower());

            if (user == null)
                throw new UserVerifyLoginPasswordException();

            user.Login(PasswordHelper.EncryptMD5(cmd.Password), cmd.IP);
        }

        public void Execute(UserQQLogin cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var openAuthType = OpenAuthType.QQ;
            var loginName = cmd.OpenID.ToLower();

            var commonRepos = IoC.Resolve<IRepository>();
            var openAuthRepos = IoC.Resolve<IOpenAuthShipRepository>();
            var openAuthShip = openAuthRepos.FindByOpenID(cmd.OpenID, openAuthType);

            if (openAuthShip == null)
            {
                var randomPassword = PasswordHelper.EncryptMD5(Guid.NewGuid().ToString());
                var user = new User(cmd.CommendBy, loginName, randomPassword, /*cmd.RippleAddress, cmd.RippleSecret,*/ openAuthType);

                commonRepos.Add(user);

                openAuthShip = new OpenAuthShip(user.ID, cmd.OpenID, openAuthType);
                commonRepos.Add(openAuthShip);
            }
            else
            {
                var user = commonRepos.FindById<User>(openAuthShip.UserID);
                user.OpenAuthLogin(cmd.IP);
            }
        }

        public void Execute(UserWeiboLogin cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var openAuthType = OpenAuthType.WEIBO;
            var loginName = cmd.OpenID.ToLower();

            var commonRepos = IoC.Resolve<IRepository>();
            var openAuthRepos = IoC.Resolve<IOpenAuthShipRepository>();
            var openAuthShip = openAuthRepos.FindByOpenID(cmd.OpenID, openAuthType);

            if (openAuthShip == null)
            {
                var randomPassword = PasswordHelper.EncryptMD5(Guid.NewGuid().ToString());
                var user = new User(cmd.CommendBy, loginName, randomPassword, /*cmd.RippleAddress, cmd.RippleSecret,*/ openAuthType);

                commonRepos.Add(user);

                openAuthShip = new OpenAuthShip(user.ID, cmd.OpenID, openAuthType);
                commonRepos.Add(openAuthShip);
            }
            else
            {
                var user = commonRepos.FindById<User>(openAuthShip.UserID);
                user.OpenAuthLogin(cmd.IP);
            }
        }

        public void Execute(UserModifyPassword cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.ChangePassword(PasswordHelper.EncryptMD5(cmd.OldPassword), PasswordHelper.EncryptMD5(cmd.NewPassword));
        }

        public void Execute(UserResetPasswordByTwoFactor cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.SetNewPasswordBy2FA(PasswordHelper.EncryptMD5(cmd.NewPassword), cmd.OneTimePassword_GA, cmd.OneTimePassword_Sms);
        }

        public void Execute(UserResetPasswordByEmailToken cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.SetNewPasswordWithEmailToken(PasswordHelper.EncryptMD5(cmd.NewPassword), cmd.EmailToken);
        }

        public void Execute(UserModifyTradePassword cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            var oldTradePassword = string.IsNullOrEmpty(cmd.OldTradePassword) ? string.Empty : PasswordHelper.EncryptMD5(cmd.OldTradePassword);

            user.ChangeTradePassword(oldTradePassword,
                                 PasswordHelper.EncryptMD5(cmd.NewTradePassword),
                                 cmd.OneTimePassword_GA,
                                 cmd.OneTimePassword_SMS);
        }


        public void Execute(UserForgetPassword cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.ResetPasswordByEmail();
        }

        public void Execute(UserForgetTradePassword cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.ResetTradePassword();
        }

        public void Execute(UserOpenGoogleAuthentication cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.SetGoogleAuthentication(cmd.GoogleSecret, cmd.OneTimePassword);
        }

        public void Execute(UserCloseGoogleAuthentication cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.CloseGoogleAuthentication(cmd.OneTimePassword);
        }

        public void Execute(UserSetMobile cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.SetMobile(cmd.Mobile, cmd.NewOTPSecret, cmd.OneTimePassword);
        }

        //public void Execute(UserSetNickName cmd)
        //{
        //    Check.Argument.IsNotNull(cmd, "cmd");

        //    var userRepos = IoC.Resolve<IUserRepository>();
        //    var user = userRepos.FindById<User>(cmd.UserID);

        //    user.SetNickName(cmd.NickName);
        //}
        public void Execute(UserOpenLoginTwoFactor cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.OpenLoginGAVerify(true, cmd.GaOtp);
            user.OpenLoginSMSVerify(true, cmd.SmsOtp);
        }

        public void Execute(UserCloseLoginTwoFactor cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.OpenLoginGAVerify(false, cmd.GaOtp);
            user.OpenLoginSMSVerify(false, cmd.SmsOtp);
        }

        public void Execute(UserScoreIncrease cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var userRepos = IoC.Resolve<IUserRepository>();
            var user = userRepos.FindById<User>(cmd.UserID);

            user.ScoreIncrease(cmd.Increase);
        }

        public void Execute(UserAssignRole cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.Assign(cmd.ManagerType, cmd.CurrentUserID);
        }

        public void Execute(UnlockUser cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.Unlock(cmd.Reason, cmd.CurrentUserID);
        }


        public void Execute(RemoveManager cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");
            var repos = IoC.Resolve<IManagerRepository>();
            var managerUser = repos.FindById<User>(cmd.UserID);
            var manager = repos.FindById<Manager>(cmd.ManagerID);

            managerUser.Unsign(manager.Type, cmd.CurrentUserID);
        }

        public void Execute(LockUser cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.Lock(cmd.Reason, cmd.CurrentUserID);
        }

        public void Execute(UserRealNameAuth cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.RealNameAuthentication(cmd.RealName, cmd.IdNo, cmd.IdType);
        }
        public void Execute(SmsCounterCommand cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.SmsAuthentication.CounterAdd();
        }
        public void Execute(UserResetTradePasswordByTwoFactor cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.SetNewTradePasswordWithTwoFactor(PasswordHelper.EncryptMD5(cmd.NewPassword), cmd.OneTimePassword_GA, cmd.OneTimePassword_SMS);
        }

        public void Execute(UserResetTradePasswordByEmailToken cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.SetNewTradePasswordWithEmailToken(PasswordHelper.EncryptMD5(cmd.NewTradePassword), cmd.EmailToken);
        }

        public void Execute(UserFirstTimeSetTradePassword cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var user = IoC.Resolve<IUserRepository>().FindById<User>(cmd.UserID);

            user.FirstSetTradePassword(PasswordHelper.EncryptMD5(cmd.TradePassword), cmd.OneTimePassword_GA, cmd.OneTimePassword_SMS);
        }

        public void Execute(UserPreRegister cmd)
        {
            var repos = IoC.Resolve<IPreRegistrationRepository>();

            PreRegistration preregistration = repos.FindByEmail(cmd.Email);

            if (preregistration == null)
            {
                preregistration = new PreRegistration(cmd.Email);
                IoC.Resolve<IRepository>().Add(preregistration);
            }
            else
            {
                preregistration.Refresh();
            }

        }
    }
}