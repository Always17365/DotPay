using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class PreRegistratorQueryTest
    {
        PreRegistratorQuery query;

        public PreRegistratorQueryTest()
        {
            TestEnvironment.Init();
            query = new PreRegistratorQuery();
        }

        [Fact]
        public void TestExistRegisterEmail()
        {
            var email = "12@11.com";
            var user = query.ExistRegisterEmail(email);
        }
        [Fact]
        public void TestExistRegisterEmailWithToken()
        {
            var email = "12@11.com";
            var token = "16541654165465456";
            var user = query.ExistRegisterEmailWithToken(email, token);
        }

    }
}
