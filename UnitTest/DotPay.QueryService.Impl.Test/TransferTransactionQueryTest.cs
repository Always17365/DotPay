using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;
using DotPay.Common;
using DotPay.ViewModel;

namespace DotPay.QueryService.Impl.Test
{
    public class TransferTransactionQueryTest
    {
        TransferTransactionQuery query;

        public TransferTransactionQueryTest()
        {
            TestEnvironment.Init();
            query = new TransferTransactionQuery();
        }
         

        [Fact]
        public void TestGetUsersCountBySearch()
        {
            var count = query.GetTransferTransactionByRippleTxid("123", PayWay.Alipay);
        }

    }
}
