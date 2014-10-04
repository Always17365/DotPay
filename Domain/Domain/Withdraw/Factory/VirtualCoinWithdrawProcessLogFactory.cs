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
    public class VirtualCoinWithdrawProcessLogFactory
    {
        public static OperateLog CreateLog(CurrencyType currencyType, int withdrawID, int processorID, string memo)
        {
            OperateLog log;


            switch (currencyType)
            {
                case CurrencyType.BTC:
                    log = new BTCWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.LTC:
                    log = new LTCWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.DOGE:
                    log = new DOGEWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.IFC:
                    log = new IFCWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.NXT:
                    log = new NXTWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.STR:
                    log = new STRWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                case CurrencyType.FBC:
                    log = new FBCWithdrawProcessLog(withdrawID, processorID, memo);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return log;
        }
    }
}
