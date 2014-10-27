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
    public class WithdrawProcessingStateMachine : IWithdrawStateMachine
    {
        private IWithdraw _withdraw;

        public WithdrawProcessingStateMachine(IWithdraw withdraw)
        {
            Check.Argument.IsNotNull(withdraw, "withdraw");

            this._withdraw = withdraw;
        }

        public void VerifyForCNY(int byUserID, string memo) { throw new WithdrawIsVerifiedException(); }
        public void SkipVerifyForCNY() { throw new WithdrawIsVerifiedException(); }

        public void ModifyReceiverBankAccountID(int receiverBankAccountID, int byUserID) { throw new WithdrawIsProcessingException(); }
        public void SetFee(CurrencyType currency, decimal fee) { throw new WithdrawIsVerifiedException(); }

        public void SubmitToProcess(int byUserID) { throw new WithdrawIsProcessingException(); }
        public void CancelForCNY(int byUserID, string memo) { throw new WithdrawIsProcessingException(); }

        public void TranferFail(int byUserID, string memo)
        {
            this._withdraw.RaiseEvent(new CNYWithdrawTransferFailed(this._withdraw.ID, this._withdraw.Amount, this._withdraw.Fee, memo, byUserID));
        }

        public void CompleteForCNY(int transferAccountID, string transferNo, int byUserID)
        {
            var cnyWithdraw = this._withdraw as CNYWithdraw;

            var receiverOldBankAccount = IoC.Resolve<IRepository>()
                                            .FindById<WithdrawReceiverBankAccount>(cnyWithdraw.ReceiverBankAccountID);

            receiverOldBankAccount.MarkAsValid(byUserID);

            this._withdraw.RaiseEvent(new CNYWithdrawCompleted(this._withdraw.ID, this._withdraw.UserID, transferAccountID, transferNo, byUserID));
        }

        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsVerifiedException(); }
        public void SkipVerifyForVirtualCoin(CurrencyType currency) { throw new WithdrawIsVerifiedException(); }

        public void CompleteForVirtualCoin(string txID, decimal txfee, CurrencyType currencyType)
        {
            this._withdraw.RaiseEvent(new VirtualCoinWithdrawCompleted(this._withdraw.ID, this._withdraw.UserID, txID, txfee, currencyType));
        }

        public void CancelForVirtualCoin(CurrencyType currency, int byUserID, string memo)
        {
            this._withdraw.RaiseEvent(new VirtualCoinWithdrawCanceled(this._withdraw.UniqueID, currency, this._withdraw.AccountID, this._withdraw.Amount, this._withdraw.Fee, memo, byUserID));
        }

        public void MarkVirtualCoinTransferFail(CurrencyType currency, int byUserID)
        {
            this._withdraw.RaiseEvent(new VirtualCoinWithdrawTranferFailed(this._withdraw.ID, this._withdraw.Amount, this._withdraw.Fee, string.Empty, byUserID));
        }
    }
}
