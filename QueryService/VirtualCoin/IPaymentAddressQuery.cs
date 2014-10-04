
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface IPaymentAddressQuery
    {
        string GetPaymentAddressByUserID(CurrencyType currency, int userID);
        int GetUserIDByPaymentAddress(CurrencyType currency, string paymentAddress);
    }
}
