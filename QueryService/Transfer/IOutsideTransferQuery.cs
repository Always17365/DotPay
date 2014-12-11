
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
        OutboundTransferModel GetOutsideTransferById(int outsideTransferId);
        OutboundTransferModel GetOutsideTransferBySequenceNo(string seqNo, TransactionState state);

    }
}
