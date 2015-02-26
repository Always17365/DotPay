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
        DepositTransaction,
        /// <summary>取款
        /// </summary>
        WithdrawTransaction,
        /// <summary>转账
        /// </summary>
        TransferTransaction
    }
}
