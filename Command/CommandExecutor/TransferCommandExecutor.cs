using System;
using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.Command;
using Orleans;

namespace Dotpay.CommandExecutor
{
    public class TransferCommandExecutor : ICommandExecutor<SubmitTransferToDotpayTransactionCommand>,
                                           ICommandExecutor<SubmitTransferToTppTransactionCommand>,
                                           ICommandExecutor<SubmitTransferToBankTransactionCommand>,
                                           ICommandExecutor<SubmitTransferToRippleTransactionCommand>
    {

        public async Task ExecuteAsync(SubmitTransferToDotpayTransactionCommand cmd)
        {
            var transferManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
            cmd.CommandResult =
                await transferManager.SubmitTransferToDotpayTransaction(cmd.TransferTransactionId, cmd.SourceAccountId,
                    cmd.TargetAccountId, cmd.RealName, cmd.Currency, cmd.Amount, cmd.Memo, cmd.PaymentPassword);
        }

        public async Task ExecuteAsync(SubmitTransferToTppTransactionCommand cmd)
        {
            var transferManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
            cmd.CommandResult =
                await transferManager.SubmitTransferToTppTransaction(cmd.TransferTransactionId, cmd.SourceAccountId,
                    cmd.TargetAccount, cmd.RealName, cmd.Payway, cmd.Currency, cmd.Amount, cmd.Memo, cmd.PaymentPassword);
        }

        public async Task ExecuteAsync(SubmitTransferToBankTransactionCommand cmd)
        {
            var transferManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
            cmd.CommandResult =
                await transferManager.SubmitTransferToBankTransaction(cmd.TransferTransactionId, cmd.SourceAccountId,
                    cmd.TargetAccount, cmd.TargetRealName, cmd.Bank, cmd.Currency, cmd.Amount, cmd.Memo, cmd.PaymentPassword);
        }

        public async Task ExecuteAsync(SubmitTransferToRippleTransactionCommand cmd)
        {
            var transferManager = GrainFactory.GetGrain<ITransferTransactionManager>(0);
            cmd.CommandResult =
                await transferManager.SubmitTransferToRippleTransaction(cmd.TransferTransactionId, cmd.SourceAccountId,
                    cmd.RippleAddress, cmd.Currency, cmd.Amount, cmd.Memo, cmd.PaymentPassword);
        }
    }
}
