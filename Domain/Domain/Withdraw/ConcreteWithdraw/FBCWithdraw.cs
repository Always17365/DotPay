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
    public class FBCWithdraw : VirtualCoinWithdraw
    {
        #region ctor
        protected FBCWithdraw() { }
        public FBCWithdraw(int withdrawUserID, int accountID, decimal amount, string receiveAddress) :
            base(CurrencyType.FBC, withdrawUserID, accountID, amount, receiveAddress) { }
        #endregion

        public override void Complete(string txID, decimal txfee)
        {
            this.StateMachine.CompleteForVirtualCoin(txID, txfee, CurrencyType.FBC);
        }

        public override void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.FBC, fee);
        }

        public override void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForVirtualCoin(CurrencyType.FBC, byUserID, memo);
        }

        public override void SkipVerify()
        {
            this.StateMachine.SkipVerifyForVirtualCoin(CurrencyType.FBC);
        }
        public override void MarkTransferFail(int byUserID)
        {
            this.StateMachine.MarkVirtualCoinTransferFail(CurrencyType.FBC, byUserID);
        }

        public override void Cancel(int byUserID, string memo)
        {
            this.StateMachine.CancelForVirtualCoin(CurrencyType.FBC, byUserID, memo);
            throw new NotImplementedException();
        }
    }
}
