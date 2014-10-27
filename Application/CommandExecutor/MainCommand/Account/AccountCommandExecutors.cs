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
    public class AccountCommandExecutors : ICommandExecutor<CreateAccount>
    {
        public void Execute(CreateAccount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IAccountRepository>();

            var existAccount = repos.FindByUserIDAndCurrency(cmd.UserID, cmd.Currency);

            if (existAccount == null)
            {
                var account = AccountFactory.CreateAccount(cmd.UserID, cmd.Currency);
                repos.Add(account);
            }
        }

    }
}
