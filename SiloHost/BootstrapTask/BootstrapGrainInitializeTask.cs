using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Interfaces.Ripple;
using Orleans;

namespace Dotpay.SiloHost.BootstrapTask
{
    public class BootstrapGrainInitializeTask
    {
        internal static async Task Run()
        {
            //ripple直通车消息监听器
            var rippleListener = GrainFactory.GetGrain<IRippleToFinancialInstitutionListener>(0);
            await rippleListener.Start();
            //email监听器--终止，如果silo在内网部署的话，无法访问外网，如果通过代理访问外网，增加了复杂性

        }
    }
}
