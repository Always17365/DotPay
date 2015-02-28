using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.TaobaoMonitor
{
    internal enum RippleTransactionStatus
    {
        Init = 0,
        Pending = 1,
        Submited = 2,
        Successed = 3,
        Failed = 4
    }
}
