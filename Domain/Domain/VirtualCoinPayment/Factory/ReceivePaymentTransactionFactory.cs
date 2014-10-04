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
    public class ReceivePaymentTransactionFactory
    {
        public static ReceivePaymentTransaction CreateReceivePaymentTransaction(string txid, string receiveCoinaddress, decimal amount, CurrencyType currencyType, int userID = 0)
        {
            ReceivePaymentTransaction rtx;

            switch (currencyType)
            {
                case CurrencyType.BTC:
                    rtx = new BTCReceivePaymentTransaction(txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.LTC:
                    rtx = new LTCReceivePaymentTransaction(txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.IFC:
                    rtx = new IFCReceivePaymentTransaction(txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.NXT:
                    rtx = new NXTReceivePaymentTransaction(txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.DOGE:
                    rtx = new DOGEReceivePaymentTransaction(txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.STR:
                    rtx = new STRReceivePaymentTransaction(userID, txid, receiveCoinaddress, amount);
                    break;
                case CurrencyType.FBC:
                    rtx = new FBCReceivePaymentTransaction(userID, txid, receiveCoinaddress, amount);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return rtx;
        }
    }
}
