using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// fund source state
    /// </summary>
    public enum FundSourceState
    {
        WaitVerify = 1,
        Verify = 2,
        Processing = 3,
        Complete = 4,
        Delete = 5
    }
}
