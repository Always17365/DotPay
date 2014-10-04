using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class VirtualCoinWithdrawCommandExecutors : ICommandExecutor<SubmitVirtualCoinWithdraw>,
                                                       ICommandExecutor<VirtualCoinWithdrawVerify>,
                                                       ICommandExecutor<CompleteVirtualCoinWithdraw>,
                                                       ICommandExecutor<VirtualCoinWithdrawFail>,
                                                       ICommandExecutor<CancelVirtualCoinWithdraw>
    {
        public void Execute(SubmitVirtualCoinWithdraw cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var account = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cmd.WithdrawUserID, cmd.Currency);
            var receiver = repos.FindById<User>(cmd.WithdrawUserID);

            if (receiver.VerifyTradePassword(PasswordHelper.EncryptMD5(cmd.TradePassword)))
            {
                if (account == null) throw new AccountBalanceNotEnoughException();

                var withdraw = VirtualCoinWithdrawFactory.CreateWithdraw(cmd.Currency, cmd.WithdrawUserID, account.ID, cmd.Amount, cmd.ReceiveAddress);

                repos.Add(withdraw);
            }
            else throw new TradePasswordErrorException();
        }

        public void Execute(VirtualCoinWithdrawVerify cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var withdraw = IoC.Resolve<IWithdrawRepository>().FindByIdAndCurrency(cmd.WithdrawID, cmd.Currency);

            withdraw.Verify(cmd.ByUserID, cmd.Memo);
        }

        public void Execute(CompleteVirtualCoinWithdraw cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var withdraw = IoC.Resolve<IWithdrawRepository>().FindByUniqueIdAndCurrency(cmd.WithdrawUniqueID, cmd.Currency) as VirtualCoinWithdraw;

            withdraw.Complete(cmd.TxID, cmd.TxFee);
        }

        public void Execute(VirtualCoinWithdrawFail cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IWithdrawRepository>();
            var withdraw = repos.FindByUniqueIdAndCurrency(cmd.WithdrawUniqueID, cmd.Currency) as VirtualCoinWithdraw;

            withdraw.MarkTransferFail(cmd.ByUserID);
        }

        public void Execute(CancelVirtualCoinWithdraw cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IWithdrawRepository>();
            var withdraw = repos.FindByUniqueIdAndCurrency(cmd.WithdrawUniqueID, cmd.Currency) as VirtualCoinWithdraw;

            withdraw.Cancel(cmd.ByUserID, cmd.Memo);
        }
    }
}
