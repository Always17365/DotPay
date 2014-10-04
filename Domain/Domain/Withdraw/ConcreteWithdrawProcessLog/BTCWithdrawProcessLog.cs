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
    public class BTCWithdrawProcessLog : OperateLog
    {
        #region ctor
        protected BTCWithdrawProcessLog() { }

        public BTCWithdrawProcessLog(int withdrawID, int processorID, string memo) : base(withdrawID, string.Empty, memo, processorID, string.Empty) { }
        #endregion
    }
}
