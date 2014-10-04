using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class ArticleQuery : AbstractQuery, IArticleQuery
    {
        public int CountArticleBySearch()
        {
            return this.Context.Sql(article_sql)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.ArticleInListModel> GetArticleBySearch(int page, int pageCount)
        {
            var paramters = new object[] { (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(data_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ArticleInListModel>();
            return users;
        }
        public ArticleInListModel GetArticleById(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            var articles = Cache.Get<ArticleInListModel>(CacheKey.ARTICLE_QUERY + id);
            if (articles == null)
            {
                var paramters = new object[] { id };

                articles = this.Context.Sql(content_Sql)
                                       .Parameters(paramters)
                                       .QuerySingle<ArticleInListModel>();
                if(articles!=null)
                    Cache.Add(CacheKey.ARTICLE_QUERY + id, articles);
            }
            return articles;
        }
        /******************************前台****************************************************/
        public int CountCategoryArticleBySearch(ArticleCategory category)
        {
            var paramters = new object[] { (int)category };
            return this.Context.Sql(countArticleCategory_sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }
        public IEnumerable<DotPay.ViewModel.ArticleInListModel> GetCategoryArticleBySearch(Lang lang, ArticleCategory category, int page, int pageCount)
        {
            var paramters = new object[] { (int)category, (int)lang, (page - 1) * pageCount, pageCount };

            return this.Context.Sql(categoryArticle_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ArticleInListModel>();
        }
        public IEnumerable<DotPay.ViewModel.ArticleInListModel> GetArticleWhichIsTop(int top)
        {
            Check.Argument.IsNotNegativeOrZero(top, "top");
            var paramters = new object[] { top };

            var articles = this.Context.Sql(articleTop_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ArticleInListModel>();

            return articles;
        }

        public ArticleInListModel GetFristArticleByCategory(ArticleCategory category)
        {
            Check.Argument.IsNotNegativeOrZero((int)category, "category");

            var article= this.Context.Sql(getFirstArticleByCategory)
                                       .Parameter("@category", category)
                                       .QuerySingle<ArticleInListModel>();

            return article;
        }


        #region SQL
        private readonly string article_sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"article ";
        private readonly string content_Sql =
                                @"SELECT   ID, Title, Content, IsTop, Category, CreateAt, CreateBy, UpdateAt, UpdateBy, Lang
                                    FROM   " + Config.Table_Prefix + @"article
                                   WHERE   ID = @0 
                                   ORDER BY  CreateAt  DESC";

        private readonly string data_Sql =
                                @"SELECT  ID, Title, Content, IsTop, Category, CreateAt, CreateBy, UpdateAt, UpdateBy, Lang
                                    FROM   " + Config.Table_Prefix + @"article 
                                   ORDER BY  CreateAt  DESC
                                   LIMIT    @0,@1";

        private readonly string countArticleCategory_sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"article
                                   WHERE  (@0=0 OR Category = @0)";

        private readonly string categoryArticle_Sql =
                                @"SELECT   ID, Title, Content, IsTop, CreateAt, CreateBy, UpdateAt, UpdateBy 
                                    FROM   " + Config.Table_Prefix + @"article
                                   WHERE   (@0=0 OR Category = @0)
                                     AND   Lang =@1
                                ORDER BY   CreateAt  DESC
                                   LIMIT   @2,@3";

        private readonly string articleTop_Sql =
                                @"SELECT  ID, Title, Content, IsTop, CreateAt, CreateBy, UpdateAt, UpdateBy
                                    FROM   " + Config.Table_Prefix + @"article 
                                   ORDER BY  CreateAt  DESC
                                   LIMIT    0,@0";

        private readonly string getFirstArticleByCategory =
                               @"SELECT   ID,Title,Category,Content,CreateAt
                                    FROM   " + Config.Table_Prefix + @"article
                                   WHERE    (@category=0 OR Category = @category)
                                   ORDER BY  CreateAt  DESC
                                   LIMIT   1";

        #endregion
    }
}
