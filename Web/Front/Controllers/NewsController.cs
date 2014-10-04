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
    public class NewsController : BaseController
    {
        #region view
        [Route("~/news")]
        [Route("~/news/p{page}")]
        [AllowAnonymous]
        public ActionResult List(int? page)
        {
            ArticleCategory category = default(ArticleCategory);
            var _page = page.HasValue ? page.Value : 1;
            ViewBag.Category = category;
            var articles = IoC.Resolve<IArticleQuery>().GetCategoryArticleBySearch(this.CurrentLang, category, _page, Constants.DEFAULT_PAGE_COUNT);
            ViewBag.Articles = articles;
            ViewBag.HasPre = _page == 1 ? false : true;
            ViewBag.HasNext = articles.Count() == Constants.DEFAULT_PAGE_COUNT ? true : false;
            ViewBag.CurrentPage = _page;
            return View();
        }


        [Route("~/news-{category}/p{page}")]
        [AllowAnonymous]
        public ActionResult List(ArticleCategory category, int? page)
        {
            var _page = page.HasValue ? page.Value : 1;
            ViewBag.Category = category;
            var articles = IoC.Resolve<IArticleQuery>().GetCategoryArticleBySearch(this.CurrentLang, category, _page, Constants.DEFAULT_PAGE_COUNT);
            ViewBag.Articles = articles;
            ViewBag.HasPre = _page == 1 ? false : true;
            ViewBag.HasNext = articles.Count() == Constants.DEFAULT_PAGE_COUNT ? true : false;
            ViewBag.CurrentPage = _page;
            return View();
        }
        #endregion

        [AllowAnonymous]
        [Route("~/news-{category}-{artId}")]
        public ActionResult Detail(int artId)
        {
            var article = IoC.Resolve<IArticleQuery>().GetArticleById(artId);

            ViewBag.Article = article;
            ViewBag.CreateAt = article.CreateAt.ToLocalDateTime().ToString("yyyy-MM-dd");

            return View();
        }

        [AllowAnonymous]
        [Route("~/zhaopin")]
        public ActionResult Zhaopin()
        {
            var article = IoC.Resolve<IArticleQuery>().GetFristArticleByCategory(ArticleCategory.Recruitment);

            if (article != null)
            {
                ViewBag.Article = article;
                ViewBag.CreateAt = article.CreateAt.ToLocalDateTime().ToString("yyyy-MM-dd");
            }
            ViewBag.Category = ArticleCategory.Recruitment;

            return View();
        }

        //[AllowAnonymous]
        //[Route("~/AboutUs")]
        //public ActionResult AboutUs()
        //{
        //    var article = IoC.Resolve<IArticleQuery>().GetFristArticleByCategory(ArticleCategory.AboutUs);

        //    if (article != null)
        //    {
        //        ViewBag.Article = article;
        //        ViewBag.CreateAt = article.CreateAt.ToLocalDateTime().ToString("yyyy-MM-dd");
        //    }
        //    ViewBag.Category = ArticleCategory.AboutUs;

        //    return View();
        //}

        //[AllowAnonymous]
        //[Route("~/ContactUs")]
        //public ActionResult Contact()
        //{
        //    var article = IoC.Resolve<IArticleQuery>().GetFristArticleByCategory(ArticleCategory.Contact);

        //    if (article != null)
        //    {
        //        ViewBag.Article = article;
        //        ViewBag.CreateAt = article.CreateAt.ToLocalDateTime().ToString("yyyy-MM-dd");
        //    }
        //    ViewBag.Category = ArticleCategory.Contact;

        //    return View();
        //}
    }
}
