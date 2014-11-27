using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using FC.Framework;
using DotPay.Common;
using DotPay.ViewModel;
using FC.Framework.Utilities;

namespace DotPay.QueryService.Impl
{
    public class InsideTransferQuery : AbstractQuery, IInsideTransferQuery
    {
        public InsideTransferModel GetInsideTransferBySequenceNo(string seqNo, TransactionState txstate, CurrencyType currency)
        {
            Check.Argument.IsNotEmpty(seqNo, "seqNo");

            return this.Context.Sql(getInsideTransferBySequenceNo_Sql.FormatWith(currency.ToString()))
                               .Parameter("@seqNo", seqNo)
                               .Parameter("@state", txstate)
                               .QuerySingle<InsideTransferModel>();
        }

        private readonly string getInsideTransferBySequenceNo_Sql =
                                @"SELECT   ID,FromUserID,ToUserID,SequenceNo,CreateAt,Description,Amount,Payway,Currency
                                    FROM   " + Config.Table_Prefix + @"{0}insidetransfertransaction 
                                   WHERE   SequenceNo=@seqNo AND State=@state";


    }
}
