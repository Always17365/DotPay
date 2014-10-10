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
    public class AccountVersionFactory
    {
        public static AccountVersion CreateAccountVersion(int userID, int accountID, decimal amount,
                                     decimal balance, decimal locked, decimal @in, decimal @out,
                                     int modifyID, int modifyType, CurrencyType currencyType)
        {
            AccountVersion accountVersion;

            switch (currencyType)
            {
                case CurrencyType.CNY:
                    accountVersion = new CNYAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return accountVersion;
        }

        public static AccountVersion CreateAccountVersion(int userID, int accountID, decimal amount,
                                    decimal balance, decimal locked, decimal @in, decimal @out,
                                    string depositCode, int modifyType, CurrencyType currencyType)
        {
            AccountVersion accountVersion;

            switch (currencyType)
            {
                case CurrencyType.CNY:
                    accountVersion = new CNYAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
            
                default:
                    throw new NotImplementedException();
            }

            return accountVersion;
        }
    }
}
