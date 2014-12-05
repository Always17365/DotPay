using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// user's account modify type
    /// </summary>
    public enum AccountModifyType
    {
        Deposit = 1,
        WithdrawCreated = 2,
        InsideTransferCompleted = 3,
        OutboundTransferCreated = 4,
        OutboundTransferFailed = 5
    }
}
