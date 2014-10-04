using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class VipSettingLog : OperateLog
    {
        #region ctor
        protected VipSettingLog() { }
        public VipSettingLog(int vipSettingId, int operatorID, string memo)
            : base(vipSettingId, string.Empty, memo, operatorID, string.Empty)
        { }
        #endregion
    }
}
