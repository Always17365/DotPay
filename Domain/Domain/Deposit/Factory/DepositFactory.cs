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
    public class VirtualCoinDepositFactory
    {
        public static VirtualCoinDeposit CreateDeposit(int userID, int accountID, string txid, CurrencyType currencyType, decimal amount, decimal fee, string memo, DepositType depositType, string receivePaymentTxUniqueID = "")
        {
            VirtualCoinDeposit deposit;

            switch (currencyType)
            {
                case CurrencyType.BTC:
                    deposit = new BTCDeposit(userID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                case CurrencyType.LTC:
                    deposit = new LTCDeposit(userID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                case CurrencyType.DOGE:
                    deposit = new DOGEDeposit(userID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                case CurrencyType.IFC:
                    deposit = new IFCDeposit(userID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                case CurrencyType.NXT:
                    deposit = new NXTDeposit(userID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                case CurrencyType.STR:
                    deposit = new STRDeposit(userID, receivePaymentTxUniqueID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;  
                case CurrencyType.FBC:
                    deposit = new FBCDeposit(userID, receivePaymentTxUniqueID, accountID, txid, amount, fee, memo, DepositType.Automatic);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
