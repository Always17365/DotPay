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
    public class LTCWithdraw : VirtualCoinWithdraw
    {
        #region ctor
        protected LTCWithdraw() { }
        public LTCWithdraw(int withdrawUserID, int accountID, decimal amount, string receiveAddress) :
            base(CurrencyType.LTC, withdrawUserID, accountID, amount, receiveAddress) { }
        #endregion

        public override void Complete(string txID, decimal txfee)
        {
            this.StateMachine.CompleteForVirtualCoin(txID, txfee, CurrencyType.LTC);
        }

        public override void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.LTC, fee);
        }

        public override void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForVirtualCoin(CurrencyType.LTC, byUserID, memo);
        }

        public override void SkipVerify()
        {
            this.StateMachine.SkipVerifyForVirtualCoin(CurrencyType.LTC);
        }

        public override void Cancel(int byUserID, string memo)
        {
            this.StateMachine.CancelForVirtualCoin(CurrencyType.LTC, byUserID, memo);
        }

        public override void MarkTransferFail(int byUserID)
        {
            this.StateMachine.MarkVirtualCoinTransferFail(CurrencyType.LTC, byUserID);
        }
    }
}
