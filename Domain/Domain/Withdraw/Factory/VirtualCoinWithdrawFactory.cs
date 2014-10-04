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
    public class VirtualCoinWithdrawFactory
    {
        public static VirtualCoinWithdraw CreateWithdraw(CurrencyType currencyType, int userID, int accountID, decimal amount, string receiveAddress)
        {
            VirtualCoinWithdraw withdraw;
             
            switch (currencyType)
            {
                case CurrencyType.BTC:
                    withdraw = new BTCWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.LTC:
                    withdraw = new LTCWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.DOGE:
                    withdraw = new DOGEWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.IFC:
                    withdraw = new IFCWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.NXT:
                    withdraw = new NXTWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.STR:
                    withdraw = new STRWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                case CurrencyType.FBC:
                    withdraw = new FBCWithdraw(userID, accountID, amount, receiveAddress);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return withdraw;
        }
    }
}
