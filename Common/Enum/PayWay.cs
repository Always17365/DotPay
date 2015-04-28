using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.Common.Enum
{
    public enum Payway
    {
        PaywayBase = 10,
        Bank = PaywayBase + 1,
        Ripple = PaywayBase + 2,
        Alipay = PaywayBase + 3,
        Tenpay = PaywayBase + 4,
        Dotpay = PaywayBase + 5,
        EasyPay = PaywayBase + 6,
        PaywayMax = 99
    }
}
