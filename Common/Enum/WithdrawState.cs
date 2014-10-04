using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// withdraw state
    /// </summary>
    public enum WithdrawState
    {
        WaitVerify = 1,
        WaitSubmit = 2,
        Processing = 3,
        Complete = 4,
        Fail = 5,
        Cancel = 6
    }

}
