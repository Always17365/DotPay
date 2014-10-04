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

namespace DotPay.Command.Executor.Trade
{
    public class ReceivePaymentTransactionCommandExecutors : ICommandExecutor<CreateReceivePaymentTransaction>,
                                                             ICommandExecutor<ConfirmReceivePaymentTransaction>
    {
        public void Execute(CreateReceivePaymentTransaction cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var receiveTx = ReceivePaymentTransactionFactory.CreateReceivePaymentTransaction(cmd.TxID, cmd.ReceiveCoinAddress, cmd.Amount, cmd.Currency, cmd.ByUserID);

            IoC.Resolve<IRepository>().Add(receiveTx);
        }

        public void Execute(ConfirmReceivePaymentTransaction cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            if (cmd.ReceivePaymentTxId == 0)
            {
                var receiveTx = IoC.Resolve<IReceivePaymentTransactionRepository>().FindByTxIDAndCurrency(cmd.TxID, cmd.Currency);

                if (cmd.Currency == CurrencyType.IFC && receiveTx.Amount.ToFixed(4) == cmd.Amount.ToFixed(4))
                    receiveTx.Confirm(cmd.Confirmations, cmd.TxID);

                else if (receiveTx.Address == cmd.ReceiveCoinAddress && receiveTx.Amount.ToFixed(4) == cmd.Amount.ToFixed(4))
                    receiveTx.Confirm(cmd.Confirmations, cmd.TxID);
                else
                {
                    Log.Error("虚拟币{0}充值确认时，发生了错误，金额和收款地址不符合:数据库金额{1}收款地址{2};网络查询金额{3}收款地址{4}",
                               cmd.Currency, receiveTx.Amount, receiveTx.Address, cmd.Amount, cmd.ReceiveCoinAddress);
                }
            }
            else
            {
                var receiveTx = IoC.Resolve<IReceivePaymentTransactionRepository>().FindByIDAndCurrency(cmd.ReceivePaymentTxId, cmd.Currency);

                if (receiveTx.Amount != cmd.Amount) throw new PaymentTransactionAmountError();
                receiveTx.Confirm(cmd.Confirmations, cmd.TxID);
            }
        }
    }
}