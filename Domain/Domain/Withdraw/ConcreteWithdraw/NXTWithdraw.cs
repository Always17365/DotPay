using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class NXTWithdraw : VirtualCoinWithdraw
    {
        #region ctor
        protected NXTWithdraw() { }
        public NXTWithdraw(int withdrawUserID, int accountID, decimal amount, string receiveAddress) :
            base(CurrencyType.NXT, withdrawUserID, accountID, amount, receiveAddress) { }
        #endregion

        public override void Complete(string txID, decimal txfee)
        {
            this.StateMachine.CompleteForVirtualCoin(txID, txfee, CurrencyType.NXT);
        }

        public override void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.NXT, fee);
        }

        public override void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForVirtualCoin(CurrencyType.NXT, byUserID, memo);
        }

        public override void SkipVerify()
        {
            this.StateMachine.SkipVerifyForVirtualCoin(CurrencyType.NXT);
        }
        public override void MarkTransferFail(int byUserID)
        {
            this.StateMachine.MarkVirtualCoinTransferFail(CurrencyType.NXT, byUserID);
        }

        public override void Cancel(int byUserID, string memo)
        {
            this.StateMachine.CancelForVirtualCoin(CurrencyType.NXT, byUserID, memo);
            throw new NotImplementedException();
        }
    }
}
