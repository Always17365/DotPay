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

namespace DotPay.CommandExecutor.Test
{
    public class AccountCommandTest
    {
        ICommandBus commandBus;

        public AccountCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateAccount()
        {
            var userID = new Random().Next(1, 10);

            var values = Enum.GetValues(typeof(CurrencyType));

            foreach (var value in values)
            {
                var currency = value.ToString().ToEnum<CurrencyType>();
                var cmd = new CreateAccount(currency, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(cmd);
                });

                var savedAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(userID, currency);

                Assert.NotNull(savedAccount);
            }
        }
    }
}
