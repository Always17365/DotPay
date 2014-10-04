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
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{
    public class ArticleController : BaseController
    {

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult Article()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult ArticleList()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Editor)]
        public ActionResult ArticleChange()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetArticleCountBySearch()
        {
            var count = IoC.Resolve<IArticleQuery>().CountArticleBySearch();

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetArticle(int id)
        {
            var count = IoC.Resolve<IArticleQuery>().GetArticleById(id);
            return Json(count);
        }
        [HttpPost]
        public ActionResult ChangeArticle(int id)
        {
            var count = IoC.Resolve<IArticleQuery>().GetArticleById(id);

            return Json(count);
        }
        [HttpPost]
        public ActionResult GetArticleBySearch(int page)
        {
            var result = IoC.Resolve<IArticleQuery>().GetArticleBySearch(page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }
        
        [HttpPost]
        public ActionResult CreateArticle(ArticleCategory category, string title, string content, bool isTop, Lang lang)
        {
            var cmd = new CreateArticle(category, title, content, isTop, lang, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult EditArticle(ArticleCategory category, int articleID, string title, string content, bool isTop, Lang lang)
        {
            var cmd = new EditArticle(category, articleID, title, content, isTop, lang, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }
        [HttpPost]
        public ActionResult GetArticleCategory()
        {
            return Json(new { articleCategory = this.GetSelectList<ArticleCategory>(),lang=this.GetSelectList<Lang>() }, JsonRequestBehavior.AllowGet); 
        }
    }
}
