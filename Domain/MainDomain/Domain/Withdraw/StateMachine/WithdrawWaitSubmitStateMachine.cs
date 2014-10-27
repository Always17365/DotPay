using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public class WithdrawWaitSubmitStateMachine : IWithdrawStateMachine
    {
        private IWithdraw _withdraw;

        public WithdrawWaitSubmitStateMachine(IWithdraw withdraw)
        {
            Check.Argument.IsNotNull(withdraw, "withdraw");

            this._withdraw = withdraw;
        }

        public void VerifyForCNY(int byUserID, string memo) { throw new WithdrawIsVerifiedException(); }
        public void SkipVerifyForCNY() { throw new WithdrawIsVerifiedException(); }
        public void SetFee(CurrencyType currency, decimal fee) { throw new WithdrawIsVerifiedException(); }

        public void ModifyReceiverBankAccountID(int receiverBankAccountID, int byUserID)
        {
            throw new WithdrawIsNotSubmitToProcessException();
        }

        public void SubmitToProcess(int byUserID)
        {
            this._withdraw.RaiseEvent(new CNYWithdrawSubmitedToProcess(this._withdraw.ID, this._withdraw.Amount, this._withdraw.Fee, byUserID));
        }

        public void TranferFail(int byUserID, string memo) { throw new WithdrawIsNotSubmitToProcessException(); }
        public void CompleteForCNY(int transferAccountID, string transferNo, int byUserID) { throw new WithdrawIsNotSubmitToProcessException(); }
        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsVerifiedException(); }
        public void SkipVerifyForVirtualCoin(CurrencyType currency) { throw new WithdrawIsVerifiedException(); }
        public void CompleteForVirtualCoin(string txID, decimal txfee, CurrencyType currencyType) { throw new WithdrawIsNotSubmitToProcessException(); }
        public void CancelForCNY(int byUserID, string memo) { throw new WithdrawIsNotSubmitToProcessException(); }
        public void CancelForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsVerifiedException(); }

        public void MarkVirtualCoinTransferFail(CurrencyType currency, int byUserID) { throw new WithdrawIsNotSubmitToProcessException(); }
    }
}
