using FC.Framework;
using DotPay.Common;
using DotPay.Command;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using QConnectSDK.Context;
using QConnectSDK;
using System.IO;

namespace DotPay.Web.Controllers
{
    public class ArticleController : BaseController
    {


        #region view
        [Route("~/article/{id}")]
        [AllowAnonymous]
        public ActionResult Index(int id)
        { 
            var article = IoC.Resolve<IArticleQuery>().GetArticleById(id);
            ViewBag.Article = article;
            return View();
        }


        [Route("~/articletop")]
        [AllowAnonymous]
        public ActionResult ArticleTop()
        {
            var article = IoC.Resolve<IArticleQuery>().GetArticleWhichIsTop(5);
            ViewBag.Article = article;
            return View();
        }
        [Route("~/getarticle/{category}/{page}")]
        [AllowAnonymous]
        public ActionResult Index(ArticleCategory category, int page)
        {
            var count = IoC.Resolve<IArticleQuery>().CountCategoryArticleBySearch(category);
            var articles = IoC.Resolve<IArticleQuery>().GetCategoryArticleBySearch(this.CurrentLang,category, page, Constants.DEFAULT_PAGE_COUNT);

            ViewBag.Count = count;
            ViewBag.Article = articles;
            return View();
        }

        #endregion 

    }
}
