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
    public class OutboundTransferQuery : AbstractQuery, IOutsideTransferQuery
    {  
        public OutboundTransferModel GetOutsideTransferById(int outsideTransferId)
        {
            Check.Argument.IsNotNegativeOrZero(outsideTransferId, "outsideTransferId");

            return this.Context.Sql(getOutboundTransferById_Sql)
                               .Parameter("@id", outsideTransferId)
                               .QuerySingle<OutboundTransferModel>();
        }

        public OutboundTransferModel GetOutsideTransferBySequenceNo(string seqNo, TransactionState txstate)
        {
            Check.Argument.IsNotEmpty(seqNo, "seqNo");

            return this.Context.Sql(getOutboundTransferBySequenceNo_Sql)
                               .Parameter("@seqNo", seqNo)
                               .Parameter("@state", txstate)
                               .QuerySingle<OutboundTransferModel>();
        }
        private readonly string getOutboundTransferById_Sql =
                         @"SELECT   ID,FromUserID,SourceAmount,SequenceNo,CreateAt,Description,TransferNo,TargetCurrency,TargetAmount,Payway
                                    FROM   " + Config.Table_Prefix + @"outboundtransfertransaction 
                                   WHERE   ID=@id";

        private readonly string getOutboundTransferBySequenceNo_Sql =
                              @"SELECT   ID,FromUserID,SourceAmount,`SequenceNo`,CreateAt,Destination,`Description`,TransferNo,TargetCurrency,TargetAmount,Payway
                                    FROM   " + Config.Table_Prefix + @"outboundtransfertransaction 
                                   WHERE   `SequenceNo`=@seqNo AND `State`=@state";
    }
}
