using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class CurrencyLog : OperateLog
    {
        #region ctor
        protected CurrencyLog() { }

        public CurrencyLog(int currencyID, int processorID, string memo) : base(currencyID, string.Empty, memo, processorID, string.Empty) { }
        #endregion
    }
}
