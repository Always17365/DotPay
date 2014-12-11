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
    public class RippleInboundTxCommandsExecutor : ICommandExecutor<CreateInboundTx>,
                                                   ICommandExecutor<CreateThirdPartyPaymentInboundTx>,
                                                   ICommandExecutor<CompleteThirdPartyPaymentInboundTx>
    {
        public void Execute(CreateInboundTx cmd)
        {
            var rippleInboundTx = new RippleInboundTx(cmd.TxID, cmd.DestinationTag, cmd.Amount);

            IoC.Resolve<IRepository>().Add(rippleInboundTx);
        }

        public void Execute(CreateThirdPartyPaymentInboundTx cmd)
        {
            var rippleInboundTx = new RippleInboundToThirdPartyPaymentTx(cmd.PayWay, cmd.Destination);

            IoC.Resolve<IRepository>().Add(rippleInboundTx);

            cmd.Result = rippleInboundTx.ID;
        }

        public void Execute(CompleteThirdPartyPaymentInboundTx cmd)
        {
            //destinationtag是to tpp的ID
            //此处不能用txid作为查找条件，因为此时的数据库中还没有txid
            var rippleInboundTx = IoC.Resolve<IInboundToThirdPartyPaymentTxRepository>().FindByIDAndPayway(cmd.DestinationTag, cmd.PayWay);

            rippleInboundTx.Complete(cmd.TxId, cmd.Amount);
        }
    }
}
