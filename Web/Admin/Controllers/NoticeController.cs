using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using DotPay.QueryService;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Web.Admin.Filters;
using DotPay.Common;

namespace DotPay.Web.Admin.Controllers
{
    public class NoticeController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult Notice()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult NoticeList()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult NoticeChange()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetNoticeCountBySearch()
        {
            var count = IoC.Resolve<IAnnouncementQuery>().CountNoticeBySearch();

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetNotice(int id)
        {
            var count = IoC.Resolve<IAnnouncementQuery>().GetNoticeContent(id);
            return Json(count);
        }
        [HttpPost]
        public ActionResult ChangeNotice(int id)
        {
            var count = IoC.Resolve<IAnnouncementQuery>().GetNoticeContent(id);

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetNoticeBySearch(int page)
        {
            var result = IoC.Resolve<IAnnouncementQuery>().GetNoticeBySearch(page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }
       

        [HttpPost]
        public ActionResult EditAnnouncement(int announcementID, string title, string content, bool isTop, Lang lang)
        {
            var cmd = new EditAnnouncement(announcementID,title, content, isTop, lang, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult CreateAnnouncement(string title, string content, bool isTop, Lang lang)
        {
            var cmd = new CreateAnnouncement(title, content, isTop, lang, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult GetNoticeCategory()
        {
            return Json( this.GetSelectList<Lang>() , JsonRequestBehavior.AllowGet);
        }
    }
}
