using FC.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Tools.NXTMonitor
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
            DefaultConfirmations = Convert.ToInt32(ConfigurationManagerWrapper.AppSettings["DefaultConfirmations"].NullSafe().Trim());
            CoinCode = ConfigurationManagerWrapper.AppSettings["CoinCode"].NullSafe().Trim();
            StatisticsFlg = ConfigurationManagerWrapper.AppSettings["StatisticsFlg"].NullSafe().Trim();
            MutexLocker = ConfigurationManagerWrapper.AppSettings["MutexLocker"].NullSafe().Trim();
            NXTAddressTableName = ConfigurationManagerWrapper.AppSettings["NXTAddressTableName"].NullSafe().Trim();
            TxTableName = ConfigurationManagerWrapper.AppSettings["TxTableName"].NullSafe().Trim();
            CurrencyTableName = ConfigurationManagerWrapper.AppSettings["CurrencyTableName"].NullSafe().Trim();
            JavaPath = ConfigurationManagerWrapper.AppSettings["JavaPath"].NullSafe().Trim();
            NXTFilePath = ConfigurationManagerWrapper.AppSettings["NXTFilePath"].NullSafe().Trim();
            NXTServer = ConfigurationManagerWrapper.AppSettings["NXTServer"].NullSafe().Trim();
            NxtSumAccount = ConfigurationManagerWrapper.AppSettings["NxtSumAccount"].NullSafe().Trim();
            SecretPhrase = ConfigurationManagerWrapper.AppSettings["SecretPhrase"].NullSafe().Trim();
            NXTServerStartRetryMaxCount = Convert.ToInt32(ConfigurationManagerWrapper.AppSettings["NXTServerStartRetryMaxCount"].NullSafe().Trim());
        }
        public static bool Debug { get; private set; }

        public static string MutexLocker { get; private set; }

        public static string Table_Prefix { get; private set; }

        public static string MQConnectString { get; private set; }

        public static string DBConnectString { get; private set; }

        public static int LoopInterval { get; private set; }

        public static int DefaultConfirmations { get; private set; }

        public static string TxTableName { get; private set; }

        public static string NXTAddressTableName { get; private set; }

        public static string CurrencyTableName { get; private set; }

        public static string CoinCode { get; private set; }
        public static string StatisticsFlg { get; private set; }

        public static string JavaPath { get; private set; }

        public static string NXTFilePath { get; private set; }

        public static string NXTServer { get; private set; }

        public static string SecretPhrase { get; private set; }

        public static int NXTServerStartRetryMaxCount { get; private set; }
        public static string NxtSumAccount { get; private set; }
    }
}
