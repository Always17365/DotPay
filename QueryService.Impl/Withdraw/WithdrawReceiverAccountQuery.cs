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
    public class WithdrawReceiverAccountQuery : AbstractQuery, IWithdrawReceiverAccountQuery
    {
        public IEnumerable<WithdrawReceiverAccountModel> GetAvailableReceiveAccountByUserID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            return this.Context.Sql(getAvailableReceiveAccountByUserID_Sql)
                               .Parameter("@userID", userID)
                               .Parameter("@valid", ValidState.Valid)
                               .QueryMany<WithdrawReceiverAccountModel>();
        }

        public WithdrawReceiverAccountModel GetLastAvailableReceiveAccountByUserID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            return this.Context.Sql(getLastAvailableReceiveAccountByUserID_Sql)
                                   .Parameter("@userID", userID)
                                   .Parameter("@invalid", ValidState.Invalid)
                                   .QuerySingle<WithdrawReceiverAccountModel>();
        }

        #region SQL
        private readonly string getAvailableReceiveAccountByUserID_Sql =
                                @"SELECT   t1.ID, t1.BankAccount,t1.Bank
                                    FROM   " + Config.Table_Prefix + @"withdrawreceiverbankaccount t1
                                   WHERE   t1.UserID = @userID AND Valid=@valid";

        private readonly string getLastAvailableReceiveAccountByUserID_Sql =
                                @"SELECT   t1.ID, t1.BankAccount,t1.Bank 
                                    FROM   " + Config.Table_Prefix + @"withdrawreceiverbankaccount t1
                                   WHERE   t1.UserID = @userID AND Valid<>@invalid
                                   ORDER   BY t1.MarkAt DESC
                                   LIMIT   1";
        #endregion

    }
}
