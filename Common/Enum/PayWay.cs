using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// pay way
    /// </summary>
    public enum PayWay
    {
        [EnumDescription("PayWay Alipay")]
        Alipay = 1,
        [EnumDescription("PayWay Tenpay")]
        Tenpay = 2,
        [EnumDescription("PayWay Bank")]
        BankTransfer = 3,
        [EnumDescription("PayWay Ripple")]
        Ripple = 4,
        [EnumDescription("PayWay DotPay Code")]
        DepositCode = 5,
        [EnumDescription("PayWay Virtual Coin Tranfer")]
        VirutalTransfer = 6
    }
}
