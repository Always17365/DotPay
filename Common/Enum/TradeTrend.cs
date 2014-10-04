using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    public enum TradeTrend
    {
        [EnumDescription("平")]
        Flat = 1,
        [EnumDescription("涨")]
        Up = 2,
        [EnumDescription("跌")]
        Down = 3
    }
}
