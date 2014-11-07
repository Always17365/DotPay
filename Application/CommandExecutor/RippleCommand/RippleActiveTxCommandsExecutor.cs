using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using DotPay.RippleCommand;
using FC.Framework.Repository;
using DotPay.RippleDomain;

namespace RippleCommand
{
    public class RippleActiveTxCommandsExecutor : ICommandExecutor<RecordActiveWalletTx>
    {
        public void Execute(RecordActiveWalletTx cmd)
        {
            var rippleActiveTx = new RippleActiveTx(cmd.TxID, cmd.TxBlob, cmd.Source, cmd.Destination, cmd.Amount, cmd.Fee);

            IoC.Resolve<IRepository>().Add(rippleActiveTx);
        }
    }
}
