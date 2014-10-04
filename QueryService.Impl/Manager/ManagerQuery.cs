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
    public class ManagerQuery : AbstractQuery, IManagerQuery
    {
        public int GetManagerCountBySearch(string email)
        {
            var paramters = new object[] { email.NullSafe(), (int)ManagerType.CustomerService, };

            return this.Context.Sql(managerCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<ManagerModel> GetManagersBySearch(string email, int page, int pageCount)
        {
            var paramters = new object[] { email.NullSafe(), (int)ManagerType.CustomerService, (page - 1) * pageCount, pageCount };

            var managers = this.Context.Sql(managers_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ManagerModel>();

            return managers;
        }
        /********************************************************************************************/

        public int GetCustomerServiceCountBySearch(string email)
        {
            var paramters = new object[] {email.NullSafe(), (int)ManagerType.CustomerService};
            return this.Context.Sql(customerServicesCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<ManagerModel> GetCustomerServicesBySearch(string email, int page, int pageCount)
        {
            var paramters = new object[] { email.NullSafe(), (int)ManagerType.CustomerService, (page - 1) * pageCount, pageCount };

            var managers = this.Context.Sql(customerServices_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ManagerModel>();

            return managers;
        }

        public IEnumerable<ManagerModel> GetManagersByUserID(int UserID)
        {
            Check.Argument.IsNotNegativeOrZero(UserID, "UserID");
            var paramters = new object[] { UserID };
            var managers = this.Context.Sql(getManagersByUserID_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<ManagerModel>();

            return managers;
        }

        
        /*********************************************************************************************/

        public bool ExistManager(int userID, ManagerType managerType)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var paramters = new object[] { userID, (int)managerType };

            var existCount = this.Context.Sql(existManager_Sql)
                                 .Parameters(paramters)
                                 .QuerySingle<int>();

            return existCount > 0;
        }

        private readonly string managerCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"Manager t1 " +
                                           " INNER JOIN " + Config.Table_Prefix + @"User t2 ON t1.UserID=t2.ID 
                                   WHERE   (@0='' OR t2.Email LIKE concat(@0,'%'))
                                     AND    Type!=@1";

        private readonly string managers_Sql =
                                @"SELECT   t1.ID,t1.UserID,t1.Type,t2.NickName,t2.Email,t2.VipLevel,t2.Mobile,t1.CreateAt
                                    FROM   " + Config.Table_Prefix + @"Manager t1 
                                            INNER JOIN " + Config.Table_Prefix + @"User t2 ON t1.UserID=t2.ID 
                                   WHERE    (@0='' OR t2.Email LIKE concat(@0,'%'))
                                     AND    Type!=@1
                                   LIMIT    @2,@3";

        private readonly string customerServicesCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"Manager t1 " +
                                           " INNER JOIN " + Config.Table_Prefix + @"User t2 ON t1.UserID=t2.ID 
                                   WHERE   (@0='' OR t2.Email LIKE concat(@0,'%'))
                                     AND    Type=@1";

        private readonly string customerServices_Sql =
                                @"SELECT   t1.ID,t1.UserID,t1.Type,t2.NickName,t2.Email,t2.VipLevel,t2.Mobile,t1.CreateAt
                                    FROM   " + Config.Table_Prefix + @"Manager t1 
                                            INNER JOIN " + Config.Table_Prefix + @"User t2 ON t1.UserID=t2.ID 
                                   WHERE    (@0='' OR t2.Email LIKE concat(@0,'%'))
                                     AND    (Type=@1)
                                   LIMIT    @2,@3";
        
        private readonly string existManager_Sql =
                                @"SELECT   COUNT(*)
                                    FROM   " + Config.Table_Prefix + @"Manager t1 
                                   WHERE    t1.UserID=@0 AND t1.Type=@1";

        private readonly string getManagersByUserID_Sql =
                                @"SELECT   UserID,Type
                                    FROM   " + Config.Table_Prefix + @"Manager
                                   WHERE    (UserID=@0)";
    }
}
