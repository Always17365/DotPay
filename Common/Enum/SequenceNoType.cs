using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.Common.Enum
{
    public enum SequenceNoType
    {
        //类型的枚举数字不要超过999
        SequenceNoTypeBase = 100,
        DepositTransaction = SequenceNoTypeBase + 1,
        TransferTransaction = SequenceNoTypeBase + 2,
        SequenceNoTypeMax = 999
    }
}
