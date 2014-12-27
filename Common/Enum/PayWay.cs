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
        [EnumDescription("Inside")]
        Inside = 1,
        [EnumDescription("Alipay")]
        Alipay,
        [EnumDescription("Tenpay")]
        Tenpay,
        [EnumDescription("Ripple")]
        Ripple,
        /// <summary>
        /// 中国工商银行
        /// </summary>
        [EnumDescription("中国工商银行")]
        ICBC,
        /// <summary>
        /// 中国银行
        /// </summary>
        [EnumDescription("中国银行")]
        BOC,
        /// <summary>
        /// 中国建设银行
        /// </summary>
        [EnumDescription("中国建设银行")]
        CCB,
        /// <summary>
        /// 中国农业银行
        /// </summary>
        [EnumDescription("中国农业银行")]
        ABC,
        /// <summary>
        /// 中国民生银行
        /// </summary>
        [EnumDescription("中国民生银行")]
        CMBC,
        /// <summary>
        /// 中国招商银行
        /// </summary>
        [EnumDescription("中国招商银行")]
        CMB,
        /// <summary>
        /// 中国交通银行
        /// </summary>
        [EnumDescription("中国交通银行")]
        BCM,
        /// <summary>
        /// 中国光大银行
        /// </summary>
        [EnumDescription("中国光大银行")]
        CEB,
        /// <summary>
        /// 上海浦发银行
        /// </summary>
        [EnumDescription("上海浦发银行")]
        SPDB,
        /// <summary>
        /// 广东发展银行(广发)
        /// </summary>
        [EnumDescription("广东发展银行")]
        GDB,
        /// <summary>
        /// 平安银行
        /// </summary>
        [EnumDescription("平安银行")]
        PAB,
        /// <summary>
        /// 华夏银行
        /// </summary>
        [EnumDescription("华夏银行")]
        HXB,
        /// <summary>
        /// 中信银行
        /// </summary>
        [EnumDescription("中信银行")]
        ECITIC,
        /// <summary>
        /// 兴业银行
        /// </summary>
        [EnumDescription("兴业银行")]
        CIB,
        /// <summary>
        /// 中国邮政
        /// </summary>
        [EnumDescription("中国邮政")]
        PSBC,
        [EnumDescription("PayWay Virtual Coin Tranfer")]

        VirutalTransfer,

        [EnumDescription("Bank")]
        BankRippleForm = 97,
        [EnumDescription("Alipay")]
        AlipayRippleForm=98,
        [EnumDescription("Tenpay")]
        TenpayRippleForm=99
      
    }
}
