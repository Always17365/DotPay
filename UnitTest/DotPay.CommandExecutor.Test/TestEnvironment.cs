using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.CouchbaseCache;
using FC.Framework.Log4net;
using FC.Framework.NHibernate;
using FC.Framework.Repository;
using DotPay.Command;
using DotPay.Common;
using DotPay.Persistence;
using DotPay.QueryService;
using DotPay.QueryService.Impl;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace DotPay.CommandExecutor.Test
{
    public class TestEnvironment
    {
        static bool inited = false;

        public static void Init()
        {
            if (!inited)
            {
                var connString = ConfigurationManagerWrapper.GetDBConnectionString("FCTestDB");
                var connProvider = ConfigurationManagerWrapper.GetProviderName("FCTestDB");
                var mqConnectionString = ConfigurationManagerWrapper.GetMessageQueueServerConnectionString("messageQueueServerConnectString");

                var assemblies = GetAllAssembly();
                var nhibernateMapperAssemblies = assemblies.Where(ass => ass.FullName.IndexOf("DotPay.Persistence", StringComparison.OrdinalIgnoreCase) > -1);

                FCFramework.Initialize().UseAutofac()
                                        .UseCouchbaseCache()
                                        .UseLog4net()
                                        .UseDefaultCommandBus(assemblies)
                                        .UseNHibernate(new ConnectionString(connString, connProvider), nhibernateMapperAssemblies)
                                        .RegisterAllRepository()
                                        .RegisterQueryServices(new ConnectionString(connString, connProvider))
                                        .UseDefaultEventBus(assemblies)
                                        .Start();
                inited = true;
                SessionManager.CreateTables();

                int i = 0;
                while (i < 10)
                {
                    i++;
                    var cmd = new UserRegister(Guid.NewGuid().Shrink(), "email" + i.ToString() + "@11.com", "1", "1", 8,"adsjfljasdljflasdjf");

                    IoC.Resolve<ICommandBus>().Send(cmd);
                }
            }
        }

        private static Assembly[] GetAllAssembly()
        {
            List<Assembly> assemlies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                     .Where(file => file.IndexOf(".dll", StringComparison.OrdinalIgnoreCase) > 0 &&
                                                  !file.Equals("DotPay.Web.DLL", StringComparison.OrdinalIgnoreCase))
                     .ForEach(dll =>
                     {
                         assemlies.Add(Assembly.LoadFrom(dll));
                     });

            return assemlies.ToArray();
        }
    }
}
