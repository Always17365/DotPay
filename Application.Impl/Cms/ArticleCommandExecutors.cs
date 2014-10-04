using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class ArticleCommandExecutors : ICommandExecutor<CreateArticle>,
                                                ICommandExecutor<EditArticle>,
                                                ICommandExecutor<ArticleSignTop>,
                                                ICommandExecutor<ArticleUnsignTop>
    {
        public void Execute(CreateArticle cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = new Article(cmd.Category, cmd.Title, cmd.Content, cmd.IsTop, cmd.Lang, cmd.CreateBy);

            IoC.Resolve<IRepository>().Add(announcement);
        }

        public void Execute(EditArticle cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Article>(cmd.ArticleID);

            announcement.Edit(cmd.Category, cmd.Title, cmd.Content, cmd.IsTop, cmd.Lang, cmd.EditBy);
        }

        public void Execute(ArticleSignTop cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Article>(cmd.ArticleID);

            announcement.SignTop(cmd.ByUserID);
        }

        public void Execute(ArticleUnsignTop cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var announcement = IoC.Resolve<IRepository>().FindById<Article>(cmd.ArticleID);

            announcement.UnsignTop(cmd.ByUserID);
        }
    }
}
