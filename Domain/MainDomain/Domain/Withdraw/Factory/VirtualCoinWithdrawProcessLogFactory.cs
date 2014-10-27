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
    public class VirtualCoinWithdrawProcessLogFactory
    {
        public static OperateLog CreateLog(CurrencyType currencyType, int withdrawID, int processorID, string memo)
        {
            OperateLog log;


            switch (currencyType)
            { 
                default:
                    throw new NotImplementedException();
            }

            return log;
        }
    }
}
