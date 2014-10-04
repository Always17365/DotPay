using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class UserQueryTest
    {
        UserQuery query;

        public UserQueryTest()
        {
            TestEnvironment.Init();
            query = new UserQuery();
        }

        [Fact]
        public void TestGetUsersCountBySearch()
        {
            var count1 = query.CountUserBySearch(1, string.Empty,false);
            var count2 = query.CountUserBySearch(null, string.Empty, false);

            Assert.Equal(count1, 1);
            Assert.True(count2 >= count1);
        }

        [Fact]
        public void TestGetUsersBySearch()
        {
            var users = query.GetUsersBySearch(1, string.Empty, false, 1, 10);

            Assert.Equal(users.Count(), 1);
            Assert.True(users.FirstOrDefault() == null || users.FirstOrDefault().ID == 1);
        }

        [Fact]
        public void TestGetUserByID()
        {
            var user = query.GetUserByID(1);

            Assert.NotNull(user);
            Assert.Equal(user.UserID, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                query.GetUserByID(0);
            });
        }

        //[Fact]
        //public void TestGetUserRealAuthInfo()
        //{
        //    Assert.DoesNotThrow(() =>
        //    {
        //        query.GetUserRealNameAuthInfoByID(1);
        //    });

        //    Assert.Throws<ArgumentOutOfRangeException>(() =>
        //    {
        //        query.GetUserRealNameAuthInfoByID(0);
        //    });
        //}

        [Fact]
        public void TestGetUserByEmail()
        {
            var email = "12@11.com";
            var user = query.GetUserByEmail(email);
            Assert.True(user == null || user.Email.Equals(email));
            Assert.Throws<ArgumentException>(() =>
            {
                query.GetUserByEmail(string.Empty);
            });
        }

        [Fact]
        public void TestGetUserByMobile()
        {
            var mobile = "1399999999";
            var user = query.GetUserByMobile(mobile);
            Assert.True(user == null || user.Mobile.Equals(mobile));
            Assert.Throws<ArgumentException>(() =>
            {
                query.GetUserByMobile(string.Empty);
            });
        }

        [Fact]
        public void TestCountUserByEmail()
        {
            var email = "12@11.com";
            var count = query.CountUserByEmail(email);
            Assert.True(count == 0 || count > 0);
            Assert.Throws<ArgumentException>(() =>
            {
                query.CountUserByEmail(string.Empty);
            });
        }

        [Fact]
        public void TestCountUserByMobile()
        {
            var mobile = "1399999999";
            var count = query.CountUserByMobile(mobile);
            Assert.True(count == 0 || count > 0);
            Assert.Throws<ArgumentException>(() =>
            {
                query.CountUserByMobile(string.Empty);
            });
        }


        [Fact]
        public void TestGetUserGoogleAuthenticationSecretByID()
        {
            var userID = new Random().Next(1, 10);
            var secret = query.GetUserGoogleAuthenticationSecretByID(userID);
            Assert.NotNull(secret);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                query.GetUserGoogleAuthenticationSecretByID(0);
            });
        }

        //[Fact]
        //public void TestGetUserRealNameAuthInfoByID()
        //{
        //    var realNameModel = query.GetUserRealNameAuthInfoByID(1);

        //    Assert.NotNull(realNameModel);
        //    var idTypeNum = (int)realNameModel.IdNoType;
        //    if (!string.IsNullOrEmpty(realNameModel.IdNo))
        //    {
        //        Assert.NotEqual(realNameModel.RealName, string.Empty);
        //        Assert.NotEqual(idTypeNum, 0);
        //    }
        //    else
        //    {
        //        Assert.Equal(realNameModel.RealName, string.Empty);
        //        Assert.Equal(idTypeNum, 0);
        //    }
        //}
    }
}
