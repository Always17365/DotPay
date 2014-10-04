using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using FC.Framework;
using DotPay.Common;
using DotPay.ViewModel;
using FC.Framework.Utilities;

namespace DotPay.QueryService.Impl
{
    public class AnnouncementQuery : AbstractQuery, IAnnouncementQuery
    {
        object _locker = new object();
        public NoticeInListModel GetAnnouncementByID(int announcementID)
        {
            Check.Argument.IsNotNegativeOrZero(announcementID, "announcementID");

            var cacheKey = CacheKey.ANNOUNCEMENT_SINGLE_PREFIX + announcementID;
            var annoucement = Cache.Get<NoticeInListModel>(cacheKey);

            if (annoucement == null)
            {
                annoucement = this.Context.Sql(announcement_getByID_Sql)
                                          .Parameter("@id", announcementID)
                                          .QuerySingle<NoticeInListModel>();

                if (annoucement != null)
                    Cache.Add(cacheKey, annoucement);
            }

            return annoucement;
        }
        public IEnumerable<AnnouncementModel> GetAnnouncementWhichIsTop()
        {
            var annoucements = Cache.Get<IEnumerable<AnnouncementModel>>(CacheKey.ANNOUNCEMENT_TOP);

            if (annoucements == null)
            {
                annoucements = this.Context.Sql(announcement_top1_Sql).QueryMany<AnnouncementModel>();

                if (annoucements != null)
                    Cache.Add(CacheKey.ANNOUNCEMENT_TOP, annoucements);
            }

            return annoucements;
        }

        public IEnumerable<DotPay.ViewModel.NoticeInListModel> GetAnnouncementBySearch(Lang lang, int page, int pageCount)
        {
            var paramters = new object[] { (int)lang, (page - 1) * pageCount, pageCount };

            var announcement = this.Context.Sql(data_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<NoticeInListModel>();

            return announcement;
        }
        public IEnumerable<ArticleInListModel> GetTopAnnouncement(int topCount)
        {
            var annoucements = Cache.Get<IEnumerable<ArticleInListModel>>(CacheKey.ANNOUNCEMENT_TOP);

            if (annoucements == null)
            {
                annoucements = this.Context.Sql(announcement_topN_Sql)
                                            .Parameter("topN", topCount)
                                            .QueryMany<ArticleInListModel>();

                Cache.Add(CacheKey.ANNOUNCEMENT_TOP, annoucements, new TimeSpan(0, 10, 0));
            }

            return annoucements;
        }

        private readonly string announcement_getByID_Sql =
                        @"SELECT   Content,ID,IsTop,Title,CreateAt 
                                    FROM   " + Config.Table_Prefix + @"Announcement 
                                   WHERE   ID=@id
                                   ORDER BY  CreateAt  DESC";

        private readonly string announcement_top1_Sql =
                                @"SELECT   Content,ID,IsTop,Title,CreateAt 
                                    FROM   " + Config.Table_Prefix + @"Announcement 
                                   WHERE   IsTop=1
                                   ORDER BY  CreateAt  DESC";

        private readonly string announcement_topN_Sql =
                             @"SELECT   Title,Content,ID,IsTop,CreateAt 
                                 FROM   " + Config.Table_Prefix + @"Announcement    
                                ORDER   BY IsTop DESC,CreateAt DESC 
                                LIMIT   @topN ";


        private readonly string data_Sql =
                              @"SELECT   ID, Title, Content, IsTop, CreateAt, CreateBy, UpdateAt, UpdateBy
                                  FROM   " + Config.Table_Prefix + @"announcement 
                                 WHERE   Lang=@0
                             ORDER BY  CreateAt  DESC
                                LIMIT    @1,@2";

        /**********************************************************************************/
        public int CountNoticeBySearch()
        {
            return this.Context.Sql(notice_sql)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.NoticeInListModel> GetNoticeBySearch(int page, int pageCount)
        {
            var paramters = new object[] { (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getNoticeBySearch_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<NoticeInListModel>();

            return users;
        }
        public NoticeInListModel GetNoticeContent(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            var paramters = new object[] { id };

            var content = this.Context.Sql(getNoticeContent_Sql)
                                   .Parameters(paramters)
                                   .QuerySingle<NoticeInListModel>();

            return content;
        }


        #region SQL
        private readonly string notice_sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"announcement 
                                   ORDER BY  CreateAt  DESC";
        private readonly string getNoticeContent_Sql =
                                @"SELECT   ID, Title, Content, IsTop, CreateAt, CreateBy, UpdateAt, UpdateBy, Lang
                                    FROM   " + Config.Table_Prefix + @"announcement
                                   WHERE   ID = @0 
                                   ORDER BY  CreateAt  DESC";

        private readonly string getNoticeBySearch_Sql =
                                @"SELECT   ID, Title, Content, IsTop, CreateAt, CreateBy, UpdateAt, UpdateBy, Lang
                                    FROM   " + Config.Table_Prefix + @"announcement 
                                   ORDER BY  CreateAt  DESC
                                   LIMIT    @0,@1";

        #endregion
    }
}
