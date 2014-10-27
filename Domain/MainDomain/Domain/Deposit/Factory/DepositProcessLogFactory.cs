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
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
