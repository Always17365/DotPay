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
using DotPay.Domain.Repository;
using DotPay.Domain;
using FC.Framework.Utilities;
using System.Threading;

namespace DotPay.CommandExecutor.Test
{
    public class TokenCommandTest
    {
        ICommandBus commandBus;

        public TokenCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }

        [Fact]
        public void TestTokenUse()
        {
            #region 生成token
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
            #endregion

            bool exist = false;
            var tokenID = 0;

            while (!exist)
            {
                tokenID = new Random().Next(1, 10);
                var existToken = IoC.Resolve<IRepository>().FindById<Token>(tokenID);

                exist = existToken != null && !existToken.IsUsed;
            }

            var tokenUse = new TokenUse(tokenID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(tokenUse);
            });

            var ex = Assert.Throws<CommandExecutionException>(delegate
                      {
                          this.commandBus.Send(tokenUse);
                      });

            Assert.Equal(ex.ErrorCode, (int)ErrorCode.TokenIsUsedOrTimeOut);

            var token = IoC.Resolve<IRepository>().FindById<Token>(tokenID);

            Assert.True(token.IsUsed);
        }


    }
}
