using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Dotpay.Language
{
    public static class LangHelpers
    {
        public static readonly ResourceManager RecourseManager = new ResourceManager(typeof(Resource));

        public static IHtmlString MetaAcceptLanguage<T>(this HtmlHelper<T> html)
        {
            var acceptLanguage = HttpUtility.HtmlAttributeEncode(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(String.Format("<meta name='accept-language' content='{0}'>", acceptLanguage));
        }
        /// <summary>
        /// 本地化获取资源值方法，用于web页面
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Lang(this HtmlHelper htmlhelper, string key, params string[] strings)
        {
            try
            {
                if (strings.Count() > 0)
                    return string.Format(Lang(key), strings);
                else
                    return Lang(key);
            }
            catch
            {
                return "?";
            }
        }

        /// <summary>
        /// 本地化获取资源值方法，用于web页面
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static HtmlString LangAsHtmlString(this HtmlHelper htmlhelper, string key, params string[] strings)
        {
            try
            {
                if (strings.Count() > 0)
                    return MvcHtmlString.Create(string.Format(Lang(key), strings));
                else
                    return MvcHtmlString.Create(Lang(key));
            }
            catch
            {
                return MvcHtmlString.Create("?");
            }
        }

        public static string Lang(string key, params string[] strings)
        {
            try
            {
                if (strings.Any())
                    return string.Format(Lang(key), strings);
                else
                    return Lang(key);
            }
            catch
            {
                return "?";
            }
        }
        public static string Lang(string key)
        {
            IEnumerable<DictionaryEntry> resxs = null;
            resxs = GetResx(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            var value = (string)resxs.FirstOrDefault<DictionaryEntry>(x => x.Key.ToString() == key).Value;
            if (value == string.Empty) value = key;
            else if (value == null) value = "?";
            return value;
        }
        /// <summary>
        /// 当前web环境线程中，获取资源视图
        /// </summary>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        private static IEnumerable<DictionaryEntry> GetResx(string resxKey)
        {
            IEnumerable<DictionaryEntry> resxs = null;

            resxs = WebCache.Get(resxKey) as IEnumerable<DictionaryEntry>;

            if (resxs == null)
            {
                ResourceManager resource = new ResourceManager(typeof(Resource));
                var resourceSet = resource.GetResourceSet(
                    Thread.CurrentThread.CurrentUICulture,
                    true,
                    true);

                if (resourceSet != null)
                {
                    resxs = resourceSet.Cast<DictionaryEntry>();
                    WebCache.Set(resxKey, resxs, int.MaxValue, false);
                }
            }

            return resxs;
        }
    }

    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public override string DisplayName
        {
            get
            {
                string displayName = LangHelpers.Lang(ResourceKey);//MyResource.ResourceManager.GetString(ResourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", ResourceKey)
                    : displayName;
            }
        }

        private string ResourceKey { get; set; }
    }
}
