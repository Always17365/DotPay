using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.Common;

namespace DotPay.QueryService
{
    public interface IWithdrawReceiverAccountQuery
    {
        IEnumerable<WithdrawReceiverAccountModel> GetAvailableReceiveAccountByUserID(int userID);
        WithdrawReceiverAccountModel GetLastAvailableReceiveAccountByUserID(int userID);
    }
}
