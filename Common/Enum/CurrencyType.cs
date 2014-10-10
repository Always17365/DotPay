using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// currency type
    /// </summary>
    public enum CurrencyType
    {
        [EnumDescription("China Yuan")] 
        CNY = 1,
        [EnumDescription("XRP")]
        XRP = 2 
    }
}
