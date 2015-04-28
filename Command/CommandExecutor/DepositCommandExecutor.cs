using System;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.Command;
using Orleans;

namespace Dotpay.CommandExecutor
{
    public class DepositCommandExecutor : ICommandExecutor<CreateDepositCommand>,
                                         ICommandExecutor<ConfirmDepositCommand>
    {
        public async Task ExecuteAsync(CreateDepositCommand cmd)
        {
            var depositManager = GrainFactory.GetGrain<IDepositTransactionManager>(0);
            var depositTxId = Guid.NewGuid();
            cmd.CommandResult = await depositManager.CreateDepositTransaction(
                                     depositTxId, cmd.AccountId, cmd.Currency,
                                     cmd.Amount, cmd.Payway, string.Empty);
        }

        public async Task ExecuteAsync(ConfirmDepositCommand cmd)
        {
            var depositManager = GrainFactory.GetGrain<IDepositTransactionManager>(0);
            cmd.CommandResult =
                await depositManager.ConfirmDepositTransaction(cmd.DepositTxId, null, cmd.TransferNo, cmd.Amount);
        }
    }
}
