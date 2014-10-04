using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// deposit's state
    /// </summary>
    public enum VirtualCoinTxState
    {
        Initialize = 1,
        WaitConfirmation = 2,
        Complete = 3
    }
}
