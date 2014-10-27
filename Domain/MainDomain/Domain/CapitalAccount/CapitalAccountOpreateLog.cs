using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using DotPay.MainDomain.Events;
using FC.Framework;

namespace DotPay.MainDomain
{
    public class CapitalAccountOpreateLog : OperateLog
    {
        #region ctor
        protected CapitalAccountOpreateLog() { }

        public CapitalAccountOpreateLog(int capitalAccountID, string uniqueID, string memo, int userID) : base(capitalAccountID, uniqueID, memo, userID, string.Empty) { }
        #endregion

    }
}
