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
    public class CapitalAccountCommandTest
    {
        ICommandBus commandBus;

        public CapitalAccountCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateAndEnableAndDisableCapitalAccount()
        {
            var account = "6221234664962155796315".Substring(0, 19);
            var accountOwner = "张三";
            var createBy = 1;
            var capitalAccountID = 1;

            var createCommand = new CreateCapitalAccount(account, accountOwner, Bank.ICBC, createBy);

            Assert.DoesNotThrow(() =>
            {
                this.commandBus.Send(createCommand);
            });

            var enableCmd = new EnableCapitalAccount(capitalAccountID, createBy);
            var disableCmd = new DisableCapitalAccount(capitalAccountID, createBy);

            Assert.DoesNotThrow(() =>
            {
                this.commandBus.Send(enableCmd);
            });

            var enableCapitalAccount = IoC.Resolve<IRepository>().FindById<CapitalAccount>(capitalAccountID);

            Assert.True(enableCapitalAccount.IsEnable);


            Assert.DoesNotThrow(() =>
            {
                this.commandBus.Send(disableCmd);
            });

            var disableCapitalAccount = IoC.Resolve<IRepository>().FindById<CapitalAccount>(capitalAccountID);

            Assert.False(enableCapitalAccount.IsEnable);
        }


    }
}