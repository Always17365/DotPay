using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using DotPay.Common;

namespace DotPay.MainDomain
{
    public class Article : DomainBase, IAggregateRoot,
                                IEventHandler<ArticleCreated>,
                                IEventHandler<ArticleEdit>,
                                IEventHandler<ArticleTop>,
                                IEventHandler<ArticleUnsignTop>
    {
        #region ctor
        protected Article() { }

        public Article(ArticleCategory category, string title, string content, bool isTop, Lang lang, int createBy)
        {
            this.RaiseEvent(new ArticleCreated(category, title, content, isTop, lang, createBy));
        }
        #endregion

        #region propertis
        public virtual int ID { get; protected set; }
        public virtual string Title { get; protected set; }
        public virtual ArticleCategory Category { get; protected set; }
        public virtual string Content { get; protected set; }
        public virtual Lang Lang { get; protected set; }
        public virtual bool IsTop { get; protected set; }

        public virtual int CreateAt { get; protected set; }
        public virtual int CreateBy { get; protected set; }
        public virtual int UpdateAt { get; protected set; }
        public virtual int UpdateBy { get; protected set; }
        #endregion

        #region public methods
        public virtual void SignTop(int byUserID)
        {
            if (!this.IsTop)
                this.RaiseEvent(new ArticleTop(this.ID, byUserID));
        }

        public virtual void UnsignTop(int byUserID)
        {
            if (this.IsTop)
                this.RaiseEvent(new ArticleUnsignTop(this.ID, byUserID));
        }

        public virtual void Edit(ArticleCategory category, string title, string content, bool isTop, Lang lang, int editBy)
        {
            this.RaiseEvent(new ArticleEdit(this.ID, category, title, content, isTop, lang, editBy));
        }
        #endregion

        #region inner events handler
        void IEventHandler<ArticleCreated>.Handle(ArticleCreated @event)
        {
            this.Title = @event.Title;
            this.Content = @event.Content;
            this.Lang = @event.Lang;
            this.IsTop = @event.IsTop;
            this.Category = @event.Category;
            this.CreateBy = @event.CreateBy;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<ArticleEdit>.Handle(ArticleEdit @event)
        {
            this.Title = @event.Title;
            this.Content = @event.Content;
            this.Lang = @event.Lang;
            this.Category = @event.Category;
            this.IsTop = @event.IsTop;
            this.UpdateBy = @event.EditBy;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<ArticleTop>.Handle(ArticleTop @event)
        {
            this.IsTop = true;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.UpdateBy = @event.ByUserID;
        }

        void IEventHandler<ArticleUnsignTop>.Handle(ArticleUnsignTop @event)
        {
            this.IsTop = false;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.UpdateBy = @event.ByUserID;
        }
        #endregion
    }
}
