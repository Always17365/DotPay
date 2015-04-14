using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using Dotpay.AdminQueryServiceImpl;
using DFramework.Memcached;
using Orleans;

namespace Dotpay.Admin
{
    public class Global : HttpApplication
    {
        void Application_PreSendRequestHeaders()
        {
            this.Response.Headers.Remove("Server");
        }

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var assemblies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                .Where(f => f.IndexOf("Dotpay.AdminCommandExcutor.dll", 0, StringComparison.OrdinalIgnoreCase) > -1)
                .ForEach(fp => assemblies.Add(Assembly.LoadFrom(fp)));


            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseLog4net()
                        .UseMemcached("192.168.2.10")
                        .UseDefaultCommandBus(assemblies.ToArray())
                        .RegisterQueryService(DotpayConfig.DBConnectionString, DotpayConfig.DatabaseName)
                        .Start();

            GrainClient.Initialize(Path.Combine(basePath, "ClientConfiguration.xml"));
        }

        void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                var sessionState = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;

                var cookieName = sessionState != null && !string.IsNullOrEmpty(sessionState.CookieName)
                  ? sessionState.CookieName
                  : "ASP.NET_SessionId";

                var timeout = sessionState != null ? sessionState.Timeout : TimeSpan.FromMinutes(20);

                if (this.Request.Cookies[cookieName] != null && Session.SessionID != null)
                {
                    Response.Cookies[cookieName].Value = Session.SessionID;
                    Response.Cookies[cookieName].Path = Request.ApplicationPath;
                    if (!DotpayConfig.Debug)
                        Response.Cookies[cookieName].Domain = DotpayConfig.Domain;
                }
            }
        }

    }
}