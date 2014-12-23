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

        public IEnumerable<DotPay.ViewModel.TransferTransaction> GetTransferTransactionBySearch(TransactionState state, PayWay payWay, int page, int pageCount)
        {
            var paramters = new object[] { (int)state, (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getTransferTransactionBySearch.FormatWith(payWay.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<TransferTransaction>();

            return users;
        }
        public IEnumerable<DotPay.ViewModel.TransferTransaction> GetTransferTransactionBySearch(string account, int? amount, string txid, DateTime? starttime, DateTime? endtime, TransactionState state, PayWay payWay, int page, int pageCount)
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
            var users = this.Context.Sql(getTransferTransactionBySearch_sql.FormatWith(payWay.ToString()))
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
            if (!Cache.TryGet<IEnumerable<TransferTransaction>>(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, out result))
            {
                lock (_locker)
                {
                    if (!Cache.TryGet<IEnumerable<TransferTransaction>>(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, out result))
                    {
                        var result1 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, PayWay.Alipay, 1, 20);
                        var result2 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Init, PayWay.Alipay, 1, 20);
                        var result3 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Pending, PayWay.Tenpay, 1, 20);
                        var result4 = IoC.Resolve<ITransferTransactionQuery>().GetTransferTransactionBySearch(TransactionState.Init, PayWay.Tenpay, 1, 20);
                        result = result1.Union<TransferTransaction>(result2).Union<TransferTransaction>(result3).Union<TransferTransaction>(result4).Take(20).OrderByDescending(q => q.CreateAt);
                        Cache.Add(CacheKey.LAST_TEN_TRANSFER_TRANSACTION, result, new TimeSpan(0, 5, 0));
                    }
                }
            }
            return result;
        }
        private readonly string getTransferTransactionByRippleTxid_sql =
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,'{0}' as PayWay,TransferNo,CreateAt,DoneAt
                                    FROM    " + Config.Table_Prefix + @"to{1}transfertransaction  
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
                                @"SELECT    ID,TxId,SequenceNo,SourcePayway,Account,Amount,state,CreateAt,DoneAt
                                    FROM    " + Config.Table_Prefix + @"to{0}transfertransaction  
                                   WHERE    state=@0 
                                ORDER BY    CreateAt
                                   LIMIT    @1,@2";
        private readonly string getTransferTransactionBySearch_sql =
                               @"SELECT    ID,TxId,TransferNo,SequenceNo,SourcePayway,Account,state,Amount,CreateAt,DoneAt
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
