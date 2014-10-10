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
    public class DepositQuery : AbstractQuery, IDepositQuery
    {
        #region 前端用查询

        #region 人民币充值记录
        public int CountCNYDepositByUserID(int userID)
        {
            return this.Context.Sql(countCNYDepositByUserID_Sql)
                               .Parameter("@userID", userID)
                               .QuerySingle<int>();
        }

        public IEnumerable<DepositInListModel> GetCNYDepositByUserID(int userID, int start, int limit)
        {
            var deposits = this.Context.Sql(getCNYDepositByUserID_Sql)
                                   .Parameter("@userID", userID)
                                   .Parameter("@start", start)
                                   .Parameter("@limit", limit)
                                   .QueryMany<DepositInListModel>();

            return deposits;
        }
        #endregion

        #region 虚拟币充值记录
        public int CountVirtualCoinDepositByUserID(int userID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            return this.Context.Sql(countVirtualCoinDepositByUserID_Sql.FormatWith(currency.ToString()))
                              .Parameter("@userID", userID)
                              .QuerySingle<int>();
        }


        public IEnumerable<DepositInListModel> GetVirtualCoinDepositByUserID(int userID, CurrencyType currency, int start, int limit)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var deposits = this.Context.Sql(getVirtualCoinDepositByUserID_Sql.FormatWith(currency.ToString()))
                                   .Parameter("@userID", userID)
                                   .Parameter("@start", start)
                                   .Parameter("@limit", limit)
                                   .QueryMany<DepositInListModel>();

            return deposits;
        }
        #endregion

        #endregion
        public int CountDepositBySearch(int? amountsearch, string email)
        {
            var paramters = new object[] { (amountsearch.HasValue ? amountsearch.Value : 0), email.NullSafe() };

            return this.Context.Sql(depositCNY_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.DepositInListModel> GetDepositBySearch(int? amountsearch, string email, int page, int pageCount)
        {
            var paramters = new object[] { amountsearch.HasValue ? amountsearch.Value : 0, email.NullSafe(), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(deposit_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<DepositInListModel>();

            return users;
        }

        public int CountVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state)
        {
            var paramters = new object[] { (userID.HasValue ? userID.Value : 0), starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0, (int)state };

            return this.Context.Sql(countdepositVirtualCurrency_Sql.FormatWith(currency.ToString()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.VirtualCurrencyDepositInListModel> GetVirtualCurrencyDepositBySearch(int? userID, DateTime? starttime, DateTime? endtime, CurrencyType currency, DepositState state, int page, int pageCount)
        {
            var paramters = new object[] { userID.HasValue ? userID.Value : 0, starttime.HasValue ? starttime.Value.ToUnixTimestamp() : 0, endtime.HasValue ? endtime.Value.ToUnixTimestamp() : 0, (int)state, (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(depositVirtualCurrency_Sql.FormatWith(currency.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<VirtualCurrencyDepositInListModel>();

            return users;
        }

        public int QueryCompleteCNYDeposit(int depositID, int userID)
        {
            var paramters = new object[] { depositID, userID };

            return this.Context.Sql(QueryCompleteCNYDeposit_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }
        /***********************************************************/
        public int CountReceivePayMentTransactionBySearch(CurrencyType currency, DepositState state)
        {
            var paramters = new object[] {  (int)state };

            return this.Context.Sql(countReceivePayMentTransactionBySearch_Sql.FormatWith(currency.ToString()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        #region SQL
        #region admin web use
        private readonly string countReceivePayMentTransactionBySearch_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"{0}receivepaymenttransaction                                  
                                   WHERE   (State=@0) ";

        private readonly string getSTRReceivePayMentTransactionBySearch_Sql =
                                @"SELECT   ID, UserID, TxID, Amount, Address, CreateAt
                                    FROM   " + Config.Table_Prefix + @"{0}receivepaymenttransaction
                                   WHERE   (State=@0) 
                                ORDER BY    CreateAt DESC 
                                   LIMIT    @1,@2";
        private readonly string getBTCReceivePayMentTransactionBySearch_Sql =
                                @"SELECT   t1.ID, t2.UserID, t1.TxID, t1.Amount, t1.Address, t1.CreateAt
                                    FROM   " + Config.Table_Prefix + @"{0}receivepaymenttransaction  t1
                               LEFT JOIN   " + Config.Table_Prefix + @"{0}deposit t2 on t1.TxID=t2.TxID
                                   WHERE   (t1.State=@0) 
                                ORDER BY    t1.CreateAt DESC 
                                   LIMIT    @1,@2";



        private readonly string depositCNY_Sql =
                               @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"cnydeposit a 
                                                 INNER JOIN " + Config.Table_Prefix + @"User b ON a.UserID=b.Id 
                                   WHERE   (@0=0 OR a.Amount=@0)  
                                     AND   (@1='' OR b.Email LIKE concat(@1,'%'))  ";

        private readonly string deposit_Sql =
                                @"SELECT   a.ID,a.UserID,b.Email,a.State,a.Amount ,a.CreateAt,a.Memo
                                    FROM   " + Config.Table_Prefix + @"cnydeposit a 
                                            INNER JOIN " + Config.Table_Prefix + @"User b ON a.UserID=b.Id 
                                   WHERE   (@0=0 OR a.Amount=@0) 
                                     AND   (@1='' OR b.Email LIKE concat(@1,'%'))
                                ORDER BY    a.CreateAt DESC 
                                   LIMIT    @2,@3";

        private readonly string countdepositVirtualCurrency_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"{0}Deposit
                                   WHERE   (@0=0 OR UserID=@0) 
                                     AND   (@1=0 OR CreateAt>=@1) 
                                     AND   (@2=0 OR CreateAt<=@2)
                                     AND   (State=@3) ";

        private readonly string depositVirtualCurrency_Sql =
                                @"SELECT   ID ,UniqueID,UserID ,AccountID ,Amount ,TxID ,CreateAt ,Memo
                                    FROM   " + Config.Table_Prefix + @"{0}Deposit
                                   WHERE   (@0=0 OR UserID=@0) 
                                     AND   (@1=0 OR CreateAt>=@1) 
                                     AND   (@2=0 OR CreateAt<=@2) 
                                     AND   (State=@3) 
                                ORDER BY    CreateAt DESC
                                   LIMIT    @4,@5";

        private readonly string QueryCompleteCNYDeposit_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"cnydeposit 
                                   WHERE   (ID=@0)  
                                     AND   (UserID=@1)  ";
        #endregion

        #region front web use
        private readonly string countCNYDepositByUserID_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"cnydeposit 
                                   WHERE    UserID=@userID";
        private readonly string getCNYDepositByUserID_Sql =
                                @"SELECT   ID,UserID,PayWay,State,Amount,Fee,CreateAt,Memo
                                    FROM   " + Config.Table_Prefix + @"cnydeposit
                                   WHERE   UserID=@userID
                                   ORDER   BY CreateAt DESC
                                   LIMIT   @start,@limit";

        private readonly string countVirtualCoinDepositByUserID_Sql =
                              @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"{0}deposit 
                                   WHERE    UserID=@userID";
        private readonly string getVirtualCoinDepositByUserID_Sql =
                                @"SELECT   ID,UserID,Amount,State,Fee,TxID,CreateAt,Memo
                                    FROM   " + Config.Table_Prefix + @"{0}deposit
                                   WHERE   UserID=@userID
                                   ORDER   BY CreateAt DESC
                                   LIMIT   @start,@limit";
        #endregion
        #endregion

    }
}
