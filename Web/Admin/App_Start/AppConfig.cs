using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.QueryService.Impl;
using DotPay.Common;
using DotPay.Persistence;

namespace DotPay.Web.Admin
{
    public static class AppConfig
    {
        public static void Run()
        {
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var connString = ConfigurationManagerWrapper.GetDBConnectionString("FCDB2");
            var connProvider = ConfigurationManagerWrapper.GetProviderName("FCDB2");
            var mqConnectionString = ConfigurationManagerWrapper.GetMessageQueueServerConnectionString("messageQueueServerConnectString");

            var assemblies = GetAllAssembly();
            var nhibernateMapperAssemblies = assemblies.Where(ass => ass.FullName.IndexOf("DotPay.Persistence", StringComparison.OrdinalIgnoreCase) > -1);

            FCFramework.Initialize().UseAutofac()
                                    .UseCouchbaseCache()
                                    .UseLog4net()
                                    .UseDefaultCommandBus(assemblies)
                                    .RegisterQueryServices(new ConnectionString(connString, connProvider))
                                    .UseNHibernate(new ConnectionString(connString, connProvider), nhibernateMapperAssemblies)
                                    .RegisterAllRepository()
                                    .UseDefaultEventBus(assemblies)
                                    .Start();

            if (DotPay.Common.Config.Debug)
            {
                //SessionManager.CreateTables();
                //    for (int i = 0; i < 35; i++)
                //    {
                //        task.factory.startnew((t1) =>
                //        {
                //            var cmd = new userregister("email" + t1.tostring() + "@11.com", t1.tostring(), 8);
                //            ioc.resolve<icommandbus>().send(cmd);
                //        }, i);
                //    }
                //var randomNum = new Random();
                //for (int i = 0; i < 100000; i++)
                //{
                //    var cmd = new GenerateDepositCode(CurrencyType.CNY, randomNum.Next(100, 10000), 2);
                //    IoC.Resolve<ICommandBus>().Send(cmd);
                //}
            }
        }

        private static Assembly[] GetAllAssembly()
        {
            List<Assembly> assemlies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                     .Where(file => file.IndexOf(".dll", StringComparison.OrdinalIgnoreCase) > 0 
                                                  &&!file.Equals("DotPay.Web.Admin.DLL", StringComparison.OrdinalIgnoreCase)
                                                  && !file.Contains("System")
                                                  && !file.Contains("Microsoft"))
                     .ForEach(dll =>
            {
                assemlies.Add(Assembly.LoadFrom(dll));
            });

            return assemlies.ToArray();
        }
    }
}
