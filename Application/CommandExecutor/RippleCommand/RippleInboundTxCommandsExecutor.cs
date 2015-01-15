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
            var rippleInboundTx = new RippleInboundToThirdPartyPaymentTx(cmd.PayWay, cmd.Destination, cmd.RealName, cmd.Amount, cmd.SendAmount, cmd.Memo);

            IoC.Resolve<IRepository>().Add(rippleInboundTx);
            cmd.ResultDestinationTag = rippleInboundTx.ID;
            cmd.ResultInvoiceID = rippleInboundTx.InvoiceID;
        }

        public void Execute(CompleteThirdPartyPaymentInboundTx cmd)
        {
            var repos = IoC.Resolve<IInboundToThirdPartyPaymentTxRepository>();

            //查一次，是防止txid的重复，导致一笔tx,生成两笔转账
            var rippleInboundTx = repos.FindByTxId(cmd.TxId);

            if (rippleInboundTx == null)
            {
                //destinationtag是to tpp的ID
                //此处不能用txid作为查找条件，因为此时的数据库中还没有txid
                rippleInboundTx = repos.FindById<RippleInboundToThirdPartyPaymentTx>(cmd.DestinationTag);

                if (!string.IsNullOrEmpty(cmd.InvoiceId) && !cmd.InvoiceId.Equals(rippleInboundTx.InvoiceID, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("invoice_id不匹配" + IoC.Resolve<IJsonSerializer>().Serialize(cmd));
                }
                rippleInboundTx.Complete(cmd.TxId, cmd.SendAmount);
            }
        }
    }
}
