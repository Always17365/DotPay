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
    public class STRWithdraw : VirtualCoinWithdraw
    {
        #region ctor
        protected STRWithdraw() { }
        public STRWithdraw(int withdrawUserID, int accountID, decimal amount, string receiveAddress) :
            base(CurrencyType.STR, withdrawUserID, accountID, amount, receiveAddress) { }
        #endregion

        public override void Complete(string txID, decimal txfee)
        {
            this.StateMachine.CompleteForVirtualCoin(txID, txfee, CurrencyType.STR);
        }

        public override void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.STR, fee);
        }

        public override void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForVirtualCoin(CurrencyType.STR, byUserID, memo);
        }

        public override void SkipVerify()
        {
            this.StateMachine.SkipVerifyForVirtualCoin(CurrencyType.STR);
        }
        public override void MarkTransferFail(int byUserID)
        {
            this.StateMachine.MarkVirtualCoinTransferFail(CurrencyType.STR, byUserID);
        }

        public override void Cancel(int byUserID, string memo)
        {
            this.StateMachine.CancelForVirtualCoin(CurrencyType.STR, byUserID, memo);
            throw new NotImplementedException();
        }
    }
}
