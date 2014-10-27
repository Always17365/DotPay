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
    public class BankOutletsCommandExecutors : ICommandExecutor<CreateBankOutlets>,
                                               ICommandExecutor<RemoveBankOutlets>
    {

        public void Execute(CreateBankOutlets cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var bankOutlets = new BankOutlets(cmd.ProvinceID, cmd.CityID, cmd.Bank, cmd.BankName);

            repos.Add(bankOutlets);
        }

        public void Execute(RemoveBankOutlets cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var repos = IoC.Resolve<IRepository>();
            var bankOutlets = repos.FindById<BankOutlets>(cmd.BankOutletsId);

            bankOutlets.MarkAsDelete(cmd.ByUserID);
        }
    }
}
