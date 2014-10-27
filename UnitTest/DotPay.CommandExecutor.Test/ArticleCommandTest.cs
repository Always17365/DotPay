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
    public class ArticleCommandTest
    {
        ICommandBus commandBus;

        public ArticleCommandTest()
        {
            TestEnvironment.Init();
            this.commandBus = IoC.Resolve<ICommandBus>();
        }


        [Fact]
        public void TestCreateAndEditArticle()
        {
            var userID = new Random().Next(1, 10);
            var title = "这是文章标题！";
            var content = @"<p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>  
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>     
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>";
            var cmd = new CreateArticle(ArticleCategory.Common, title, content, true, Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedArticle = IoC.Resolve<IRepository>().FindById<Article>(1);

            Assert.NotNull(savedArticle);
            Assert.Equal(savedArticle.Title, title);
            Assert.Equal(savedArticle.Content, content);

            var editTitle = title + "edited";
            var editContent = content + "edited";

            var cmdEdit = new EditArticle(ArticleCategory.Common, savedArticle.ID, editTitle, editContent, true,Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmdEdit);
            });

            savedArticle = IoC.Resolve<IRepository>().FindById<Article>(1);

            Assert.NotNull(savedArticle);
            Assert.Equal(savedArticle.Title, editTitle);
            Assert.Equal(savedArticle.Content, editContent);
        }

        [Fact]
        public void TestSignTopAndCancelTopArticle()
        {
            var userID = new Random().Next(1, 10);
            var title = "这是文章标题！";
            var content = @"<p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>  
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>     
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>
                            <p>这是一段测试的文章，可能长度会很长，也可能很短！</p><p>这是一段测试的文章，可能长度会很长，也可能很短！</p>";
            var cmd = new CreateArticle(ArticleCategory.Contact, title, content, true, Lang.ZH_CN, userID);

            Assert.DoesNotThrow(delegate
            {
                this.commandBus.Send(cmd);
            });

            var savedArticle = IoC.Resolve<IRepository>().FindById<Article>(1);

            if (!savedArticle.IsTop)
            {
                var topCmd = new ArticleSignTop(savedArticle.ID, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(topCmd);
                });
                savedArticle = IoC.Resolve<IRepository>().FindById<Article>(1);
                Assert.True(savedArticle.IsTop);
            }
            else
            {
                var cannelTopCmd = new ArticleUnsignTop(savedArticle.ID, userID);

                Assert.DoesNotThrow(delegate
                {
                    this.commandBus.Send(cannelTopCmd);
                });
                savedArticle = IoC.Resolve<IRepository>().FindById<Article>(1);
                Assert.False(savedArticle.IsTop);
            }
        }
    }
}

