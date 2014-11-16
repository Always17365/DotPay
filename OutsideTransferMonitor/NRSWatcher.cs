using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using System.Threading;
using DotPay.Tools.NXTClient;
using System.Net;
using DotPay.Common;

namespace DotPay.Tools.NXTMonitor
{
    internal class NRSWatcher
    {
        public static bool Started { get; private set; }

        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private static int retryCount = 0;
        public static CurrencyType _currency;
        private const string _windowTitle = "NXT NRS";

        internal static void Start()
        {
            if (NRSWatcher.Started)
            {
                Log.Info("NRS运行监视器已启动");
                return;
            }

            if (!Enum.TryParse<CurrencyType>(Config.CoinCode, out _currency))
            {
                Log.Error("配置文件中的CoinCode配置错误:错误值为{0},请使用正确的虚拟币Code", Config.CoinCode);
            }

            var thread = new Thread(new ThreadStart(() =>
            {
                Log.Info("NRS运行监视器启动中...");

                while (true)
                {
                    if (Program.Fusing)
                    {
                        Log.Info("发生熔断事件，虚拟币客户端监控器停止运行");
                        _cancelTokenSource.Cancel();
                        StopCoinServer();
                        break;
                    }

                    var existThread = Process.GetProcesses()
                                             .SingleOrDefault(p => p.ProcessName.Equals("java", StringComparison.InvariantCultureIgnoreCase) &&
                                                                   p.MainWindowTitle == _windowTitle);

                    if (!NRSWatcher.Started)
                    {
                        Log.Info("NRS运行监视器启动成功,监视中...");
                        if (existThread != null)
                        {
                            Log.Info("NRS已启动");
                        }
                        NRSWatcher.Started = true;
                    }

                    //if not exist the nxt thread
                    if (existThread == null)
                    {
                        Log.Info("发现NRS未运行,监视器正在启动NRS...");
                        StartCoinServer(Config.NXTFilePath);
                    }

                    Thread.Sleep(Config.LoopInterval * 1000);
                }
            }));

            thread.Start();
        }

        #region 私有方法
        private static void StartCoinServer(string nxtFileName)
        {
            var nrs = new ProcessStartInfo(nxtFileName);
            nrs.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            nrs.WorkingDirectory = Path.GetDirectoryName(nxtFileName);
            try
            {
                System.Diagnostics.Process.Start(nrs);
                System.Threading.Thread.Sleep(1000);
                retryCount = 0;
                Log.Info("NRS启动成功！");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                retryCount += 1;
                if (retryCount <= Config.NXTServerStartRetryMaxCount)
                {
                    //每重试一次，增量睡眠30秒
                    var addSleepTime = retryCount * 30;
                    Log.Warn("启动NRX过程中出现错误，在{1}秒之后，会再次重新启动", Config.LoopInterval + addSleepTime);

                    Thread.Sleep(addSleepTime);
                }
                else
                {
                    var msg = "启动NRS过程中出现错误，到达最大重启次数{1}".FormatWith(Config.NXTServerStartRetryMaxCount);
                    Log.Fatal(msg, ex);
                }
            }
        }


        private static void StopCoinServer()
        {
            try
            {
                Log.Info("关闭NRS中...");
                var existThread = Process.GetProcesses()
                                          .SingleOrDefault(p => p.ProcessName.Equals("java", StringComparison.InvariantCultureIgnoreCase) &&
                                                                p.MainWindowTitle == _windowTitle);


                //if not exist the nxt thread
                if (existThread == null)
                {
                    Log.Info("NRS已关闭");
                }
                else if (!existThread.HasExited)
                {
                    existThread.Kill();
                }
                Log.Info("成功关闭NRS!");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Log.Error("关闭{0} server失败".FormatWith(_currency), ex);
            }
        }
        #endregion
    }
}
