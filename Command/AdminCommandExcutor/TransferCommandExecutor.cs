using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.AdminCommand;
using Dotpay.Common;
using Orleans;

namespace Dotpay.AdminCommandExcutor
{
    public class TransferCommandExecutor : ICommandExecutor<LockTransferTransactionCommand>,
                                           ICommandExecutor<ConfirmTransferTransactionSuccessCommand>,
                                           ICommandExecutor<ConfirmTransferTransactionFailCommand>
    {
        public async Task ExecuteAsync(LockTransferTransactionCommand cmd)
        {
            var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);

            cmd.CommandResult = await transferTransactionManager.MarkAsProcessing(
                cmd.TransferTransactionId, cmd.LockBy);
        }

        public async Task ExecuteAsync(ConfirmTransferTransactionSuccessCommand cmd)
        {
            var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);

            cmd.CommandResult = await transferTransactionManager.ConfirmTransactionComplete(
               cmd.TransferTransactionId, cmd.FiTransferNo, cmd.Amount, cmd.ConfrimBy);
        }

        public async Task ExecuteAsync(ConfirmTransferTransactionFailCommand cmd)
        {
            var transferTransactionManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);

            cmd.CommandResult = await transferTransactionManager.ConfirmTransactionFail(
               cmd.TransferTransactionId, cmd.ConfrimBy, cmd.Reason);
        }
    }
}
