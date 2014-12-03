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
    public class TransactionQuery : AbstractQuery, ITransactionQuery
    { 
        private object _locker = new object();

        public IEnumerable<TransactionRecord> GetUserTransactions(int userID, int pagesize, int page, int amountIncomeStart = -1, int amountIncomeEnd = -1,
                                  int amountOutputStart = -1, int amountOutputEnd = -1,
                                  int start_date = -1, int end_date = -1, bool includeDeposit = true,
                                  bool includeWithdraw = true, bool includeOther = true)
        {
            var result = default(IEnumerable<TransactionRecord>);
            var cacheKey = CacheKey.PROFILE_INDEX_TX_RECORD + userID;
            if (Config.Debug || !Cache.TryGet<IEnumerable<TransactionRecord>>(cacheKey, out result))
            {
                lock (_locker)
                {
                    result = this.Context.Sql(getUserTransactions_sql)
                                           .Parameter("@userID", userID)
                                           .Parameters("@createAt", DateTime.Now.AddDays(-30).ToUnixTimestamp())
                                           .QueryMany<TransactionRecord>();
                    Cache.Add(cacheKey, result, new TimeSpan(1, 0, 0));
                }
            }
           return result = result.Where(q => 
               q.Income > amountIncomeStart && q.Income < amountIncomeEnd && 
               q.Output > amountOutputStart && q.Output < amountOutputEnd && 
               q.CreateAt > start_date && q.CreateAt < end_date
               ).Skip(pagesize * (page-1)).Take(pagesize).OrderBy(q => q.CreateAt);
        }

        #region SQL 

        private readonly string getUserTransactions_sql =
                                @"SELECT    SequenceNo,DoneAt,if(CAST(PayWay AS char(1))='{0}','Withdraw','Deposit') as Memo,Amount as Revenue,0 as Expenses,Payway   
                                    FROM    " + Config.Table_Prefix + @"cnydeposit
                                   WHERE    UserID = @userID
                                     AND    CreateAt = @createAt
                                UNION ALL
                                  SELECT    SequenceNo,DoneAt,'Withdraw' as Memo,if(FromUserID=@UserID,'0',Amount) as Revenue,if(ToUserID=@UserID,'0',Amount) as Expenses,Payway  
                                    FROM    " + Config.Table_Prefix + @"cnyInsideTransferTransaction
                                   WHERE    ToUserID = @userID OR FromUserID = @userID
                                     AND    CreateAt = @createAt"; 

        
        #endregion

    }
}
