using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.Common;
using DotPay.Persistence;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using FC.Framework.Utilities;
using DotPay.MainDomain.Repository;
using DotPay.MainDomain;

namespace DotPay.CommandExecutor.Test
{
    public class UserCommandTest
    {
        ICommandBus commandBus;

        public UserCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }

        [Fact]
        public void TestUserRegisterAndLogin()
        {
            var password = Guid.NewGuid().Shrink();
            var email = "test" + password.GetHashCode() + "@mytest.com";
            //var rippleAddress = "test" + password.GetHashCode() + "@mytest.com";
            //var rippleSecret = "test" + password.GetHashCode() + "@mytest.com";

            var cmd = new UserRegister(Guid.NewGuid().Shrink(), email, password, password, 8, "asdjlfjadljflasdjflsjdf");
            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedUser = IoC.Resolve<IUserRepository>().FindByEmail(email);

            Assert.NotNull(savedUser);
            Assert.Equal(savedUser.Email, email);
            Assert.True(savedUser.ID > 0);
            Assert.Null(savedUser.GoogleAuthentication);
            Assert.Null(savedUser.SmsAuthentication);

            var loginCmd = new UserLogin(email, password, "192.168.0.6");

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(loginCmd);
            });
        }

        [Fact]
        public void TestUserOpenAuthLogin()
        {
            var openID = Guid.NewGuid().Shrink();

            //var cmd = new UserQQLogin(openID, "NICKNAME", "192.169.13.1");
            //Assert.DoesNotThrow(delegate
            //{
            //    this.commandBus.Send(cmd);
            //});

            //var openAuthShipQQ = IoC.Resolve<IOpenAuthShipRepository>().FindByOpenID(openID, OpenAuthType.QQ);
            //var savedQQUser = IoC.Resolve<IUserRepository>().FindById<User>(openAuthShipQQ.UserID);

            //Assert.NotNull(openAuthShipQQ);
            //Assert.NotNull(savedQQUser);
            //Assert.Equal(openAuthShipQQ.Type, OpenAuthType.QQ);
            //Assert.Equal(openAuthShipQQ.OpenID, openID);

            //Assert.DoesNotThrow(delegate
            //{
            //    this.commandBus.Send(cmd);
            //});

            //var openAuthShipQQ2 = IoC.Resolve<IOpenAuthShipRepository>().FindByOpenID(openID, OpenAuthType.QQ);

            //Assert.Equal(openAuthShipQQ.UserID, openAuthShipQQ2.UserID);
        }

        [Fact]
        public void TestUserSetNickName()
        {
            var userID = new Random().Next(1, 10);
            var nickName = "小蜜蜂";
            var cmd = new UserSetNickName(userID, nickName);
            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedUser = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotNull(savedUser);
            Assert.Equal(savedUser.ID, userID);
            Assert.Equal(savedUser.LoginName, nickName);
        }

        [Fact]
        public void TestUserGoogleAuthentication()
        {
            var userID = new Random().Next(1, 3);
            var gakey = Utilities.GenerateOTPKey();

            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);
            var otp = Utilities.GenerateGoogleAuthOTP(gakey);
            var cmd = new UserOpenGoogleAuthentication(userID, gakey, otp);

            if (user.GoogleAuthentication != null)
            {
                var ex = Assert.Throws<CommandExecutionException>(delegate
                    {
                        this.commandBus.Send(cmd);
                    });

                Assert.Equal(ex.ErrorCode, (int)ErrorCode.GoogleAuthenticationIsSetted);
            }

            else
                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(cmd);
                });

            var savedUser = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotNull(savedUser);
            Assert.NotNull(savedUser.GoogleAuthentication);
            Assert.Equal(savedUser.GoogleAuthentication.OTPSecret, gakey);
            Assert.False((savedUser.TwoFactorFlg & 2) == 2);
            Assert.False((savedUser.TwoFactorFlg & 8) == 8);
            Assert.Equal(savedUser.GoogleAuthentication.UserID, userID);

        }

        [Fact]
        public void TestUserSMSAuthentication()
        {
            var userID = new Random().Next(1, 10);
            var mobile = "1399999999";
            var smskey = Utilities.GenerateOTPKey();
            var smsCounter = 0;

            var cmd = new UserSetMobile(userID, mobile, smskey, Utilities.GenerateSmsOTP(smskey, 1));
            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedUser = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotNull(savedUser);
            Assert.NotNull(savedUser.SmsAuthentication);
            Assert.Equal(savedUser.SmsAuthentication.OTPSecret, smskey);
            Assert.Equal(savedUser.SmsAuthentication.SmsCounter, smsCounter);
            Assert.False((savedUser.TwoFactorFlg & 4) == 4);
            Assert.False((savedUser.TwoFactorFlg & 16) == 16);
            Assert.Equal(savedUser.SmsAuthentication.UserID, userID);
        }

        [Fact]
        public void TestUserPassword()
        {
            var password = Guid.NewGuid().Shrink();
            var userID = new Random().Next(4, 10);
            var email = "email" + userID + "@11.com";


            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);
            var lastVerifyAt = user.Membership.LastPasswordVerifyAt;
            var lastVerifyFailAt = user.Membership.LastPasswordFailureAt;


            Assert.NotNull(user);

            var loginCmdRight = new UserLogin(email, userID.ToString(), "192.168.0.6");
            var loginCmdError = new UserLogin(email, userID.ToString() + "1", "192.168.0.6");

            var exception = Assert.Throws<CommandExecutionException>(delegate
                         {
                             this.commandBus.Send(loginCmdError);
                         });

            Assert.Equal(exception.ErrorCode, (int)ErrorCode.LoginNameOrPasswordError);

            user = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(loginCmdRight);
            });

            var savedUser = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotEqual(lastVerifyFailAt, savedUser.Membership.LastPasswordFailureAt);
            Assert.NotEqual(lastVerifyAt, savedUser.Membership.LastPasswordVerifyAt);

            var newpassword = Guid.NewGuid().Shrink();
            var ga_otp = savedUser.GoogleAuthentication == null ? string.Empty : Utilities.GenerateGoogleAuthOTP(savedUser.GoogleAuthentication.OTPSecret);
            var sms_otp = savedUser.SmsAuthentication == null ? string.Empty : Utilities.GenerateSmsOTP(savedUser.SmsAuthentication.OTPSecret, savedUser.SmsAuthentication.SmsCounter);
            var modifyPassword = new UserModifyPassword(userID, user.ID.ToString(), newpassword);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(modifyPassword);
            });

            var loginCmdAfterModifyPassword = new UserLogin(email, newpassword, "192.168.0.6");

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(loginCmdAfterModifyPassword);
            });
        }

        [Fact]
        public void TestUserTradePassword()
        {
            var password = Guid.NewGuid().Shrink();
            var userID = new Random().Next(4, 10);
            var email = "email" + userID + "@11.com";

            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);
            var lastVerifyAt = user.Membership.LastTradePasswordVerifyAt;
            var lastVerifyFailAt = user.Membership.LastTradePasswordFailureAt;
            var modifyAt = user.Membership.TradePasswordChangeAt;

            var newpassword = Guid.NewGuid().Shrink();
            var ga_otp = user.GoogleAuthentication == null ? string.Empty : Utilities.GenerateGoogleAuthOTP(user.GoogleAuthentication.OTPSecret);
            var sms_otp = user.SmsAuthentication == null ? string.Empty : Utilities.GenerateSmsOTP(user.SmsAuthentication.OTPSecret, user.SmsAuthentication.SmsCounter);
            var modifyTradePassword = new UserModifyTradePassword(userID, user.ID.ToString(), newpassword /*, ga_otp, sms_otp*/);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(modifyTradePassword);
            });


            var verifyResult = user.VerifyTradePassword(PasswordHelper.EncryptMD5(newpassword));

            Assert.True(verifyResult);


            var exception = Assert.Throws<CommandExecutionException>(delegate
            {
                this.commandBus.Send(modifyTradePassword);
            });
            var userSaved = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotEqual(userSaved.Membership.TradePasswordChangeAt, modifyAt);
            Assert.Equal(exception.ErrorCode, (int)ErrorCode.TradePasswordError);
            Assert.NotEqual(userSaved.Membership.LastTradePasswordFailureAt, lastVerifyFailAt);
            if (string.IsNullOrEmpty(user.Membership.TradePassword))
                Assert.NotEqual(userSaved.Membership.LastTradePasswordVerifyAt, lastVerifyAt);
            else
                Assert.Equal(userSaved.Membership.LastTradePasswordVerifyAt, lastVerifyAt);
        }


        [Fact]
        public void TestUserResetPasswordAndTradePassword()
        {
            var userID = new Random().Next(1, 10);
            var email = "email" + userID + "@11.com";

            var resetPasswordCmd = new UserForgetPassword(userID);
            var resetTradePasswordCmd = new UserForgetTradePassword(userID);

            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            var oldPasswordToken = user.Membership.PasswordResetToken;
            var oldTradePasswordToken = user.Membership.TradePasswordResetToken;

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(resetPasswordCmd);
            });

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(resetTradePasswordCmd);
            });
            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(resetPasswordCmd);
            });

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(resetTradePasswordCmd);
            });

            var userSaved = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.NotEqual(oldPasswordToken, userSaved.Membership.PasswordResetToken);
            Assert.NotEqual(oldTradePasswordToken, userSaved.Membership.TradePasswordResetToken);

        }

        [Fact]
        public void TestUserLockAndUnlock()
        {
            var userID = new Random().Next(1, 10);
            var email = "email" + userID + "@11.com";

            var lockCmd = new LockUser(userID, "test reason", 1);
            var unlockCmd = new UnlockUser(userID, "test reason", 1);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(lockCmd);
            });
            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.True(user.Membership.IsLocked);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(unlockCmd);
            });

            user = IoC.Resolve<IUserRepository>().FindById<User>(userID);
            Assert.False(user.Membership.IsLocked);
        }

        [Fact]
        public void TestUserScoreIncrease()
        {
            var userID = new Random().Next(1, 10);

            var addScore = new UserScoreIncrease(userID, 100);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(addScore);
            });
            var user = IoC.Resolve<IUserRepository>().FindById<User>(userID);

            Assert.Equal(user.ScoreBalance, 100);
        }

        [Fact]
        public void TestUserAssignAndRemoveRole()
        {
            var userID = new Random().Next(1, 10);

            var assignRole = new UserAssignRole(userID, ManagerType.DepositOfficer, 1);
            var removeManager = new RemoveManager(1, 1, 1);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(assignRole);
            });

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(removeManager);
            });

        }

        [Fact]
        public void TestUserRealNameAuthentication()
        {
            var userID = new Random().Next(1, 10);
            var realName = "张三";
            var idno = "210224195506031426";
            var idType = IdNoType.IdentificationCard;

            var realNameAuth = new UserRealNameAuth(userID, realName, idType, idno);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(realNameAuth);
            });

            var savedUser = IoC.Resolve<IRepository>().FindById<User>(userID);

            Assert.Equal(savedUser.Membership.RealName, realName);
            Assert.Equal(savedUser.Membership.IdNo, idno);
            Assert.Equal(savedUser.Membership.IdNoType, idType);
        }

    }
}
