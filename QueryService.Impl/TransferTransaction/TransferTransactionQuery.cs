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

        private object _locker = new object();

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

        public IEnumerable<DotPay.ViewModel.TransferTransaction> GetTransferTransactionBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay,string orderBy, int page, int pageCount)
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
            var users = this.Context.Sql(getTransferTransactionBySearch_sql.FormatWith(payWay.ToString(),orderBy))
                                   .Parameters(paramters)
                                   .QueryMany<TransferTransaction>();

            return users;
        }
        public TransferTransaction GetTransferTransactionByRippleTxid(string txid, PayWay payWay)
        {
            var paramters = new object[] { txid };
            var result = this.Context.Sql(getTransferTransactionByRippleTxid_sql.FormatWith(payWay.ToString(), payWay.ToString()))
                                   .Parameters(paramters)
                                   .QuerySingle<TransferTransaction>();

            return result;
        }
        public IEnumerable<TransferTransaction> GetLastTwentyTransferTransaction()
        {
            IEnumerable<TransferTransaction> result = default(IEnumerable<TransferTransaction>);
            if (Config.Debug || !Cache.TryGet<IEnumerable<TransferTransaction>>(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, out result))
            {
                lock (_locker)
                {
                    if (Config.Debug || !Cache.TryGet<IEnumerable<TransferTransaction>>(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, out result))
                    {
                        var result1 = this.Context.Sql(getLastTwentyTransferTransaction_sql.FormatWith(PayWay.Alipay.ToString()))
                                   .QueryMany<TransferTransaction>();
                        var result2 = this.Context.Sql(getLastTwentyTransferTransaction_sql.FormatWith(PayWay.Tenpay.ToString()))
                                   .QueryMany<TransferTransaction>();
                        result = result1.Union<TransferTransaction>(result2);
                        Cache.Add(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, result, new TimeSpan(0, 5, 0));
                    }
                }
            }
            return result;
        }
        #region SQL

        private readonly string getLastTwentyTransferTransaction_sql =
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,'{0}' as PayWay,TransferNo,CreateAt,DoneAt,Reason,RealName,Memo
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction  
                                   WHERE    state<>'"+TransactionState.Fail+@"' 
                                     AND    state<>'"+TransactionState.Cancel+@"'                                    
                                ORDER BY    CreateAt DESC
                                   LIMIT    20";

        private readonly string getTransferTransactionByRippleTxid_sql =
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,'{0}' as PayWay,TransferNo,CreateAt,DoneAt,Reason,RealName,Memo
                                    FROM    " + Config.Table_Prefix + @"to{1}transfertransaction  
                                   WHERE    TxId=@0                               
                                ORDER BY    CreateAt DESC";

        private readonly string getTransferTransactionCountBySearch_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction                              
                                   WHERE   (@0='' OR Account=@0)
                                     AND   (@1=0 OR Amount=@1) 
                                     AND   (@2='' OR TxId=@2)
                                     AND   (@3=0 OR CreateAt>=@3) 
                                     AND   (@4=0 OR CreateAt<=@4) 
                                     AND   State=@5                               
                                ORDER BY    CreateAt DESC";


        private readonly string getTransferTransactionBySearch_sql =
                               @"SELECT    ID,TxId,TransferNo,SequenceNo,SourcePayway,Account,state,Amount,CreateAt,DoneAt,Reason,RealName,Memo
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction                              
                                   WHERE   (@0='' OR Account=@0)
                                     AND   (@1=0 OR Amount=@1) 
                                     AND   (@2='' OR TxId=@2)
                                     AND   (@3=0 OR CreateAt>=@3) 
                                     AND   (@4=0 OR CreateAt<=@4) 
                                     AND   State=@5
                                ORDER BY   CreateAt {1}
                                   LIMIT   @6,@7";
        #endregion
    }
}
