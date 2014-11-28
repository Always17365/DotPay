using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class TransferTransactionQuery : AbstractQuery, ITransferTransactionQuery
    {

        public int GetTransferTransactionCountBySearch(TransactionState state, PayWay payWay)
        {

            return this.Context.Sql(getTransferTransactionCountBySearch_Sql.FormatWith(payWay.ToString()))
                               .Parameter("@state", (int)state)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.TransferTransaction> GetTransferTransactionBySearch(TransactionState state, PayWay payWay, int page, int pageCount)
        {
            var paramters = new object[] {(int)state, (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getTransferTransactionBySearch.FormatWith(payWay.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<TransferTransaction>();

            return users;
        }

        #region SQL
        private readonly string getTransferTransactionCountBySearch_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction                              
                                   WHERE    state=@state";

        private readonly string getTransferTransactionBySearch =
                                @"SELECT    ID,SequenceNo,Account,Amount,CreateAt
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction  
                                   WHERE    state=@0 
                                ORDER BY    CreateAt
                                   LIMIT    @1,@2";      
        #endregion
    }
}
