using FC.Framework;
using DotPay.QueryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.Common;

namespace DotPay.Web.Controllers
{
    public class AnnouncementController : BaseController
    {
        // GET: Anoncement

        [AllowAnonymous]
        [Route("~/notices")]
        [Route("~/notices-{page}")]
        public ActionResult List(int? page)
        {
            var _page = page.HasValue ? page.Value : 1;
            var acmts = IoC.Resolve<IAnnouncementQuery>().GetAnnouncementBySearch(this.CurrentLang, _page, Constants.DEFAULT_PAGE_COUNT);
            ViewBag.Announcements = acmts;
            ViewBag.IsNotice = true;
            ViewBag.HasPre = _page == 1 ? false : true;
            ViewBag.HasNext = acmts.Count() == Constants.DEFAULT_PAGE_COUNT ? true : false;
            ViewBag.CurrentPage = _page;

            return View();
        }

        // GET: Anoncement
        [AllowAnonymous]
        [Route("~/notice-{noticeID}")]
        public ActionResult Detail(int noticeID)
        {
            var announcement = IoC.Resolve<IAnnouncementQuery>().GetAnnouncementByID(noticeID);

            ViewBag.Article = announcement;
            ViewBag.CreateAt = announcement.CreateAt.ToLocalDateTime().ToString("yyyy-MM-dd");

            return View();
        }

    }
}