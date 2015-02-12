using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Authentication.SASL;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using RabbitMQ.Client;

namespace Dotpay.SiloHost.BoostrapTask
{
    internal class DFrameworkInitializeTask
    {
        internal static void Start()
        {
            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseLog4net()
                        .Start();

            var mqConnectionString = ConfigurationManagerWrapper.GetDBConnectionString("MQConnectionString");
            var factory = new ConnectionFactory() { Uri = mqConnectionString, AutomaticRecoveryEnabled = true };

            IoC.Register<IConnectionFactory>(factory);
        }
    }
}
