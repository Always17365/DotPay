
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ITransferTransactionQuery
    {
        int GetTransferTransactionCountBySearch(TransactionState state, PayWay payWay);
        IEnumerable<TransferTransaction> GetTransferTransactionBySearch(TransactionState state, PayWay payWay, int page, int pageCount);

    }
}
