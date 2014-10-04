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
    public class DepositProcessLogFactory
    {
        public static OperateLog CreateLog(CurrencyType currency, int depositID, string depositUniqueID, int operatorID, string memo)
        {
            OperateLog deposit;

            switch (currency)
            {
                case CurrencyType.CNY:
                    deposit = new CNYDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.BTC:
                    deposit = new BTCDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.LTC:
                    deposit = new LTCDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.DOGE:
                    deposit = new DOGEDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.IFC:
                    deposit = new IFCDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.NXT:
                    deposit = new NXTDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.STR:
                    deposit = new STRDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                case CurrencyType.FBC:
                    deposit = new FBCDepositProcessLog(depositID, depositUniqueID, operatorID, memo);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
