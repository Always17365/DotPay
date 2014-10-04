using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    public enum OrderType
    {
        /// <summary>
        /// 买单
        /// </summary>
        [EnumDescription("BidOrder")]
        Bid = 1,
        /// <summary>
        /// 卖单
        /// </summary>
        [EnumDescription("AskOrder")]
        Ask = 2
    }
}
