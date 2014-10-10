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
                
                default:
                    throw new NotImplementedException();
            }

            return log;
        }
    }
}
