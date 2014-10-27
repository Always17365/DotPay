using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleCommand
{
    public class RecordActiveWalletTx : FC.Framework.Command
    {
        public RecordActiveWalletTx(string txhash, string txblob, string source, string destination, decimal amount, decimal fee)
        {
            this.TxID = txhash;
            this.TxBlob = txblob;
            this.Source = source;
            this.Destination = destination;
            this.Amount = amount;
            this.Fee = fee;
        }
        public virtual string TxID { get; protected set; }
        public virtual string TxBlob { get; protected set; }
        public string Source { get; protected set; }
        public string Destination { get; protected set; }
        public decimal Amount { get; protected set; }
        public decimal Fee { get; protected set; }
        public int Result { get; protected set; }
    }

    public class ActiveWalletTxSubmitSuccess : FC.Framework.Command
    {
        public ActiveWalletTxSubmitSuccess(int activeTxId)
        {
            this.RippleActiveTxID = activeTxId;
        }
        public virtual int RippleActiveTxID { get; protected set; }
    }

    public class ActiveWalletTxSubmitFail : FC.Framework.Command
    {
        public ActiveWalletTxSubmitFail(int activeTxId, string reason)
        {
            this.RippleActiveTxID = activeTxId;
            this.FailReason = reason;
        }
        public virtual int RippleActiveTxID { get; protected set; }
        public virtual string FailReason { get; protected set; }
    }
}
