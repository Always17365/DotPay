using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Ripple;
using Dotpay.SiloHost.BootstrapTask;
using log4net.Repository;
using Orleans;
using Orleans.Providers;

namespace Dotpay.SiloHost
{
    public class InitializeTaskBootstrap : IBootstrapProvider
    {
        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            DFrameworkInitializeTask.Run();
            EventStoreInitializeTask.Run();
            await BootstrapGrainInitializeTask.Run(); 
        }

        public string Name
        {
            get { return "InitializeTaskBootstrap"; }
        }


    }
}
