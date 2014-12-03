using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class TransactionQueryTest
    {
        TransactionQuery query;

        public TransactionQueryTest()
        {
            TestEnvironment.Init();
            query = new TransactionQuery();
        }

        [Fact]
        public void TestGetUsersCountBySearch()
        {
            var count = query.GetUserTransactions(1,10,1);
             
        }

    }
}
