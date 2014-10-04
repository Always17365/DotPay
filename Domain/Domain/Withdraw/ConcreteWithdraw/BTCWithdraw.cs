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
    public class BTCWithdraw : VirtualCoinWithdraw
    {
        #region ctor
        protected BTCWithdraw() { }
        public BTCWithdraw(int withdrawUserID, int accountID, decimal amount, string receiveAddress) :
            base(CurrencyType.BTC, withdrawUserID, accountID, amount, receiveAddress) { }
        #endregion

        public override void Complete(string txID, decimal txfee)
        {
            this.StateMachine.CompleteForVirtualCoin(txID, txfee, CurrencyType.BTC);
        }

        public override void SetFee(decimal fee)
        {
            this.StateMachine.SetFee(CurrencyType.BTC, fee);
        }

        public override void Verify(int byUserID, string memo)
        {
            this.StateMachine.VerifyForVirtualCoin(CurrencyType.BTC, byUserID, memo);
        }

        public override void SkipVerify()
        {
            this.StateMachine.SkipVerifyForVirtualCoin(CurrencyType.BTC);
        }

        public override void Cancel(int byUserID, string memo)
        {
            this.StateMachine.CancelForVirtualCoin(CurrencyType.BTC, byUserID, memo);
        }
        public override void MarkTransferFail(int byUserID)
        {
            this.StateMachine.MarkVirtualCoinTransferFail(CurrencyType.BTC, byUserID);
        }
    }
}
