using System.Threading.Tasks;
using Dotpay.Actor.Tools;
using Orleans;

namespace Dotpay.SiloHost.BootstrapTask
{
    public class BootstrapGrainInitializeTask
    {
        internal static Task Run()
        {
            ////ripple直通车消息监听器
            //var rippleListener = GrainFactory.GetGrain<IRippleToFIListener>(0);
            //await rippleListener.Start();
            ////email监听器--终止，如果silo在内网部署的话，无法访问外网，如果通过代理访问外网，增加了复杂性

            ////----------------------------------
            ////充值监听器-监听来自异常关闭后未处理的消息
            ////----------------------------------- 
            //var depositTransactionManager = GrainFactory.GetGrain<IDepositTransactionManager>(0);
            //await depositTransactionManager.Start();
            ////----------------------------------
            ////转账监听器-监听来自Ripple处理的消息
            ////----------------------------------- 
            //var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
            //await transferTransactionManager.Start();
            ////-----------------------------------
            ////退款监听器-监听来自转账失败等的处理消息，及时进行退款处理
            ////----------------------------------- 
            //var refundTransactionManager = GrainFactory.GetGrain<IRefundTransactionManager>(0);
            //await refundTransactionManager.Start(); 
            var rippleRpcClient = GrainFactory.GetGrain<IRippleRpcClient>(0);
            rippleRpcClient.GetLastLedgerIndex();
            return TaskDone.Done;
        }
    }
}
