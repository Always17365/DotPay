using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class WithdrawCompleteStateMachine : IWithdrawStateMachine
    {
        private IWithdraw _withdraw;

        public WithdrawCompleteStateMachine(IWithdraw withdraw)
        {
            Check.Argument.IsNotNull(withdraw, "withdraw");

            this._withdraw = withdraw;
        }

        public void VerifyForCNY(int byUserID, string memo) { throw new WithdrawIsCompletedException(); }
        public void SkipVerifyForCNY() { throw new WithdrawIsCompletedException(); }
        public void SetFee(CurrencyType currency, decimal fee) { throw new WithdrawIsCompletedException(); }
        public void ModifyReceiverBankAccountID(int receiverBankAccountID, int byUserID)
        {
            throw new WithdrawIsCompletedException();
        }

        public void SubmitToProcess(int byUserID) { throw new WithdrawIsCompletedException(); }
        public void TranferFail(int byUserID, string memo) { throw new WithdrawIsCompletedException(); }
        public void CompleteForCNY(int transferAccountID, string transferNo, int byUserID) { throw new WithdrawIsCompletedException(); }
        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsCompletedException(); }
        public void SkipVerifyForVirtualCoin(CurrencyType currency) { throw new WithdrawIsCompletedException(); }
        public void CompleteForVirtualCoin(string txID, decimal txfee, CurrencyType currencyType) { throw new WithdrawIsCompletedException(); }
        public void CancelForCNY(int byUserID, string memo) { throw new WithdrawIsCompletedException(); }
        public void CancelForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsCompletedException(); }
        public void MarkVirtualCoinTransferFail(CurrencyType currency, int byUserID) { throw new WithdrawIsCompletedException(); } 
    }
}
