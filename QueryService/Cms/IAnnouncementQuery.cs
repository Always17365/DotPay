using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IAnnouncementQuery
    {
        IEnumerable<AnnouncementModel> GetAnnouncementWhichIsTop();
        IEnumerable<ArticleInListModel> GetTopAnnouncement(int topCount);
        IEnumerable<NoticeInListModel> GetAnnouncementBySearch(Lang lang,int page, int pageCount);
        NoticeInListModel GetAnnouncementByID(int announcementID);
       

        /***********************************Notice***************************************/
        int CountNoticeBySearch();
        IEnumerable<NoticeInListModel> GetNoticeBySearch(int page, int pageCount);
        NoticeInListModel GetNoticeContent(int id);
    }
}