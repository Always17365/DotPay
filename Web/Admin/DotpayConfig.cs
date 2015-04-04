using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DFramework;

namespace Dotpay.Admin
{
    public class DotpayConfig
    {
        static DotpayConfig()
        {
            //if not config this, Debug is True
            if (false.ToString().Equals(ConfigurationManagerWrapper.AppSettings["Debug"], StringComparison.OrdinalIgnoreCase))
                Debug = false;
            else Debug = true;

            DBConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("dbconn");
            DatabaseName = ConfigurationManagerWrapper.AppSettings["DatabaseName"];
            WeiboAppkey = ConfigurationManagerWrapper.AppSettings["WeiboAppkey"];
            WeiboAppSecret = ConfigurationManagerWrapper.AppSettings["WeiboAppSecret"];
        }
        public static bool Debug { get; private set; } 
        public static string DBConnectionString { get; private set; }
        public static string DatabaseName { get; private set; }
        public static string WeiboAppkey { get; private set; }
        public static string WeiboAppSecret { get; private set; } 
    }
}