using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.Common;
using DotPay.Persistence;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO; 
using FC.Framework.Utilities;
using DotPay.MainDomain;

namespace DotPay.CommandExecutor.Test
{
    public class AnnouncementCommandTest
    {
        ICommandBus commandBus;

        public AnnouncementCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateAndEditAnnouncement()
        {
            var userID = new Random().Next(1, 10);
            var title = "这是公告标题！";
            var content = "这是一段测试的文章，可能长度会很长，也可能很短，但是其实这只是一个公告，一般不会太长，按照这个长度测试，可行！";
            var cmd = new CreateAnnouncement(title, content, true, Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedAnnouncement = IoC.Resolve<IRepository>().FindById<Announcement>(1);

            Assert.NotNull(savedAnnouncement);
            Assert.Equal(savedAnnouncement.Title, title);
            Assert.Equal(savedAnnouncement.Content, content);

            var editTitle = title + "edited";
            var editContent = content + "edited";

            var cmdEdit = new EditAnnouncement(savedAnnouncement.ID, editTitle, editContent, true, Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmdEdit);
            });

            savedAnnouncement = IoC.Resolve<IRepository>().FindById<Announcement>(1);

            Assert.NotNull(savedAnnouncement);
            Assert.Equal(savedAnnouncement.Title, editTitle);
            Assert.Equal(savedAnnouncement.Content, editContent);
        }

        [Fact]
        public void TestSignTopAndCancelTopAnnouncement()
        {
            var userID = new Random().Next(1, 10);
            var title = "这是公告标题！";
            var content = "这是一段测试的文章，可能长度会很长，也可能很短，但是其实这只是一个公告，一般不会太长，按照这个长度测试，可行！";
            var cmd = new CreateAnnouncement(title, content, true, Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedAnnouncement = IoC.Resolve<IRepository>().FindById<Announcement>(1);

            if (!savedAnnouncement.IsTop)
            {
                var topCmd = new AnnouncementSignTop(savedAnnouncement.ID, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(topCmd);
                });
                savedAnnouncement = IoC.Resolve<IRepository>().FindById<Announcement>(1);
                Assert.True(savedAnnouncement.IsTop);
            }
            else
            {
                var cannelTopCmd = new AnnouncementUnsignTop(savedAnnouncement.ID, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(cannelTopCmd);
                });
                savedAnnouncement = IoC.Resolve<IRepository>().FindById<Announcement>(1);
                Assert.False(savedAnnouncement.IsTop);
            }
        }
    }
}

