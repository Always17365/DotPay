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
    public class WithdrawWaitVerifyStateMachine : IWithdrawStateMachine
    {
        private IWithdraw _withdraw;

        public WithdrawWaitVerifyStateMachine(IWithdraw withdraw)
        {
            Check.Argument.IsNotNull(withdraw, "withdraw");

            this._withdraw = withdraw;
        }

        public void VerifyForCNY(int byUserID, string memo)
        {
            this._withdraw.RaiseEvent(new CNYWithdrawVerified(this._withdraw.ID, this._withdraw.Amount, this._withdraw.Fee, memo, byUserID));
        }

        public void SkipVerifyForCNY()
        {
            this._withdraw.RaiseEvent(new CNYWithdrawSkipVerify(this._withdraw.ID));
        }

        public void ModifyReceiverBankAccountID(int receiverBankAccountID, int byUserID)
        {
            throw new WithdrawIsNotVerifyException();
        }

        public void SubmitToProcess(int byUserID) { throw new WithdrawIsNotVerifyException(); }
        public void TranferFail(int byUserID, string memo) { throw new WithdrawIsNotVerifyException(); }
        public void CompleteForCNY(int transferAccountID, string transferNo, int byUserID) { throw new WithdrawIsNotVerifyException(); }
        public void CancelForCNY(int byUserID, string memo) { throw new WithdrawIsNotVerifyException(); }
        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string memo)
        {
            this._withdraw.RaiseEvent(new VirtualCoinWithdrawVerified(currency, this._withdraw.ID, this._withdraw.UniqueID, this._withdraw.Amount, this._withdraw.Fee, memo, byUserID));
        }
        public void SkipVerifyForVirtualCoin(CurrencyType currency)
        {
            var withdraw = this._withdraw as VirtualCoinWithdraw;
            this._withdraw.RaiseEvent(new VirtualCoinWithdrawSkipVerify(currency, this._withdraw.ID, this._withdraw.UniqueID, withdraw));
        }
        public void CompleteForVirtualCoin(string txID, decimal txfee, CurrencyType currencyType) { throw new WithdrawIsNotVerifyException(); }


        public void SetFee(CurrencyType currency, decimal fee)
        {
            if (currency == CurrencyType.CNY)
            {
                var withdraw = this._withdraw as CNYWithdraw;
                this._withdraw.RaiseEvent(new CNYWithdrawSetFee(withdraw, fee));
            }
            else
            {
                var withdraw = this._withdraw as VirtualCoinWithdraw;
                this._withdraw.RaiseEvent(new VirtualCoinWithdrawSetFee(withdraw, withdraw.AccountID, currency, fee));
            }
        }

        public void CancelForVirtualCoin(CurrencyType currency, int byUserID, string memo) { throw new WithdrawIsNotVerifyException(); }

        public void MarkVirtualCoinTransferFail(CurrencyType currency, int byUserID) { throw new WithdrawIsNotVerifyException(); }
    }
}
