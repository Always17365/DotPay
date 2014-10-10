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
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
