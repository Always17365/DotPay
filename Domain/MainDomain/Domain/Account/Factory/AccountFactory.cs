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
    public class AccountFactory
    {
        public static Account CreateAccount(int userID, CurrencyType currencyType)
        {
            Account account;

            switch (currencyType)
            {
                case CurrencyType.CNY:
                    account = new CNYAccount(userID);
                    break;
               
                default:
                    throw new NotImplementedException();
            }

            return account;
        }
    }
}
