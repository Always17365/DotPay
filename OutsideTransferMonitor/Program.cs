using FC.Framework;
using FC.Framework.Autofac;
using FC.Framework.Log4net;
using FC.Framework.CouchbaseCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using FC.Framework.NHibernate;
using DotPay.Common;
using RippleRPC.Net;
using System.Reflection;
using FC.Framework.Repository;

namespace DotPay.OutsideTransferMonitor
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
                UserRegisterWatcher.Start(mqpool);
                //NXTReceiveTransactionListener.Start(mqpool);
                //NXTAccountGenerator.Start(mqpool);
                //NXTTransactionConfirmationValidator.Start(mqpool);
                //NXTSendTransactionListener.Start(mqpool);

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
                Console.WriteLine("监控器已启动，请勿重复启动监控！");
                Console.WriteLine("输入任意键结束...");
                Console.Read();
            }

            Console.Read();
        }

        #region 私有方法
        private static void InitializeEnvironment()
        {
            Console.WriteLine("正在初始化运行环境...");
            var assemblies = GetAllAssembly();
            var nhibernateMapperAssemblies = assemblies.Where(ass => ass.FullName.IndexOf("DotPay.RipplePersistence", StringComparison.OrdinalIgnoreCase) > -1);
            var connString = ConfigurationManagerWrapper.GetDBConnectionString("FCDB2");
            var connProvider = ConfigurationManagerWrapper.GetProviderName("FCDB2");

            FCFramework.Initialize().UseAutofac()
                                    .UseCouchbaseCache()
                                    .UseLog4net()
                                    .UseDefaultCommandBus(assemblies)
                                    .UseNHibernate(new ConnectionString(connString, connProvider), nhibernateMapperAssemblies)
                                    //.RegisterQueryServices(new FC.Framework.Repository.ConnectionString(Config.DBConnectString, "MySql.Data.MySqlClient"))
                                    .Start();

            var rippleClient = new RippleClient(new Uri(Config.WssServer), 30, true); 
            IoC.Register<IRippleClient>(rippleClient);

            if (Config.Debug)
            {
                //SessionManager.CreateTables();
            }
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

        #region 获取程序集
        private static Assembly[] GetAllAssembly()
        {
            List<Assembly> assemlies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                     .Where(file => file.IndexOf(".dll", StringComparison.OrdinalIgnoreCase) > 0 &&
                                    file.IndexOf("DotPay", StringComparison.OrdinalIgnoreCase) > 0 &&
                                    !file.Equals("DotPay.Web.DLL", StringComparison.OrdinalIgnoreCase))
                     .ForEach(dll =>
                     {
                         assemlies.Add(Assembly.LoadFrom(dll));
                     });

            return assemlies.ToArray();
        }
        #endregion
        #endregion
    }
}
