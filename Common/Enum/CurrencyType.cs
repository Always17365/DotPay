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
        [EnumDescription("BitCoin")]
        BTC = 2,
        [EnumDescription("LiteCoin")]
        LTC = 3,
        [EnumDescription("InfiniteCoin")]
        IFC = 4,
        [EnumDescription("NextCoin")]
        NXT = 5,
        [EnumDescription("DogeCoin")]
        DOGE = 6,
        [EnumDescription("StrellarCoin")]
        STR = 7,
        [EnumDescription("FootballCoin")]
        FBC = 8
    }
}
