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
    public class TransferCommandExecutors : ICommandExecutor<InsideTransfer>,
                   ICommandExecutor<InsideTransferComplete>,
                                         ICommandExecutor<OutsideTransfer>
    {
        public void Execute(InsideTransfer cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var insideTransfer = InsideTransferTransactionFactory.CreateInsideTransferTransaction(cmd.FromUserID, cmd.ToUserID, cmd.Currency, cmd.Amount, PayWay.Inside, cmd.Description);

            IoC.Resolve<IRepository>().Add(insideTransfer);
        }

        public void Execute(OutsideTransfer cmd)
        {
            throw new NotImplementedException();
        }

        public void Execute(InsideTransferComplete cmd)
        {
            var insideTransfer = IoC.Resolve<IInsideTransferTransactionRepository>().FindTransferTxByID(cmd.InsideTransferID, cmd.Currency);

            insideTransfer.Complete();
        }
    }
}
