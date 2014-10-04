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
                case CurrencyType.BTC:
                    accountVersion = new BTCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                case CurrencyType.LTC:
                    accountVersion = new LTCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                case CurrencyType.IFC:
                    accountVersion = new IFCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                case CurrencyType.NXT:
                    accountVersion = new NXTAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                case CurrencyType.DOGE:
                    accountVersion = new DOGEAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;

                case CurrencyType.STR:
                    accountVersion = new STRAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, modifyID, modifyType);
                    break;
                case CurrencyType.FBC:
                    accountVersion = new FBCAccountVersion(userID, accountID,
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
                case CurrencyType.BTC:
                    accountVersion = new BTCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.LTC:
                    accountVersion = new LTCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.IFC:
                    accountVersion = new IFCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.NXT:
                    accountVersion = new NXTAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.DOGE:
                    accountVersion = new DOGEAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.STR:
                    accountVersion = new STRAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                case CurrencyType.FBC:
                    accountVersion = new FBCAccountVersion(userID, accountID,
                                amount, balance, locked, @in, @out, depositCode, modifyType);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return accountVersion;
        }
    }
}
