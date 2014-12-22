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

        public int GetTransferTransactionCountBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay)
        {
            var paramters = new object[] { 
                account.NullSafe(), 
                (amount.HasValue ? amount.Value : 0), 
                txid.NullSafe(),
                starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, 
                endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0,
                (int)state
            };
            return this.Context.Sql(getTransferTransactionCountBySearch_Sql.FormatWith(payWay.ToString()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.TransferTransaction> GetTransferTransactionBySearch(TransactionState state, PayWay payWay, int page, int pageCount)
        {
            var paramters = new object[] { (int)state, (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getTransferTransactionBySearch.FormatWith(payWay.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<TransferTransaction>();

            return users;
        }
        public IEnumerable<DotPay.ViewModel.TransferTransaction> SelectTransferTransactionBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay, int page, int pageCount)
        {
            var paramters = new object[] { 
                account.NullSafe(), 
                (amount.HasValue ? amount.Value : 0), 
                txid.NullSafe(),
                starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, 
                endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0,
                (int)state, 
                (page - 1) * pageCount, 
                pageCount

            };
            var users = this.Context.Sql(selectTransferTransactionBySearch_sql.FormatWith(payWay.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<TransferTransaction>();

            return users;
        }

        public TransferTransaction SelectRippleTxid(string txid, PayWay payWay)
        {
            var paramters = new object[] { txid };
            var result = this.Context.Sql(selectRippleTxid_sql.FormatWith(payWay.ToString()))
                                   .Parameters(paramters)
                                   .QuerySingle<TransferTransaction>();

            return result;
        } 
        private readonly string selectRippleTxid_sql =
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,CreateAt
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction  
                                   WHERE    TxId=@0";
        #region SQL
        private readonly string getTransferTransactionCountBySearch_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction                              
                                   WHERE   (@0='' OR Account=@0)
                                     AND   (@1=0 OR Amount=@1) 
                                     AND   (@2='' OR TxId=@2)
                                     AND   (@3=0 OR CreateAt>=@3) 
                                     AND   (@4=0 OR CreateAt<=@4) 
                                     AND   State=@5";

        private readonly string getTransferTransactionBySearch =
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,CreateAt
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction  
                                   WHERE    state=@0 
                                ORDER BY    CreateAt
                                   LIMIT    @1,@2";
        private readonly string selectTransferTransactionBySearch_sql =
                               @"SELECT    ID,TxId,TransferNo,SequenceNo,SourcePayway,Account,state,Amount,CreateAt
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction                              
                                   WHERE   (@0='' OR Account=@0)
                                     AND   (@1=0 OR Amount=@1) 
                                     AND   (@2='' OR TxId=@2)
                                     AND   (@3=0 OR CreateAt>=@3) 
                                     AND   (@4=0 OR CreateAt<=@4) 
                                     AND   State=@5
                                ORDER BY   CreateAt DESC
                                   LIMIT   @6,@7";
        #endregion
    }
}
