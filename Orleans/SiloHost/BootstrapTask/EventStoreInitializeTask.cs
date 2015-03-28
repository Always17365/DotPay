using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orleans.EventSourcing;

namespace Dotpay.SiloHost.BootstrapTask
{
    internal static class EventStoreInitializeTask
    {
        internal static void Run()
        {
            var eventStoreSection = (EventStoreSection)ConfigurationManager.GetSection("eventStoreProvider");
            EventStoreProviderManager.Initailize(eventStoreSection);

            var assembly = Assembly.LoadFrom(".\\Applications\\Dotpay.Actor.Implementations\\Dotpay.Actor.Implementations.dll");

            GrainInternalEventHandlerProvider.RegisterInternalEventHandler(assembly);
            EventNameTypeMapping.RegisterEventType(assembly); 
        }
    }
}
