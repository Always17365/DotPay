using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class CNYWithdrawCommandExecutors : ICommandExecutor<SubmitCNYWithdraw>,
                                               ICommandExecutor<CNYWithdrawVerify>,
                                               ICommandExecutor<SubmitCNYWithdrawToProcess>,
                                               ICommandExecutor<CNYWithdrawModifyReceiverBankAccount>,
                                               ICommandExecutor<CNYWithdrawMarkAsSuccess>,
                                               ICommandExecutor<CNYWithdrawMarkAsTransferFail>,
                                               ICommandExecutor<CNYWithdrawCancel>
    {

        public void Execute(SubmitCNYWithdraw cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var account = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cmd.WithdrawUserID, CurrencyType.CNY);
            var receiver = repos.FindById<User>(cmd.WithdrawUserID);
            var receiverBankAccountID = 0;

            if (receiver.VerifyTradePassword(PasswordHelper.EncryptMD5(cmd.TradePassword)))
            {
                if (account == null) throw new AccountBalanceNotEnoughException();

                if (!cmd.ReceiverBankAccountID.HasValue)
                {
                    var receiverBankAccount = IoC.Resolve<IWithdrawReceiverAccountRepository>().FindByAccountAndBank(cmd.BankAccount, cmd.Bank);

                    if (receiverBankAccount == null)
                    {
                        receiverBankAccount = new WithdrawReceiverBankAccount(cmd.WithdrawUserID, cmd.Bank, cmd.BankAccount, receiver.Membership.RealName);

                        repos.Add(receiverBankAccount);
                    }

                    receiverBankAccountID = receiverBankAccount.ID;
                }
                else receiverBankAccountID = cmd.ReceiverBankAccountID.Value;

                var cnyWithdraw = new CNYWithdraw(cmd.WithdrawUserID, account.ID, cmd.Amount, receiverBankAccountID);

                repos.Add(cnyWithdraw);
            }
            else throw new TradePasswordErrorException();
        }

        public void Execute(CNYWithdrawVerify cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);

            cnyWithdraw.Verify(cmd.ByUserID, cmd.Memo);
        }

        public void Execute(SubmitCNYWithdrawToProcess cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);

            cnyWithdraw.SubmitToProcess(cmd.ByUserID);
        }

        public void Execute(CNYWithdrawModifyReceiverBankAccount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);
            var receiver = repos.FindById<User>(cnyWithdraw.UserID);

            var receiverNewBankAccount = new WithdrawReceiverBankAccount(cnyWithdraw.UserID, cmd.Bank, cmd.BankAccount, receiver.Membership.RealName);

            repos.Add(receiverNewBankAccount);

            cnyWithdraw.ModifyReceiverBankAccount(receiverNewBankAccount.ID, cmd.ByUserID);
        }

        public void Execute(CNYWithdrawMarkAsSuccess cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);

            cnyWithdraw.Complete(cmd.TransferAccountID, cmd.TransferNo, cmd.ByUserID);
        }

        public void Execute(CNYWithdrawMarkAsTransferFail cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);

            cnyWithdraw.TranferFail(cmd.Memo, cmd.ByUserID);
        }

        public void Execute(CNYWithdrawCancel cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyWithdraw = repos.FindById<CNYWithdraw>(cmd.WithdrawID);

            cnyWithdraw.Cancel(cmd.Memo, cmd.ByUserID);
        }
    }
}
