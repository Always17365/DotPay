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
    public class CapitalAccountCommandExecutors : ICommandExecutor<CreateCapitalAccount>,
                                           ICommandExecutor<EnableCapitalAccount>,
                                           ICommandExecutor<DisableCapitalAccount>
    {
        public void Execute(CreateCapitalAccount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var capitalAccount = new CapitalAccount(cmd.Bank, cmd.BankAccount, cmd.OwnerName, cmd.CreateBy);

            IoC.Resolve<IRepository>().Add(capitalAccount);
        }

        public void Execute(EnableCapitalAccount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<CapitalAccount>(cmd.CapitalAccountID);

            currency.Enable(cmd.EnableBy);
        }

        public void Execute(DisableCapitalAccount cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var currency = IoC.Resolve<IRepository>().FindById<CapitalAccount>(cmd.CapitalAccountID);

            currency.Disable(cmd.DisableBy);
        }
    }
}
