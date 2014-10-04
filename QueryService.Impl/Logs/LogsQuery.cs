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
    public class LogsQuery : AbstractQuery, ILogsQuery
    {

        public int GetUserLoginCountBySearch( string email)
        {
            var paramters = new object[] { email.NullSafe() };

            return this.Context.Sql(getUserLoinCount_sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.LogsInListModel> GetUserLoginBySearch(string email, int page, int pageCount)
        {
            var paramters = new object[] { email.NullSafe(), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getUserLoinLogs_sql)
                                   .Parameters(paramters)
                                   .QueryMany<LogsInListModel>();



            return users;
        }


        public int GetLogsCountBySearch(string tableName, string email)
        {
            Check.Argument.IsNotEmpty(tableName, "tableName");
            var paramters = new object[] { email.NullSafe() };

            return this.Context.Sql(getLogsCount.FormatWith(tableName.NullSafe()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.LogsInListModel> GetLogsBySearch(string tableName, string email, int page, int pageCount)
        {
            Check.Argument.IsNotEmpty(tableName, "tableName");
            var paramters = new object[] { email.NullSafe(), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getLogs_Sql.FormatWith(tableName.NullSafe()))
                                   .Parameters(paramters)
                                   .QueryMany<LogsInListModel>();  
            return users;
        }



        public int GetUserLogCountBySearch(int userlogID, DateTime? starttime, DateTime? endtime)
        {
            Check.Argument.IsNotNegativeOrZero(userlogID, "userlogID");
            var paramters = new object[] { userlogID, starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0 };

            return this.Context.Sql(userlogsCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }
        public IEnumerable<UserLogInListModel> GetUseLogsBySearch(int userlogID, DateTime? starttime, DateTime? endtime, int page, int pageCount)
        {
            Check.Argument.IsNotNegativeOrZero(userlogID, "userlogID");
            var paramters = new object[] { userlogID, starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0, (page - 1) * pageCount, pageCount };

            return this.Context.Sql(userlogs_Sql)
                               .Parameters(paramters)
                               .QueryMany<UserLogInListModel>();
        }


        public int CountUserLoginHistory(int userID)
        {

            return this.Context.Sql(countUserLoginHistory_Sql)
                               .Parameter("@userID", userID) 
                               .QuerySingle<int>();
        }
        public IEnumerable<LogsInListModel> GetUserLoginHistory(int userID, int start, int limit)
        {
            return this.Context.Sql(getUserLoginHistory_Sql)
                               .Parameter("@userID", userID)
                               .Parameter("@start", start)
                               .Parameter("@limit", limit)
                               .QueryMany<LogsInListModel>();
        }
       
        #region SQL

        private readonly string countUserLoginHistory_Sql =
                               @"SELECT     count(*) 
                                   FROM   " + Config.Table_Prefix + @"loginlog
                                  WHERE     UserID=@userID
                               ORDER BY     LoginTime DESC";
        private readonly string getUserLoginHistory_Sql =
                               @"SELECT   ID,LoginTime,IP 
                                   FROM   " + Config.Table_Prefix + @"loginlog
                                  WHERE   UserID=@userID
                               ORDER BY   LoginTime DESC
                                  LIMIT   @start,@limit";

        private readonly string userlogsCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"depositauthorizationuselog 
                                   WHERE   (DomainID = @0)  
                                     AND   (@1=0 OR OperateTime>=@1)
                                     AND   (@2=0 OR OperateTime<=@2)";

        private readonly string userlogs_Sql =
                                @"SELECT   ID, OperateTime,Memo
                                    FROM   " + Config.Table_Prefix + @"depositauthorizationuselog 
                                   WHERE   (DomainID = @0)  
                                     AND   (@1=0 OR OperateTime>=@1)
                                     AND   (@2=0 OR OperateTime<=@2)
                                   LIMIT    @3,@4";

        private readonly string getUserLoinCount_sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + "loginlog t1  LEFT JOIN "
                                             + Config.Table_Prefix + @"user t2 on t1.UserID =t2.ID                          
                                   WHERE   (@0='' OR  t2.Email LIKE concat(@0,'%'))  ";
        private readonly string getUserLoinLogs_sql =
                                @"SELECT   t1.ID, t2.Email, t1.LoginTime,t1.IP   
                                    FROM   " + Config.Table_Prefix + "loginlog t1  LEFT JOIN "
                                             + Config.Table_Prefix + @"user t2 on t1.UserID =t2.ID          
                                   WHERE   (@0='' OR  t2.Email LIKE concat(@0,'%')) 
                                   LIMIT    @1,@2";


        private readonly string getLogsCount =
                                 @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + "{0} t1 LEFT JOIN "
                                              + Config.Table_Prefix + @"user t2 on t1.DomainID =t2.ID                          
                                   WHERE   (@0='' OR  t2.Email LIKE concat(@0,'%'))  ";

        private readonly string getLogs_Sql =
                                @"SELECT   t1.ID,t2.Email as DomainEmail,t3.Email as OperatorEmail,OperateTime,Memo  
                                    FROM   " + Config.Table_Prefix + "{0} t1  LEFT JOIN "
                                             + Config.Table_Prefix + @"user t2 on t1.DomainID =t2.ID  LEFT JOIN "
                                             + Config.Table_Prefix + @"user t3 on t1.OperatorID = t3.ID                        
                                   WHERE   (@0='' OR  t2.Email LIKE concat(@0,'%')) 
                                   LIMIT    @1,@2";
      
        #endregion
    }
}
