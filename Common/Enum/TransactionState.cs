using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    public enum TransactionState
    {
        [EnumDescription("待处理")]
        Init = 0,
        [EnumDescription("处理中")]
        Pending = 1,
        [EnumDescription("失败")]
        Fail = 2,
        [EnumDescription("成功")]
        Success = 3,
        [EnumDescription("取消")]
        Cancel = 4
    }

    public enum RippleTransactionState
    {
        Init = 0,
        Pending = 1,
        Submit,
        Fail,
        Success,
        Cancel
    }
}
