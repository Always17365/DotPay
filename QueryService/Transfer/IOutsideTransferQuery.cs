
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.ViewModel;

namespace DotPay.QueryService
{
    public interface IOutsideTransferQuery
    {
        OutsideTransferModel GetOutsideTransferById(int outsideTransferId);
        OutsideTransferModel GetOutsideTransferBySequenceNo(string seqNo, TransactionState state);

    }
}
