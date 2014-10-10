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
