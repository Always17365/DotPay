using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain
{
    public interface IDepositStateMachine
    {
        void VerifyForCNY(int byUserID, PayWay payway,Bank bank, int fundSourceID, decimal fundAmount);
        void CompleteForCNY(int byUserID);
        void UndoCompleteForCNY(int byUserID);

        void VerifyForVirtualCoin(CurrencyType currency, int byUserID, string txid, decimal txAmount);
        void CompleteForVirtualCoin(CurrencyType currencyType, int byUserID);
    }
}
