using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Article Command
    [ExecuteSync]
    public class CreateArticle : FC.Framework.Command
    {
        public CreateArticle(ArticleCategory category, string title, string content, bool isTop, Lang lang, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero((int)category, "category");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");
            Check.Argument.IsNotEmpty(title, "title");
            Check.Argument.IsNotEmpty(content, "content");
            Check.Argument.IsNotNull(isTop, "isTop");

            this.Category = category;
            this.IsTop = isTop;
            this.Lang = lang;
            this.Title = title;
            this.Content = content;
            this.CreateBy = currentUserID;
        }
        public ArticleCategory Category { get; private set; }
        public int CurrencyID { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public bool IsTop { get; private set; }
        public int CreateBy { get; private set; }
        public Lang Lang { get; private set; }
    }
    #endregion

    #region Edit Article Command
    [ExecuteSync]
    public class EditArticle : FC.Framework.Command
    {
        public EditArticle(ArticleCategory category, int articleID, string title, string content, bool isTop, Lang lang, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero((int)category, "category");
            Check.Argument.IsNotNegativeOrZero(articleID, "articleID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");
            Check.Argument.IsNotEmpty(title, "title");
            Check.Argument.IsNotEmpty(content, "content");
            Check.Argument.IsNotNull(isTop, "isTop");

            this.Category = category;
            this.ArticleID = articleID;
            this.IsTop = isTop;
            this.Lang = lang;
            this.Title = title;
            this.Content = content;
            this.EditBy = currentUserID;
        }

        public ArticleCategory Category { get; private set; }
        public int ArticleID { get; private set; }
        public int CurrencyID { get; private set; }
        public string Title { get; private set; }
        public Lang Lang { get; private set; }
        public string Content { get; private set; }
        public bool IsTop { get; private set; }
        public int EditBy { get; private set; }
    }
    #endregion

    #region Article Sign Top Command
    [ExecuteSync]
    public class ArticleSignTop : FC.Framework.Command
    {
        public ArticleSignTop(int articleID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(articleID, "articleID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.ArticleID = articleID;
            this.ByUserID = currentUserID;
        }
        public int ArticleID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Article Unsign Top Command
    [ExecuteSync]
    public class ArticleUnsignTop : FC.Framework.Command
    {
        public ArticleUnsignTop(int articleID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(articleID, "articleID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.ArticleID = articleID;
            this.ByUserID = currentUserID;
        }
        public int ArticleID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
