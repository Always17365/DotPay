using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// deposit's state
    /// </summary>
    public enum DepositState
    {
        [EnumDescription("DepositStatusPending")]
        Pending = 1,
        [EnumDescription("DepositStatusVerified")]
        Verify = 2,
        [EnumDescription("DepositStatusCompleted")]
        Complete = 3,
        //Fail=3,
        //TimeOut=4
    }
}
