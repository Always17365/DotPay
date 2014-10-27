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
    public class CNYDepositCommandExecutors : ICommandExecutor<CompleteCNYDeposit>,
                                              ICommandExecutor<UndoCompleteCNYDeposit>
    {


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
    }
}
