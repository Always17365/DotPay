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

        public TransactionRecordInfo GetUserTransactions(int userID, int pagesize, int page, int amountIncomeStart = -1, int amountIncomeEnd = -1,
                                  int amountOutputStart = -1, int amountOutputEnd = -1,
                                  int start_date = -1, int end_date = -1, bool includeDeposit = true,
                                  bool includeWithdraw = true, bool includeOther = true)
        {
            TransactionRecordInfo data = new TransactionRecordInfo();
            var result = default(IEnumerable<TransactionRecord>);
            var cacheKey = CacheKey.PROFILE_INDEX_TX_RECORD + userID;
            if (Config.Debug || !Cache.TryGet<IEnumerable<TransactionRecord>>(cacheKey, out result))
            {
                lock (_locker)
                {
                    result = this.Context.Sql(getUserTransactions_sql.FormatWith((int)PayWay.Ripple))
                                           .Parameter("@userID", userID)
                                           .Parameter("@createAt", DateTime.Now.AddDays(-30).ToUnixTimestamp())
                                           .QueryMany<TransactionRecord>();
                    Cache.Add(cacheKey, result, new TimeSpan(1, 0, 0));
                }
            }
            result = result.Where(q =>
              (amountIncomeStart != -1 ? q.Income > amountIncomeStart : true) &&
              (amountIncomeEnd != -1 ? q.Income < amountIncomeEnd : true) &&
              (amountOutputStart != -1 ? q.Output > amountOutputStart : true) &&
              (amountOutputEnd != -1 ? q.Output < amountOutputEnd : true) &&
              (start_date != -1 ? q.CreateAt > start_date : true) &&
              (end_date != -1 ? q.CreateAt < end_date : true)
            ); 
            data.Count = result.Count();   
            data.Data=result.Skip(pagesize * (page-1)).Take(pagesize).OrderBy(q => q.CreateAt);
            return data;
        }

        #region SQL

        private readonly string getUserTransactions_sql =
                                @"SELECT    SequenceNo,DoneAt,if(CAST(PayWay AS char(1))='{0}','Withdraw','Deposit') as Category,Amount as Income,0 as Output,Payway,CreateAt   
                                    FROM    " + Config.Table_Prefix + @"cnydeposit
                                   WHERE    UserID = @userID
                                     AND    CreateAt > @createAt
                                UNION ALL
                                  SELECT    SequenceNo,DoneAt,'Withdraw' as Category,if(FromUserID=@UserID,'0',Amount) as Income,if(ToUserID=@UserID,'0',Amount) as Output,Payway,CreateAt  
                                    FROM    " + Config.Table_Prefix + @"cnyInsideTransferTransaction
                                   WHERE    (ToUserID = @userID OR FromUserID = @userID)
                                     AND    CreateAt > @createAt
                                UNION ALL
                                  SELECT    SequenceNo,DoneAt,'Withdraw' as Category,0 as Income,SourceAmount as Output,Payway,CreateAt  
                                    FROM    " + Config.Table_Prefix + @"OutboundTransferTransaction
                                   WHERE    FromUserID = @userID
                                     AND    CreateAt > @createAt";


        #endregion

    }
}
