using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    public class ArticleCreated : DomainEvent
    {
        public ArticleCreated(ArticleCategory category, string title, string content, bool isTop, Lang lang, int createBy)
        {
            this.Category = category;
            this.Title = title;
            this.Content = content;
            this.Lang = lang;
            this.IsTop = isTop;
            this.CreateBy = createBy;
        }

        public bool IsTop { get; private set; }
        public Lang Lang { get; private set; }
        public ArticleCategory Category { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int CreateBy { get; private set; }
    }

    public class ArticleEdit : DomainEvent
    {
        public ArticleEdit(int articleID, ArticleCategory category, string title, string content, bool isTop, Lang lang, int editBy)
        {
            this.ArticleID = articleID;
            this.Category = category;
            this.Lang = lang;
            this.Title = title;
            this.Content = content;
            this.IsTop = isTop;
            this.EditBy = editBy;
        }

        public int ArticleID { get; private set; }
        public ArticleCategory Category { get; private set; }
        public Lang Lang { get; private set; }
        public bool IsTop { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int EditBy { get; private set; }
    }

    public class ArticleTop : DomainEvent
    {
        public ArticleTop(int articleID, int byUserID)
        {
            this.ArticleID = articleID;
            this.ByUserID = byUserID;
        }
        public int ArticleID { get; private set; }
        public int ByUserID { get; private set; }
    }
    public class ArticleUnsignTop : DomainEvent
    {
        public ArticleUnsignTop(int articleID, int byUserID)
        {
            this.ArticleID = articleID;
            this.ByUserID = byUserID;
        }
        public int ArticleID { get; private set; }
        public int ByUserID { get; private set; }
    }


}
