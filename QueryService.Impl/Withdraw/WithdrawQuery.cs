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
    public class WithdrawQuery : AbstractQuery, IWithdrawQuery
    {
        public int CountWithdrawBySearch(int? amount, string transferId, WithdrawState state)
        {
            var paramters = new object[] { amount.HasValue ? amount.Value : 0, transferId.NullSafe(), (int)state };

            return this.Context.Sql(withdrawCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.WithdrawListModel> GetWithdrawBySearch(int? amount, string transferId, WithdrawState state, int page, int pageCount)
        {
            var paramters = new object[] { amount.HasValue ? amount.Value : 0, transferId.NullSafe(), (int)state, (page - 1) * pageCount, pageCount };

            var result = this.Context.Sql(withdraw_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<WithdrawListModel>();
            return result;

        }
        public IEnumerable<DotPay.ViewModel.Bankoutlets> GetBank(int cityid)
        {

            Check.Argument.IsNotNegativeOrZero(cityid, "cityid");

            var paramters = new object[] { cityid };

            var result = this.Context.Sql(getBank_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<Bankoutlets>();
            return result;
        }

        public int CountVirtualCoinWithdrawBySearch(int?userID, CurrencyType currencyType, VirtualCoinTxState state)
        {
            var paramters = new object[] { userID.HasValue ? userID.Value : 0,  (int)state };

            return this.Context.Sql(getVirtualCoinWithdrawCount_Sql.FormatWith(currencyType.ToString()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.VirtualCoinWithdrawListModel> GetVirtualCoinWithdrawBySearch(int? userID, CurrencyType currencyType, VirtualCoinTxState state, int page, int pageCount)
        {

            var paramters = new object[] { userID.HasValue ? userID.Value : 0, (int)state, (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getVirtualCoinWithdraw_Sql.FormatWith(currencyType.ToString()))
                                   .Parameters(paramters)
                                   .QueryMany<VirtualCoinWithdrawListModel>();
            return users;

        }

        /****************************************************************************/
        public IEnumerable<DotPay.ViewModel.WithdrawListModel> GetMyCNYWithdraw(int userID, int start, int limit)
        {
            var result = this.Context.Sql(getLastTenCNYWithdrawByUserID_Sql)
                                   .Parameter("@userID", userID)
                                   .Parameter("@start", start)
                                   .Parameter("@limit", limit)
                                   .QueryMany<WithdrawListModel>();
            result.ForEach(r =>
            {
                r.PayWay = r.Bank == default(Bank) ? PayWay.ABC : PayWay.BCM;
            });

            return result;
        }

        public int CountMyCoinWithdraw_Sql(int userID, CurrencyType currency)
        {
            return this.Context.Sql(countMyCoinWithdraw_Sql.FormatWith(currency.ToString()))
                               .Parameter("@userID", userID)
                               .QuerySingle<int>();
        }
        public IEnumerable<DotPay.ViewModel.WithdrawListModel> GetMyVirtualCoinWithdraw(int userID, CurrencyType currency, int start, int limit)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var paramters = new object[] { userID };

            var result = this.Context.Sql(getLastTenVirtualCoinWithdrawByUserID_Sql.FormatWith(currency.ToString()))
                                   .Parameter("@userID", userID)
                                   .Parameter("@start", start)
                                   .Parameter("@limit", limit)
                                   .QueryMany<WithdrawListModel>();

            return result;
        }

        /****************************************************************************/

        #region SQL
        private readonly string getVirtualCoinWithdrawCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"{0}withdraw
                                   WHERE   (@0=0 OR UserID = @0)
                                     AND   (State = @1)";

        private readonly string getVirtualCoinWithdraw_Sql =
                                @"SELECT   t1.ID,t1.UserID,t1.UniqueID,t1.TxID,t1.ReceiveAddress,t2.Email,t1.Amount,t1.TxFee,t1.CreateAt,t1.Memo 
                                    FROM   " + Config.Table_Prefix + @"{0}withdraw t1  LEFT JOIN  "
                                             + Config.Table_Prefix + @"user t2 on t1.UserID = t2.ID
                                   WHERE   (@0=0 OR t1.UserID = @0)
                                     AND   (State = @1)
                                   ORDER   BY  CreateAt DESC            
                                   LIMIT   @2,@3";

        /****************************************************************************/

        private readonly string processingWithdrawCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"{0}receivepaymenttransaction
                                   WHERE   (State = @0)";

        private readonly string processingWithdraw_Sql =
                                @"SELECT   ID,TxID,Amount,Address,Confirmation,CreateAt 
                                    FROM   " + Config.Table_Prefix + @"{0}receivepaymenttransaction
                                   WHERE   (State = @0)
                                   LIMIT    @1,@2";


        private readonly string getProvince_Sql =
                                @"SELECT   ID ,NAME
                                    FROM   " + Config.Table_Prefix + @"province";

        private readonly string getCity_Sql =
                                @"SELECT   ID ,NAME
                                    FROM   " + Config.Table_Prefix + @"city
                                   WHERE   (fatherid = @0)";

        private readonly string getBank_Sql =
                                @"SELECT   ID,Bank ,Name
                                    FROM   " + Config.Table_Prefix + @"bankoutlets
                                   WHERE   (IsDelete = 0)
                                     AND   (CityID = @0)";

        private readonly string withdrawCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"cnywithdraw
                                   WHERE   (@0=0 OR amount = @0)
                                     AND   (@1='' OR transferNo = @1)
                                     AND   (State = @2)";



        private readonly string withdraw_Sql =
                                @"SELECT    a.id,a.UserID,b.bank,b.ReceiverName,b.BankAccount,a.Amount,a.Fee,a.CreateAt
                                    FROM   " + Config.Table_Prefix + @"cnywithdraw  a LEFT JOIN 
                                           " + Config.Table_Prefix + @"withdrawreceiverbankaccount b 
                                           on a.ReceiverBankAccountID=b.ID
                                   WHERE   (@0=0 OR a.amount = @0)
                                     AND   (@1='' OR a.transferNo = @1)
                                     AND   (State = @2)
                                   LIMIT    @3,@4";


        private readonly string getLastTenCNYWithdrawByUserID_Sql =
                                @"(SELECT     t1.ID,t1.UserID,t2.bank,t2.BankAccount,t1.Amount,t1.Fee,t1.CreateAt,t1.TransferNo ,t1.State as State,0 as DepositCodeIsUsed
                                    FROM      " + Config.Table_Prefix + @"cnywithdraw t1 left join  " + Config.Table_Prefix + @"withdrawreceiverbankaccount t2  
                                    ON        t1.ReceiverBankAccountID=t2.ID
                                    WHERE     t1.UserID = @userID)
                                    UNION     ALL
                                  (SELECT     ID,CreateBy AS UserID,0 AS bank,'' AS BankAccount,Amount,0 AS Fee,CreateAt,CONCAT(`Code`,'-',`password`) AS TransferNo,0 as State,IsUsed as DepositCodeIsUsed
                                     FROM     " + Config.Table_Prefix + @"cnydepositcode
                                    WHERE     CreateBy = @userID)
                                    ORDER     BY CreateAt DESC
                                    LIMIT     @start,@limit";

        private readonly string countMyCoinWithdraw_Sql =
                                @"SELECT COUNT(*) FROM (
                                   (SELECT    ID
                                     FROM    " + Config.Table_Prefix + @"{0}withdraw
                                    WHERE    UserID = @userID)
                                   UNION     ALL
                                  (SELECT    ID
                                    FROM     " + Config.Table_Prefix + @"{0}depositcode
                                    WHERE    CreateBy = @userID)
                                  ) as AllWithdraw";

        private readonly string getLastTenVirtualCoinWithdrawByUserID_Sql =
                                @"(SELECT    ID,UserID," + PayWay.VirutalTransfer.ToString("D") + @" AS PayWay, ReceiveAddress,Amount,Fee,CreateAt,State,0 as DepositCodeIsUsed
                                     FROM    " + Config.Table_Prefix + @"{0}withdraw
                                    WHERE    userid = @userID)
                                   UNION     ALL
                                  (SELECT    ID,CreateBy AS UserID," + PayWay.BCM.ToString("D") + @" AS PayWay,CONCAT(`Code`,'-',`password`) AS ReceiveAddress,Amount,0 AS Fee,CreateAt,0 as State,IsUsed AS DepositCodeIsUsed 
                                    FROM     " + Config.Table_Prefix + @"{0}depositcode
                                    WHERE    CreateBy = @userID)
                                    ORDER    BY CreateAt DESC
                                    LIMIT    @start,@limit";
        #endregion

    }
}
