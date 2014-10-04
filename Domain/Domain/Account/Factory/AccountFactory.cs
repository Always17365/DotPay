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
                case CurrencyType.BTC:
                    account = new BTCAccount(userID);
                    break;
                case CurrencyType.LTC:
                    account = new LTCAccount(userID);
                    break;
                case CurrencyType.IFC:
                    account = new IFCAccount(userID);
                    break;
                case CurrencyType.NXT:
                    account = new NXTAccount(userID);
                    break;
                case CurrencyType.DOGE:
                    account = new DOGEAccount(userID);
                    break;
                case CurrencyType.STR:
                    account = new STRAccount(userID);
                    break;
                case CurrencyType.FBC:
                    account = new FBCAccount(userID);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return account;
        }
    }
}
