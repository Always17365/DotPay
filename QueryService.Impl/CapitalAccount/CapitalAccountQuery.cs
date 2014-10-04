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
    public class CapitalAccountQuery : AbstractQuery, ICapitalAccountQuery
    {
        public int CountCapitalAccountBySearch(string name)
        {
            var paramters = new object[] { name.NullSafe() };

            return this.Context.Sql(capitalAccountCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.CapitalAccountListModel> GetCapitalAccountBySearch(string name, int page, int pageCount)
        {
            var paramters = new object[] { name.NullSafe(), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(capitalAccount_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<CapitalAccountListModel>();

            return users;
        }
       
        #region SQL
        private readonly string capitalAccountCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"capitalaccount
                                   WHERE   (@0='' OR OwnerName LIKE concat(@0,'%'))  ";

        private readonly string capitalAccount_Sql =
                                @"SELECT   ID,Bank,OwnerName,BankAccount,IsEnable,CreateAt,CreateBy 
                                    FROM   " + Config.Table_Prefix + @"capitalaccount
                                   WHERE   (@0='' OR OwnerName LIKE concat(@0,'%')) 
                                   LIMIT    @1,@2";
      
        #endregion
    }
}
