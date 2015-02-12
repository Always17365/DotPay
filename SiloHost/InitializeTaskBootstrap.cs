using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces.Ripple;
using Dotpay.SiloHost.BoostrapTask;
using log4net.Repository;
using Orleans;
using Orleans.Providers;

namespace Dotpay.SiloHost
{
    public class InitializeTaskBootstrap : IBootstrapProvider
    {
        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            DFrameworkInitializeTask.Start();
            EventStoreInitializeTask.Start();
            //一些长期处理的监控器的启动，比如rippleLister
            var rippleListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionListener>(0);
            await rippleListener.Start();
           
        }

        public string Name
        {
            get { return "InitializeTaskBootstrap"; }
        }


    }
}
