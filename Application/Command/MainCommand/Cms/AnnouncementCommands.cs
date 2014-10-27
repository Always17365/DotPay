using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Announcement Command
    [ExecuteSync]
    public class CreateAnnouncement : FC.Framework.Command
    {
        public CreateAnnouncement(string title, string content, bool isTop,Lang lang, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");
            Check.Argument.IsNotEmpty(title, "title");
            Check.Argument.IsNotEmpty(content, "content"); 
            Check.Argument.IsNotNull(isTop, "isTop");

            this.IsTop = isTop;
            this.Title = title;
            this.Content = content;
            this.Lang = lang;
            this.CreateBy = currentUserID;
        }
        public int CurrencyID { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public bool IsTop { get; private set; }
        public int CreateBy { get; private set; } 
        public Lang Lang { get; private set; }
    }
    #endregion

    #region Edit Announcement Command
    [ExecuteSync]
    public class EditAnnouncement : FC.Framework.Command
    {
        public EditAnnouncement(int announcementID, string title, string content, bool isTop, Lang lang, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(announcementID, "announcementID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");
            Check.Argument.IsNotEmpty(title, "title");
            Check.Argument.IsNotEmpty(content, "content");
            Check.Argument.IsNotNull(isTop, "isTop");

            this.AnnouncementID = announcementID;
            this.IsTop = isTop;
            this.Title = title;
            this.Lang = lang;
            this.Content = content;
            this.EditBy = currentUserID;
        }

        public int AnnouncementID { get; private set; }
        public int CurrencyID { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public bool IsTop { get; private set; }
        public int EditBy { get; private set; }
        public Lang Lang { get; private set; }
    }
    #endregion

    #region Announcement Sign Top Command
    [ExecuteSync]
    public class AnnouncementSignTop : FC.Framework.Command
    {
        public AnnouncementSignTop(int announcementID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(announcementID, "announcementID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.AnnouncementID = announcementID;
            this.ByUserID = currentUserID;
        }
        public int AnnouncementID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Announcement Unsign Top Command
    [ExecuteSync]
    public class AnnouncementUnsignTop : FC.Framework.Command
    {
        public AnnouncementUnsignTop(int announcementID, int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero(announcementID, "announcementID");
            Check.Argument.IsNotNegativeOrZero(currentUserID, "currentUserID");

            this.AnnouncementID = announcementID;
            this.ByUserID = currentUserID;
        }
        public int AnnouncementID { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
