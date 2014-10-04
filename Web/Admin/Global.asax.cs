using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DotPay.Web.Admin
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {
            AppConfig.Run();
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PreSendRequestHeaders()
        {
            HttpContext.Current.Response.Headers.Remove("Server");
        }

        protected void Application_BeginRequest()
        {
            string lang = "zh-CN";

            if (null != Request.Cookies["lanaguage"])
            {
                lang = Request.Cookies["lanaguage"].Value;
            }
            else
            {
                if (null != Request.UserLanguages)
                {
                    Response.Cookies.Add(new HttpCookie("lanaguage", Request.UserLanguages[0]));
                    lang = Request.UserLanguages[0];
                }
            }
            if (!lang.Equals(Thread.CurrentThread.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
                SetApplicationLanaguage(lang);
        }

        #region
        private void SetApplicationLanaguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }
        #endregion
    }
}