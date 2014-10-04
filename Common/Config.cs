using FC.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    public class Config
    {
        static Config()
        {
            //if not config this, Debug is True
            if (false.ToString().Equals(ConfigurationManagerWrapper.AppSettings["Debug"], StringComparison.OrdinalIgnoreCase))
                Debug = false;
            else Debug = true;

            Table_Prefix = ConfigurationManagerWrapper.AppSettings["Table_Prefix"];
            MQConnectionString = ConfigurationManagerWrapper.GetConnectionString("messageQueueServerConnectString");
            CDN = ConfigurationManagerWrapper.AppSettings["CDN"];
            WeiboAppkey = ConfigurationManagerWrapper.AppSettings["WeiboAppkey"];
            WeiboAppSecret = ConfigurationManagerWrapper.AppSettings["WeiboAppSecret"];
            EmailSMTP = ConfigurationManagerWrapper.AppSettings["EmailSMTP"];
            SupportEmail = ConfigurationManagerWrapper.AppSettings["SupportEmail"];
            EmailAccount = ConfigurationManagerWrapper.AppSettings["EmailAccount"];
            EmailPassword = ConfigurationManagerWrapper.AppSettings["EmailPassword"]; 
        }
        public static bool Debug { get; private set; }
        public static string CDN { get; private set; }
        public static string Table_Prefix { get; private set; }

        public static string MQConnectionString { get; private set; } 
        public static string EmailSMTP { get; private set; }
        public static string SupportEmail { get; private set; }
        public static string EmailAccount { get; private set; }
        public static string EmailPassword { get; private set; }

        public static string WeiboAppkey { get; private set; }

        public static string WeiboAppSecret { get; private set; }
    }
}