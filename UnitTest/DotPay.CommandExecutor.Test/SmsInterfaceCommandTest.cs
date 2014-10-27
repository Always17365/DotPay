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
using DotPay.MainDomain;

namespace DotPay.CommandExecutor.Test
{
    public class SmsInterfaceCommandTest
    {
        ICommandBus commandBus;

        public SmsInterfaceCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateAndModifySmsInterface()
        {
            var smsAccount = "sms_account";
            var smsPassword = "sms_password";
            var userID = 1;
            var smsInterfaceID = 1;

            var smsInterface = IoC.Resolve<IRepository>().FindById<SmsInterface>(smsInterfaceID);

            if (smsInterface == null)
            {
                var cmd = new CreateSmsInterface(SmsInterfaceType.IHUYI, smsAccount, smsPassword, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(cmd);
                });
                smsInterface = IoC.Resolve<IRepository>().FindById<SmsInterface>(smsInterfaceID);
                Assert.Equal(smsInterface.Account, smsAccount);
                Assert.Equal(smsInterface.SmsType, SmsInterfaceType.IHUYI);
                Assert.Equal(smsInterface.Password, smsPassword);
            }

            var smsAccount_modify =smsAccount+ "modify";
            var smsPassword_modify = smsPassword + "modify";
            var cmdModify = new ModifySmsInterface(smsInterfaceID, SmsInterfaceType.IHUYI, smsAccount_modify, smsPassword_modify, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmdModify);
            });
            smsInterface = IoC.Resolve<IRepository>().FindById<SmsInterface>(smsInterfaceID);

            Assert.Equal(smsInterface.Account, smsAccount_modify);
            Assert.Equal(smsInterface.SmsType, SmsInterfaceType.IHUYI);
            Assert.Equal(smsInterface.Password, smsPassword_modify);
        }
    }
}
