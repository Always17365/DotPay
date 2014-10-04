using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;
using DotPay.Common;

namespace DotPay.QueryService.Impl.Test
{
    public class CurrencyQueryTest
    {
        CurrencyQuery query;
        public CurrencyQueryTest()
        {
            TestEnvironment.Init();
            query = new CurrencyQuery();
        }

        [Fact]
        public void TestCurrencyQuery()
        {
            var currencyTypes = Enum.GetValues(typeof(CurrencyType));

            var randomNum = new Random().Next(1, currencyTypes.Length);

            var currenciesCount = query.GetCurrencyCountBySearch(currencyTypes.GetValue(randomNum).ToString(), string.Empty);
            var currencies = query.GetCurrenciesBySearch(currencyTypes.GetValue(randomNum).ToString(), string.Empty, 1, 10000);

            Assert.Equal(currencies.Count(), currenciesCount);
        }

    }
}
