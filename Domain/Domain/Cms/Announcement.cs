using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;
using DotPay.Common;

namespace DotPay.Domain
{
    public class Announcement : DomainBase, IAggregateRoot,
                                IEventHandler<AnnouncementCreated>,
                                IEventHandler<AnnouncementEdit>,
                                IEventHandler<AnnouncementTop>,
                                IEventHandler<AnnouncementUnsignTop>
    {
        #region ctor
        protected Announcement() { }

        public Announcement(string title, string content, bool isTop, Lang lang, int createBy)
        {
            this.RaiseEvent(new AnnouncementCreated(title, content, isTop, lang, createBy));
        }
        #endregion

        #region propertis
        public virtual int ID { get; protected set; }
        public virtual string Title { get; protected set; }
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
                this.RaiseEvent(new AnnouncementTop(byUserID));
        }

        public virtual void UnsignTop(int byUserID)
        {
            if (this.IsTop)
                this.RaiseEvent(new AnnouncementUnsignTop(byUserID));
        }

        public virtual void Edit(string title, string content, bool isTop, Lang lang, int editBy)
        {
            this.RaiseEvent(new AnnouncementEdit(title, content, isTop, lang, editBy));
        }
        #endregion

        #region inner events handler
        void IEventHandler<AnnouncementCreated>.Handle(AnnouncementCreated @event)
        {
            this.Title = @event.Title;
            this.Content = @event.Content;
            this.IsTop = @event.IsTop;
            this.Lang = @event.Lang;
            this.CreateBy = @event.CreateBy;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<AnnouncementEdit>.Handle(AnnouncementEdit @event)
        {
            this.Title = @event.Title;
            this.Content = @event.Content;
            this.IsTop = @event.IsTop;
            this.Lang = @event.Lang;
            this.UpdateBy = @event.EditBy;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        void IEventHandler<AnnouncementTop>.Handle(AnnouncementTop @event)
        {
            this.IsTop = true;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.UpdateBy = @event.ByUserID;
        }

        void IEventHandler<AnnouncementUnsignTop>.Handle(AnnouncementUnsignTop @event)
        {
            this.IsTop = false;
            this.UpdateAt = @event.UTCTimestamp.ToUnixTimestamp();
            this.UpdateBy = @event.ByUserID;
        }
        #endregion
    }
}
