//#define TAOBAODEBUG
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Client.Providers;
using DFramework;
using DFramework.Autofac;
using DFramework.CouchbaseCache;
using DFramework.Log4net;
using DFramework.Utilities;



namespace Dotpay.TaobaoMonitor
{
    class Program
    {
        /// <summary>
        /// 此监控程序的启动应在node ripple处理器启动之后再启动
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            InitEnvironment();
            TaobaoOrderReadRecordUpdateMonitor.Start();
            DepositDispatcher.Start();
            RippleMessageListener.Start();
            TaobaoAutoSendGoodsMonitor.Start();
            TrustLineMessageListener.Start();
#if TAOBAODEBUG
            TaobaoUtils.StartGenerateTaobaoTrade();
#endif
            Log.Info("-->等待2分钟后，将启动丢失tx验证器...等待中...");
            Task.Delay(60 * 2 * 1000).Wait();
            Log.Info("-->等待2分钟结束，开始启动丢失tx验证器...");
            LoseTransactionRevalidator.Start();

            Console.Read();
        }


        private static void InitEnvironment()
        {
            var configSection =
              (CouchbaseClientSection)ConfigurationManager.GetSection("couchbaseClients/couchbaseCache");

            var clientConfig = new ClientConfiguration(configSection);

            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseCouchbaseCache(clientConfig, "dotpay")
                        .UseLog4net()
                        .UseDefaultJsonSerialaizer();

            EmailHelper.Config("smtp.exmail.qq.com", "webmaster@dotpay.co", "webmaster@dotpay.co", "lwt#dotpay1");
        }
    }
}
