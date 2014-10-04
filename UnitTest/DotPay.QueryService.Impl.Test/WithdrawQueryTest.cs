using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;
using DotPay.Common;

namespace DotPay.QueryService.Impl.Test
{
    public class WithdrawQueryTest
    {
        WithdrawQuery query;

        public WithdrawQueryTest()
        {
            TestEnvironment.Init();
            query = new WithdrawQuery();
        }

        [Fact]
        public void DepositQuery()
        {
            Assert.DoesNotThrow(() =>
            {
                query.CountWithdrawBySearch(null, "", WithdrawState.Complete);
            });

            Assert.DoesNotThrow(() =>
            {
                query.GetWithdrawBySearch(null, "", WithdrawState.Complete, 1, 10);
            });
          
                       
        }


    }
}
