using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.Common.Enum
{
    public enum TransactionType
    {
        /// <summary>存款
        /// </summary>
        DepositTransaction = 1,
        /// <summary>取款
        /// </summary>
        WithdrawTransaction = 2,
        /// <summary>转账
        /// </summary>
        TransferTransaction = 3,
        /// <summary>退款
        /// </summary>
        RefundTransaction = 4
    }
}
