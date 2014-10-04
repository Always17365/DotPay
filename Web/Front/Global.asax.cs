using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace DotPay.Web
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
            lang = "zh-CN";
            if (!lang.Equals(Thread.CurrentThread.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
                SetApplicationLanaguage(lang);
        }

        protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            /// only apply session cookie persistence to requests requiring session information
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                var sessionState = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
                var cookieName = sessionState != null && !string.IsNullOrEmpty(sessionState.CookieName)
                  ? sessionState.CookieName
                  : "ASP.NET_SessionId";

                var timeout = sessionState != null
                  ? sessionState.Timeout
                  : TimeSpan.FromMinutes(20);

                /// Ensure ASP.NET Session Cookies are accessible throughout the subdomains.
                if (Request.Cookies[cookieName] != null && Session != null && Session.SessionID != null)
                {
                    Response.Cookies[cookieName].Value = Session.SessionID;
                    Response.Cookies[cookieName].Path = Request.ApplicationPath;
                    if (!Config.Debug)
                        Response.Cookies[cookieName].Domain = "fullcoin.com";
                }
            }
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