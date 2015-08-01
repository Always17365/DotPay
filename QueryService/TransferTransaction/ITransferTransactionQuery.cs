﻿
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
        /***************************************/
        int GetTransferTransactionCountBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay);
        IEnumerable<TransferTransaction> GetTransferTransactionBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay, string orderBy, int page, int pageCount);
        /***************************************/
        TransferTransaction GetTransferTransactionByRippleTxid(string txid, PayWay payWay);
        IEnumerable<TransferTransaction> GetLastTwentyTransferTransaction();
        IEnumerable<TaobaoRippleDeposit> GetLastThirtyTaobaoDeposit();
    }
}