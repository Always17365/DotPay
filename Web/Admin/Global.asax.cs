using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using Dotpay.AdminQueryServiceImpl;
using Orleans;

namespace Dotpay.Admin
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var assemblies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                .Where(f => f.IndexOf("Dotpay.AdminCommandExcutor.dll", 0, StringComparison.OrdinalIgnoreCase) > -1)
                .ForEach(fp => assemblies.Add(Assembly.LoadFrom(fp)));


            DFramework.DEnvironment.Initialize()
                                   .UseAutofac()
                                   .UseLog4net()
                                   .UseDefaultCommandBus(assemblies.ToArray())
                                   .RegisterQueryService(DotpayConfig.DBConnectionString, DotpayConfig.DatabaseName)
                                   .Start();

            GrainClient.Initialize(Path.Combine(basePath, "ClientConfiguration.xml"));
        }

    }
}