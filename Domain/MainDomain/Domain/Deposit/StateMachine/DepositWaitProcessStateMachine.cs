using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using DotPay.Common;

namespace DotPay.MainDomain
{
    internal class DepositWaitProcessStateMachine : IDepositStateMachine
    {
        private IDeposit _deposit;

        public DepositWaitProcessStateMachine(IDeposit deposit)
        {
            Check.Argument.IsNotNull(deposit, "deposit");

            this._deposit = deposit;
        }
        public void VerifyForCNY(int byUserID, PayWay payway, Bank bank, int fundSourceID, decimal fundAmount)
        {
            this._deposit.RaiseEvent(new CNYDepositVerified(this._deposit.ID, payway, this._deposit.SequenceNo, byUserID, fundSourceID, fundAmount));
        }

        public void CompleteForCNY(int byUserID) { throw new DepositNotVerifyException(); }
        public void UndoCompleteForCNY(int byUserID) { throw new DepositIsNotCompletedException(); }

        public void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string txid, decimal txAmount)
        {
            this._deposit.RaiseEvent(new VirtualCoinDepositVerified(currency, this._deposit.ID, this._deposit.SequenceNo, byUserID, txid, txAmount));
        }
        public void CompleteForVirtualCoin(CurrencyType currency, int byUserID) { throw new DepositNotVerifyException(); }
    }
}
