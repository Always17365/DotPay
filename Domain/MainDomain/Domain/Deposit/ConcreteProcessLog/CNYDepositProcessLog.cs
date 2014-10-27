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
    public class CNYDepositProcessLog : OperateLog
    {
        #region ctor
        protected CNYDepositProcessLog() { }

        public CNYDepositProcessLog(int depositID, string depositUniqueID, int processorID, string memo) : base(depositID, depositUniqueID, memo, processorID, string.Empty) { }
        #endregion
    }
}
