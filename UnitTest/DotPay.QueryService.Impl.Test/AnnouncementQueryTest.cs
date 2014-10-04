using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Repository;
using System.Linq;

namespace DotPay.QueryService.Impl.Test
{
    public class AnnouncementQueryTest
    {
        AnnouncementQuery query;

        public AnnouncementQueryTest()
        {
            TestEnvironment.Init();
            query = new AnnouncementQuery();
        }

        [Fact]
        public void TestGetTopAnnouncement()
        {
            var announcements = query.GetAnnouncementWhichIsTop();

            if (announcements.Count() > 0)
            {
                announcements.ForEach(a =>
                {
                    Assert.True(a.IsTop);
                });
            }


        }


    }
}
