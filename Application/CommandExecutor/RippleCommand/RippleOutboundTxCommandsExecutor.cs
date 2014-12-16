using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using DotPay.RippleCommand;
using FC.Framework.Repository;
using DotPay.RippleDomain.Repository;
using DotPay.RippleDomain;

namespace RippleCommand
{
    public class RippleOutboundTxCommandsExecutor : ICommandExecutor<CreateRippleOutboundTx>,
                                                    ICommandExecutor<SignRippleOutboundTx>,
                                                    ICommandExecutor<SubmitRippleOutboundTxSuccess>,
                                                    ICommandExecutor<SubmitRippleOutboundTxFail>
    {
        public void Execute(CreateRippleOutboundTx cmd)
        {
            var rippleOutboundTx = new RippleOutboundTransferTx(cmd.Destination, cmd.DestinationTag, cmd.TargetCurrency, cmd.TargetAmount, cmd.SourceSendMaxAmount, cmd.RipplePaths);

            IoC.Resolve<IRepository>().Add(rippleOutboundTx);
        }

        //public void Execute(CreateThirdPartyPaymentInboundTx cmd)
        //{
        //    var rippleInboundTx = new RippleInboundToThirdPartyPaymentTx(cmd.PayWay, cmd.Destination);

        //    IoC.Resolve<IRepository>().Add(rippleInboundTx);

        //    cmd.Result = rippleInboundTx.ID;
        //}

        public void Execute(SignRippleOutboundTx cmd)
        {
            var rippleOutboundTx = IoC.Resolve<IRepository>().FindById<RippleOutboundTransferTx>(cmd.OutboundTxId);

            rippleOutboundTx.MarkSigned(cmd.TxHash, cmd.TxBlob);
        }

        public void Execute(SubmitRippleOutboundTxSuccess cmd)
        {
            var rippleOutboundTx = IoC.Resolve<IRippleOutboundTransferTxRepository>().FindByTxId(cmd.TxId);

            rippleOutboundTx.MarkSubmitedSuccess(cmd.TxId);
        }

        public void Execute(SubmitRippleOutboundTxFail cmd)
        {
            var rippleOutboundTx = IoC.Resolve<IRippleOutboundTransferTxRepository>().FindByTxId(cmd.TxId);

            rippleOutboundTx.MarkSubmitedSuccess(cmd.TxId);
        }
    }
}
