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

namespace DotPay.CommandExecutor.Test
{
    public class ReceivePaymentTransactionCommandTest
    {
        ICommandBus commandBus;

        public ReceivePaymentTransactionCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateReceivePaymentTransactionAndConfirm()
        {
            var userID = new Random().Next(1, 10);
            var paymentAddress = Guid.NewGuid().Shrink();

            var currencyLtc = IoC.Resolve<IRepository>().FindById<Currency>((int)(CurrencyType.LTC));

            if (currencyLtc == null)
                this.commandBus.Send(new CreateCurrency((int)(CurrencyType.LTC), CurrencyType.LTC.ToString(), CurrencyType.LTC.ToString(), userID));

            this.commandBus.Send(new CreatePaymentAddress(userID, paymentAddress, CurrencyType.LTC));


            var txid = Guid.NewGuid().Shrink() + Guid.NewGuid().Shrink();
            var amount = new Random().Next(1, 1000);

            var cmd = new CreateReceivePaymentTransaction(txid, paymentAddress, amount, CurrencyType.LTC);

            Assert.DoesNotThrow(() =>
            {
                this.commandBus.Send(cmd);
            });

            var cmd_Confirm = new ConfirmReceivePaymentTransaction(txid, paymentAddress, 6, amount, CurrencyType.LTC);

            Assert.DoesNotThrow(() =>
            {
                this.commandBus.Send(cmd_Confirm);
            });

        }
    }
}

