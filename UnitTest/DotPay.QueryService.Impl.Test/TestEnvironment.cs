using System;
using Xunit;
using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Log4net;
using FC.Framework.CouchbaseCache;
using FC.Framework.Repository;
using DotPay.Common;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace DotPay.QueryService.Impl.Test
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
                var conn = new ConnectionString(connString, connProvider);
                FCFramework.Initialize().UseAutofac().UseLog4net().UseCouchbaseCache().RegisterQueryServices(conn);

                inited = true;
            }
        }
    }
}
