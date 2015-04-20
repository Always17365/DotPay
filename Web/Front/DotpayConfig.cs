using System;
using DFramework;

namespace Dotpay.Front
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
            Domain = ConfigurationManagerWrapper.AppSettings["Domain"]; 
        }
        public static bool Debug { get; private set; } 
        public static string DBConnectionString { get; private set; }
        public static string DatabaseName { get; private set; }
        public static string Domain { get; private set; } 
    }
}