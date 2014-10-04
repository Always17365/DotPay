using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class TokenQueryTest
    {
        TokenQuery query;

        public TokenQueryTest()
        {
            TestEnvironment.Init();
            query = new TokenQuery();
        }

        [Fact]
        public void TestGetUsersCountBySearch()
        {
            Assert.DoesNotThrow(() =>
            {
                query.CountByTokenAndUserIDWhichIsNotUsed("d", 1, Common.TokenType.EmailVerify);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                query.CountByTokenAndUserIDWhichIsNotUsed(string.Empty, 1, Common.TokenType.EmailVerify);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                query.CountByTokenAndUserIDWhichIsNotUsed("d", 0, Common.TokenType.EmailVerify);
            });
        }


    }
}
