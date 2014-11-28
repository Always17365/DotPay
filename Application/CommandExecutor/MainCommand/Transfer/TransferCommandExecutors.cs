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
    public class TransferCommandExecutors : ICommandExecutor<CreateInsideTransfer>,
                                            ICommandExecutor<SubmitInsideTransfer>,
                                            ICommandExecutor<OutboundTransfer>,
                                            ICommandExecutor<CreateThirdPartyPaymentTransfer>,
                                            ICommandExecutor<ThirdPartyPaymentTransferComplete>,
                                            ICommandExecutor<ThirdPartyPaymentTransferFail>
    {
        public void Execute(CreateInsideTransfer cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var insideTransfer = InsideTransferTransactionFactory.CreateInsideTransferTransaction(cmd.FromUserID, cmd.ToUserID, cmd.Currency, cmd.Amount, PayWay.Inside, cmd.Description);
            cmd.Result = insideTransfer.SequenceNo;
            IoC.Resolve<IRepository>().Add(insideTransfer);

        }

        public void Execute(OutboundTransfer cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            throw new NotImplementedException();
        }

        public void Execute(SubmitInsideTransfer cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var insideTransfer = IoC.Resolve<IInsideTransferTransactionRepository>().FindTransferTxByID(cmd.InsideTransferSeq, cmd.Currency);
            var fromUser = IoC.Resolve<IRepository>().FindById<User>(insideTransfer.FromUserID);

            if (!fromUser.VerifyTradePassword(PasswordHelper.EncryptMD5(cmd.TradePassword)))
                throw new TradePasswordErrorException();
            else
                insideTransfer.Complete();
        }
        //public void Execute(ConfirmInsideTransfer cmd)
        //{ 
        //    var insideTransfer = IoC.Resolve<IInsideTransferTransactionRepository>().FindTransferTxByID(cmd.InsideTransferSeq, cmd.Currency);

        //    insideTransfer.Confirm();
        //}

        public void Execute(CreateThirdPartyPaymentTransfer cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var tpptx = ToThirdPartyPaymentTransactionFactory.CreateInboundTransferTransaction(cmd.TxId, cmd.Account, cmd.Amount, cmd.PayWay, cmd.SourcePayway);

            IoC.Resolve<IRepository>().Add(tpptx);
        }

        public void Execute(ThirdPartyPaymentTransferComplete cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var ttpTx = IoC.Resolve<IInboundTransferToThirdPartyPaymentTxRepository>().FindTransferTxByIDAndPayway(cmd.TransferId, cmd.PayWay);

            ttpTx.Complete(cmd.TransferNo, cmd.ByUserID);
        }

        public void Execute(ThirdPartyPaymentTransferFail cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var ttpTx = IoC.Resolve<IInboundTransferToThirdPartyPaymentTxRepository>().FindTransferTxByIDAndPayway(cmd.TransferId, cmd.PayWay);

            ttpTx.Fail(cmd.Reason, cmd.ByUserID);
        }
    }
}
