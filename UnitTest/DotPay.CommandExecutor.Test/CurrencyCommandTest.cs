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
    public class CurrencyCommandTest
    {
        ICommandBus commandBus;

        public CurrencyCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateCurrency()
        {
            var userID = new Random().Next(1, 10);
            var values = Enum.GetValues(typeof(CurrencyType));
            var randomNum = new Random().Next(1, values.Length);
            var currency = default(Currency);
            var currencyID = 0;
            while (true)
            {
                currencyID = (int)values.GetValue(randomNum);
                var currencyCode = values.GetValue(randomNum).ToString();
                currency = IoC.Resolve<IRepository>().FindById<Currency>(currencyID);
                var createCommand = new CreateCurrency(currencyID, currencyCode, currencyCode, userID);

                if (currency == null)
                {
                    Assert.DoesNotThrow(() =>
                    {
                        this.commandBus.Send(createCommand);
                    });
                    break;
                }
            }

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currencyID);

            Assert.NotNull(savedCurrency);
        }

        [Fact]
        public void TestEnableAndDisableCurrency()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();

            var enableCmd = new EnableCurrency(currency.ID, userID);
            var disableCmd = new DisableCurrency(currency.ID, userID);

            Assert.DoesNotThrow(() => { this.commandBus.Send(disableCmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);
            Assert.False(savedCurrency.IsEnable);

            Assert.DoesNotThrow(() => { this.commandBus.Send(enableCmd); });

            savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);
            Assert.True(savedCurrency.IsEnable);

        }

        [Fact]
        public void TestModifyCurrencyDepositFeeRate()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();
            var depositFixedFee = 2M;
            var depositFeeRate = 0.002M;

            var cmd = new ModifyCurrencyDepositFeeRate(currency.ID, depositFixedFee, depositFeeRate, userID);

            Assert.DoesNotThrow(() => { this.commandBus.Send(cmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);

            Assert.Equal(savedCurrency.DepositFixedFee, depositFixedFee);
            Assert.Equal(savedCurrency.DepositFeeRate, depositFeeRate);
        }

        [Fact]
        public void TestModifyCurrencyWithdrawFeeRate()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();
            var withdrawFixedFee = 2M;
            var withdrawFeeRate = 0.002M;

            var cmd = new ModifyCurrencyWithdrawFeeRate(currency.ID, withdrawFixedFee, withdrawFeeRate, userID);

            Assert.DoesNotThrow(() => { this.commandBus.Send(cmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);

            Assert.Equal(savedCurrency.WithdrawFixedFee, withdrawFixedFee);
            Assert.Equal(savedCurrency.WithdrawFeeRate, withdrawFeeRate);
        }

        [Fact]
        public void TestModifyCurrencyNeedConfirm()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();
            var needConfirm = 6;

            var cmd = new ModifyCurrencyNeedConfirm(currency.ID, needConfirm, userID);

            Assert.DoesNotThrow(() => { this.commandBus.Send(cmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);

            Assert.Equal(savedCurrency.NeedConfirm, needConfirm);
        }

        [Fact]
        public void TestModifyCurrencyWithdrawVerifyLine()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();
            var verifyLine = 50000;

            var cmd = new ModifyCurrencyWithdrawVerifyLine(currency.ID, verifyLine, userID);

            Assert.DoesNotThrow(() => { this.commandBus.Send(cmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);

            Assert.Equal(savedCurrency.WithdrawVerifyLine, verifyLine);
        }

        [Fact]
        [Trait("dayLimit", "sd")]
        public void TestModifyCurrencyWithdrawDayLimit()
        {
            var userID = new Random().Next(1, 10);
            var currency = GetOneCurrency();
            var dayLimit = 50000;

            var cmd = new ModifyCurrencyWithdrawDayLimit(currency.ID, dayLimit, userID);

            if (!currency.IsEnable)
            {
                var ex = Assert.Throws<CommandExecutionException>(() => { this.commandBus.Send(cmd); });
                Assert.Equal(ex.ErrorCode, (int)ErrorCode.CurrencyIsDisabled);
                this.commandBus.Send(new EnableCurrency(currency.ID, userID));
            }

            Assert.DoesNotThrow(() => { this.commandBus.Send(cmd); });

            var savedCurrency = IoC.Resolve<IRepository>().FindById<Currency>(currency.ID);

            Assert.Equal(savedCurrency.WithdrawDayLimit, dayLimit);
        }

        #region 私有方法
        private Currency GetOneCurrency()
        {
            var userID = new Random().Next(1, 10);
            var values = Enum.GetValues(typeof(CurrencyType));
            var randomNum = new Random().Next(1, values.Length);
            var currency = default(Currency);

            randomNum = new Random().Next(1, values.Length);

            var currencyID = (int)values.GetValue(randomNum);
            var currencyCode = values.GetValue(randomNum).ToString();
            currency = IoC.Resolve<IRepository>().FindById<Currency>(currencyID);
            if (currency == null)
            {
                var createCommand = new CreateCurrency(currencyID, currencyCode, currencyCode, userID);
                this.commandBus.Send(createCommand);
                currency = IoC.Resolve<IRepository>().FindById<Currency>(currencyID);
            }
            return currency;
        }
        #endregion
    }
}
