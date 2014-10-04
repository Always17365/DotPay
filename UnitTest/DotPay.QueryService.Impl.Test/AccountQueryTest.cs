using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;
using DotPay.Common;

namespace DotPay.QueryService.Impl.Test
{
    public class AccountQueryTest
    {
        AccountQuery query;
        public AccountQueryTest()
        {
            TestEnvironment.Init();
            query = new AccountQuery();

        }

        [Fact]
        public void TestGetAccountsByUserID()
        {
            var accounts = query.GetAccountsByUserID(1);
            var currencyTypesQuantity = Enum.GetValues(typeof(CurrencyType)).Length;
            var currencies = IoC.Resolve<ICurrencyQuery>().GetAllCurrencies();

            if (currencies != null&&currencies.Count()>0)
            {
                Assert.NotNull(accounts);
                Assert.Equal(currencies.Count(), accounts.Count());
                Assert.True(accounts.Count() <= currencyTypesQuantity);
            }

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                query.GetAccountsByUserID(0);
            });
        }

    }
}
