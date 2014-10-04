
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IArticleQuery
    {
        int CountArticleBySearch();
        IEnumerable<ArticleInListModel> GetArticleBySearch(int page, int pageCount);
        ArticleInListModel GetArticleById(int id);
        int CountCategoryArticleBySearch(ArticleCategory category);
        ArticleInListModel GetFristArticleByCategory(ArticleCategory category);
        IEnumerable<ArticleInListModel> GetCategoryArticleBySearch(Lang lang, ArticleCategory category,int page, int pageCount);
        IEnumerable<ArticleInListModel> GetArticleWhichIsTop(int top);


    }
}
