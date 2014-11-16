
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.ViewModel;

namespace DotPay.QueryService
{
    public interface IInsideTransferQuery
    {
        InsideTransferModel GetInsideTransferBySequenceNo(string seqNo, CurrencyType currency);

    }
}
