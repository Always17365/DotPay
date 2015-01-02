using FC.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.TransferMonitor
{
    public class Config
    {
        static Config()
        {
            //if not config this, Debug is True
            if (false.ToString().Equals(ConfigurationManagerWrapper.AppSettings["Debug"], StringComparison.OrdinalIgnoreCase))
                Debug = false;
            else Debug = true;

            Table_Prefix = ConfigurationManagerWrapper.AppSettings["Table_Prefix"].NullSafe().Trim();
            DBConnectString = ConfigurationManagerWrapper.GetDBConnectionString("FCDB2").NullSafe().Trim();
            MQConnectString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectString").NullSafe().Trim();
            LoopInterval = Convert.ToInt32(ConfigurationManagerWrapper.AppSettings["LoopInterval"].NullSafe().Trim());
            WithdrawTable = ConfigurationManagerWrapper.AppSettings["WithdrawTable"].NullSafe().Trim();
            DepositTable = ConfigurationManagerWrapper.AppSettings["DepositTable"].NullSafe().Trim();
            RippleAccount = ConfigurationManagerWrapper.AppSettings["RippleAccount"].NullSafe().Trim();
            RippleSecret = ConfigurationManagerWrapper.AppSettings["RippleSecret"].NullSafe().Trim();
            TransferTable = ConfigurationManagerWrapper.AppSettings["TransferTable"].NullSafe().Trim();
            EmailSMTP = ConfigurationManagerWrapper.AppSettings["EmailSMTP"].NullSafe().Trim(); 
            EmailAccount = ConfigurationManagerWrapper.AppSettings["EmailAccount"].NullSafe().Trim(); 
            EmailPassword = ConfigurationManagerWrapper.AppSettings["EmailPassword"].NullSafe().Trim(); 
            WssServer = ConfigurationManagerWrapper.AppSettings["WssServer"].NullSafe().Trim();  
        }

        public static bool Debug { get; private set; }

        public static string Table_Prefix { get; private set; }

        public static string MQConnectString { get; private set; }

        public static string DBConnectString { get; private set; }

        public static int LoopInterval { get; private set; }
        public static string WithdrawTable { get; private set; }
        public static string DepositTable { get; private set; }
        public static string TransferTable { get; private set; }
        public static string RippleSecret { get; private set; }
        public static string WssServer { get; private set; } 
        public static string RippleAccount { get; private set; }
        public static string EmailSMTP { get; private set; }
        public static string EmailAccount { get; private set; }
        public static string EmailPassword { get; private set; }
    }
}
