using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Log4net;
using FC.Framework.CouchbaseCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DotPay.Tools.NXTClient;
using System.IO;
using DotPay.QueryService;
using DotPay.QueryService.Impl;
using DotPay.Common;

namespace DotPay.Tools.NXTMonitor
{
    class Program
    {
        public static bool Fusing { get; set; }
        static void Main(string[] args)
        {
            if (!IsRuning())
            {
                Fusing = false; //熔断
                InitializeEnvironment();

                var mqpool = new MQConnectionPool(Config.MQConnectString);
                NXTBalanceWatcher.Start(mqpool);
                NXTReceiveTransactionListener.Start(mqpool);
                NXTAccountGenerator.Start(mqpool);
                NXTTransactionConfirmationValidator.Start(mqpool);
                NXTSendTransactionListener.Start(mqpool);

                while (true)
                {
                    if (Fusing)
                        break;
                    else
                    {
                        Console.Read();
                        Thread.Sleep(60 * 1000);
                    }
                }

                Stop();
            }
            else
            {
                Console.WriteLine(Config.CoinCode + "监控器已启动，请勿重复启动监控！");
                Console.WriteLine("输入任意键结束...");
                Console.Read();
            }

            Console.Read();
        }

        #region 私有方法
        private static void InitializeEnvironment()
        {
            Console.WriteLine("正在初始化运行环境...");
            FCFramework.Initialize().UseAutofac()
                                    .UseCouchbaseCache()
                                    .UseLog4net()
                                    .RegisterQueryServices(new FC.Framework.Repository.ConnectionString(Config.DBConnectString, "MySql.Data.MySqlClient"))
                                    .Start();

            Log.Info("运行环境初始化完毕!");
        }

        private static bool IsRuning()
        {
            if (Config.Debug) return false;
            bool _isRuning = false;
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var lockFile = Path.Combine(path, "m.lock");

            _isRuning = File.Exists(lockFile);

            if (!_isRuning)
            {
                using (var sw = File.CreateText(lockFile))
                {
                    sw.Write(DateTime.Now.ToString() + " lock");
                    sw.Flush();
                }
            }
            return _isRuning;
        }

        private static void Stop()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var lockFile = Path.Combine(path, "m.lock");
            if (File.Exists(lockFile))
                File.Delete(lockFile);
        }
        #endregion
    }
}
