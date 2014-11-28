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
    public class CNYDepositCommandExecutors : ICommandExecutor<CreateCommonCNYDeposit>,
                                              ICommandExecutor<CompleteCNYDeposit>,
                                              ICommandExecutor<CreateInboundCNYDeposit>,
                                              ICommandExecutor<UndoCompleteCNYDeposit>
    {
        public void Execute(CreateCommonCNYDeposit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var account = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cmd.UserID, CurrencyType.CNY);

            if (account == null)
            {
                account = AccountFactory.CreateAccount(cmd.UserID, CurrencyType.CNY);
                repos.Add(account);
            }

            var cnyDeposit = new CNYDeposit(cmd.UserID, account.ID, cmd.Amount);

            repos.Add(cnyDeposit);
        }
        public void Execute(CompleteCNYDeposit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyDeposit = repos.FindById<CNYDeposit>(cmd.DepositID);

            cnyDeposit.Complete(cmd.ByUserID);
        }

        public void Execute(UndoCompleteCNYDeposit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var cnyDeposit = repos.FindById<CNYDeposit>(cmd.DepositID);

            cnyDeposit.UndoComplete(cmd.ByUserID);
        }

        public void Execute(CreateInboundCNYDeposit cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var account = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(cmd.UserID, CurrencyType.CNY);

            if (account == null)
            {
                account = AccountFactory.CreateAccount(cmd.UserID, CurrencyType.CNY);
                repos.Add(account);
            }

            var cnyDeposit = new CNYDeposit(cmd.UserID, account.ID, cmd.Amount);
            var nullUserID = 0;

            repos.Add(cnyDeposit);

            cnyDeposit.Complete(nullUserID);
        }
    }
}
