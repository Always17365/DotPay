using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using Orleans;
using RabbitMQ.Client;

namespace Dotpay.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            InitEnvronment();
        }

        private static void InitEnvronment()
        {
            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseLog4net()
                        .Start();
            var mqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("messageQueueServerConnectionString");
            var factory = new ConnectionFactory() { Uri = mqConnectionString, AutomaticRecoveryEnabled = true };

            GrainClient.Initialize("ClientConfiguration.xml");
        }
    }
}
