using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using Dotpay.Actor.Ripple;
using Orleans;
using RabbitMQ.Client;
using Dotpay.Application.Monitor;


namespace Dotpay.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            InitEnvronment();
            StartMonitor();
            var random = new Random();
            var exitCode = "999";

            while (true)
            {
                var input = Console.ReadLine();
                if (input == exitCode)
                    break;
            }
        }

        private static void InitEnvronment()
        {
            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseLog4net()
                        .UseDefaultJsonSerialaizer();
            var mqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectionString");
            var factory = new ConnectionFactory() { Uri = mqConnectionString, AutomaticRecoveryEnabled = true };

            IoC.Register<IConnectionFactory>(factory);

            GrainClient.Initialize("ClientConfiguration.xml");
        }

        private static void StartMonitor()
        {
            var depositRecheckerMonitor = new DepositRecheckerMonitor();
            var emailMessagMonitor = new EmailMessageMonitor();
            var refundTransactionMonitor = new RefundTransactionMonitor();
            var rippleTxMessageMonitor = new RippleToFIMonitor();
            var transferTxMonitor = new TransferTransactionMonitor();
            var userActiveMonitor = new UserActiveMessageMonitor();
            var rippleResultMonitor = new RippleResultMessageMonitor();

            depositRecheckerMonitor.Start();
            emailMessagMonitor.Start();
            refundTransactionMonitor.Start();
            rippleTxMessageMonitor.Start();
            transferTxMonitor.Start();
            userActiveMonitor.Start();
            rippleResultMonitor.Start();
        }
    }
}
