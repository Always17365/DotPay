using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class CapitalAccountTest
    {
        CapitalAccountQuery query;

        public CapitalAccountTest()
        {
            TestEnvironment.Init();
            query = new CapitalAccountQuery();
        }

        [Fact]
        public void DepositQuery()
        {
            Assert.DoesNotThrow(() =>
            {
                query.CountCapitalAccountBySearch("");
            });

            Assert.DoesNotThrow(() =>
            {
                query.GetCapitalAccountBySearch(null,1, 3);
            });
          
                       
        }


    }
}
