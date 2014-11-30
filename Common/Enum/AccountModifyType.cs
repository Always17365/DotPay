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
        Withdraw = 2,
        InsideTransfer = 3,
        OutboundTransfer=4
    }
}
