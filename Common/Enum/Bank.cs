using FC.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    /// <summary>
    /// bank of china
    /// </summary>
    public enum Bank
    {
        /// <summary>
        /// 中国工商银行
        /// </summary>
        [EnumDescription("中国工商银行")]
        ICBC = 1,
        /// <summary>
        /// 中国银行
        /// </summary>
        [EnumDescription("中国银行")]
        BOC = 2,
        /// <summary>
        /// 中国建设银行
        /// </summary>
        [EnumDescription("中国建设银行")]
        CCB = 3,
        /// <summary>
        /// 中国农业银行
        /// </summary>
        [EnumDescription("中国农业银行")]
        ABC = 4,
        /// <summary>
        /// 中国民生银行
        /// </summary>
        [EnumDescription("中国民生银行")]
        CMBC = 5,
        /// <summary>
        /// 中国招商银行
        /// </summary>
        [EnumDescription("中国招商银行")]
        CMB = 6,
        /// <summary>
        /// 中国交通银行
        /// </summary>
        [EnumDescription("中国交通银行")]
        BCM = 7,
        /// <summary>
        /// 中国光大银行
        /// </summary>
        [EnumDescription("中国光大银行")]
        CEB = 8,
        /// <summary>
        /// 上海浦发银行
        /// </summary>
        [EnumDescription("上海浦发银行")]
        SPDB = 9,
        /// <summary>
        /// 广东发展银行(广发)
        /// </summary>
        [EnumDescription("广东发展银行")]
        GDB = 10,
        /// <summary>
        /// 平安银行
        /// </summary>
        [EnumDescription("平安银行")]
        PAB = 11,
        /// <summary>
        /// 华夏银行
        /// </summary>
        [EnumDescription("华夏银行")]
        HXB = 12,
        /// <summary>
        /// 中信银行
        /// </summary>
        [EnumDescription("中信银行")]
        ECITIC = 13,
        /// <summary>
        /// 兴业银行
        /// </summary>
        [EnumDescription("兴业银行")]
        CIB = 14,
        /// <summary>
        /// 中国邮政
        /// </summary>
        [EnumDescription("中国邮政")]
        PSBC = 15
    }
}
