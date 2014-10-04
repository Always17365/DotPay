using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// 统计类型
    /// <remarks>如果变更，需要注意变更trade.js中对应的类型</remarks>
    /// </summary>
    public enum StatisticsType
    {
        Day = 1,
        EightHour = 2,
        OneHour = 3,
        ThirtyMinute = 4,
        FifteenMinute = 5,
        FiveMinute = 6
    }
}
