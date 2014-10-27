using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class AnnouncementCreated : DomainEvent
    {
        public AnnouncementCreated(string title, string content, bool isTop,Lang lang, int createBy)
        {
            this.Title = title;
            this.Content = content;
            this.Lang = lang;
            this.IsTop = isTop;
            this.CreateBy = createBy;
        }

        public bool IsTop { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public Lang Lang { get; private set; }
        public int CreateBy { get; private set; }
    }

    public class AnnouncementEdit : DomainEvent
    {
        public AnnouncementEdit(string title, string content, bool isTop, Lang lang, int editBy)
        {
            this.Title = title;
            this.Content = content;
            this.IsTop = isTop;
            this.Lang = lang;
            this.EditBy = editBy;
        }

        public bool IsTop { get; private set; }
        public Lang Lang { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int EditBy { get; private set; }
    }

    public class AnnouncementTop : DomainEvent
    {
        public AnnouncementTop(int byUserID)
        {
            this.ByUserID = byUserID;
        }
        public int ByUserID { get; private set; }
    }
    public class AnnouncementUnsignTop : DomainEvent
    {
        public AnnouncementUnsignTop(int byUserID)
        {
            this.ByUserID = byUserID;
        }
        public int ByUserID { get; private set; }
    }


}
