using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class DepositQueryTest
    {
        DepositQuery query;

        public DepositQueryTest()
        {
            TestEnvironment.Init();
            query = new DepositQuery();
        } 
        [Fact]
        public void DepositQuery()
        {
            Assert.DoesNotThrow(() =>
            {
                query.CountDepositBySearch(null,"");
            });

            Assert.DoesNotThrow(() =>
            {
                query.GetDepositBySearch(null, "", 1, 3);
            });
          
                       
        }


    }
}
