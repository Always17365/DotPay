using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.QueryService;
using DotPay.QueryService.Impl;
using DotPay.Common;
using DotPay.Persistence;
using System.Threading;
using DotPay.Tools.SmsInterface;

namespace DotPay.Web
{
    public static class AppConfig
    {
        public static void Run()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var connString = ConfigurationManagerWrapper.GetDBConnectionString("FCDB2");
            var connProvider = ConfigurationManagerWrapper.GetProviderName("FCDB2");
            var mqConnectionString = ConfigurationManagerWrapper.GetMessageQueueServerConnectionString("messageQueueServerConnectString");

            var assemblies = GetAllAssembly();
            var nhibernateMapperAssemblies = assemblies.Where(ass => ass.FullName.IndexOf("Persistence", StringComparison.OrdinalIgnoreCase) > -1);

            FCFramework.Initialize().UseAutofac()
                                    .UseCouchbaseCache()
                                    .UseLog4net()
                                    .UseDefaultCommandBus(assemblies)
                                    .RegisterQueryServices(new ConnectionString(connString, connProvider))
                                    .UseNHibernate(new ConnectionString(connString, connProvider), nhibernateMapperAssemblies)
                                    .RegisterAllRepository()
                                    .UseDefaultEventBus(assemblies)
                                    .Start();

            FC.Framework.Utilities.EmailHelper.Config(Config.EmailSMTP, Config.SupportEmail, Config.EmailAccount, Config.EmailPassword);
            SmsHelper.SetSmsInterface(SmsInterfaceType.IHUYI, "cf_bfwl", "HvebKj");
            SmsHelper.SetBackupSmsInterface(SmsInterfaceType.C123, "500919170001", "fc_sms_account!1");
            if (Config.Debug)
            {
                //SessionManager.CreateTables(); 
            }
        }

        #region 获取程序集
        private static Assembly[] GetAllAssembly()
        {
            List<Assembly> assemlies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                     .Where(file => file.IndexOf(".dll", StringComparison.OrdinalIgnoreCase) > 0 &&
                                    file.IndexOf("DotPay", StringComparison.OrdinalIgnoreCase) > 0 &&
                                    !file.Equals("DotPay.Web.DLL", StringComparison.OrdinalIgnoreCase))
                     .ForEach(dll =>
            {
                assemlies.Add(Assembly.LoadFrom(dll));
            });

            return assemlies.ToArray();
        }
        #endregion
    }
}
