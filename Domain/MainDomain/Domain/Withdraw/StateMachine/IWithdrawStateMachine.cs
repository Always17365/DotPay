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

namespace DotPay.MainDomain
{
    public interface IWithdrawStateMachine
    {
        void SetFee(CurrencyType currency, decimal fee);

        void VerifyForCNY(int byUserID, string memo);
        void SkipVerifyForCNY();
        void ModifyReceiverBankAccountID(int userBankAccountID, int byUserID);
        void SubmitToProcess(int byUserID);
        void TranferFail(int byUserID, string memo);
        void CompleteForCNY(int transferAccountID, string transferNo, int byUserID);
        void CancelForCNY(int byUserID, string memo);

        void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string memo);
        void SkipVerifyForVirtualCoin(CurrencyType currency);
        void CompleteForVirtualCoin(string txID, decimal txfee, CurrencyType currencyType);
        void CancelForVirtualCoin(CurrencyType currency, int byUserID,string memo);
        void MarkVirtualCoinTransferFail(CurrencyType currency, int byUserID);
    }
}
