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
    public class UserQuery : AbstractQuery, IUserQuery
    {
        public IEnumerable<DotPay.ViewModel.SeeUserDepositAmounModel> SeeUserDepositAmoun(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var paramters = new object[] { userID };

            var users = this.Context.Sql(seeUserDepositAmounListModel_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<SeeUserDepositAmounModel>();

            return users;
        }
        public int CountUserBySearch(int? userID, string email, bool IsLocked)
        {
            var paramters = new object[] { (userID.HasValue ? userID.Value : 0), email.NullSafe(), Convert.ToInt32(IsLocked) };

            return this.Context.Sql(userCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.UserInListModel> GetUsersBySearch(int? userID, string email, bool IsLocked, int page, int pageCount)
        {
            var paramters = new object[] { userID.HasValue ? userID.Value : 0, email.NullSafe(), Convert.ToInt32(IsLocked), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(users_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<UserInListModel>();

            return users;
        }
        public LoginUser GetUserByID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var cacheKey = CacheKey.USER_INFO_BY_ID + userID.ToString();
            var user = Cache.Get<LoginUser>(cacheKey);

            if (user == null)
            {
                user = this.Context.Sql(getUserByID_Sql)
                                 .Parameter("@id", userID)
                                 .QuerySingle<LoginUser>();

                if (user != null)
                    Cache.Add(cacheKey, user);
            }
            return user;
        }

        public LoginUser GetUserByEmail(string email)
        {
            Check.Argument.IsNotEmpty(email, "email");

            var cacheKey = CacheKey.USER_INFO_BY_MAIL + email;
            var user = Cache.Get<LoginUser>(cacheKey);

            if (user == null)
            {
                user = this.Context.Sql(getUserByEmail_Sql)
                                   .Parameter("@email", email)
                                   .QuerySingle<LoginUser>();

                if (user != null)
                    Cache.Add(cacheKey, user, new TimeSpan(1, 0, 0));
            }
            return user;
        }

        public LoginUser GetUserByMobile(string mobile)
        {
            Check.Argument.IsNotEmpty(mobile, "mobile");

            var cacheKey = CacheKey.USER_INFO_BY_MOBILE + mobile;
            var user = Cache.Get<LoginUser>(cacheKey);

            if (user == null)
            {
                user = this.Context.Sql(getUserByMobile_Sql)
                                   .Parameter("@mobile", mobile)
                                   .QuerySingle<LoginUser>();

                if (user != null)
                    Cache.Add(cacheKey, user);
            }
            return user;
        }

        public LoginUser GetUserByOpenID(string openID, OpenAuthType authType)
        {
            Check.Argument.IsNotEmpty(openID, "openID");
            Check.Argument.IsNotNegativeOrZero((int)authType, "authType");

            var cacheKey = CacheKey.USER_INFO_BY_OPENID + openID + authType.ToString();
            var user = Cache.Get<LoginUser>(cacheKey);

            if (user == null)
            {
                user = this.Context.Sql(getUserByOpenID_Sql)
                                   .Parameter("@openID", openID)
                                   .Parameter("@authType", authType)
                                   .QuerySingle<LoginUser>();

                if (user != null)
                    Cache.Add(cacheKey, user);
            }

            return user;
        }

        public int CountUserByEmail(string email)
        {
            Check.Argument.IsNotEmpty(email, "email");

            var count = this.Context.Sql(countUserByEmail_Sql)
                                    .Parameter("@email", email)
                                    .QuerySingle<int>();

            return count;
        }

        public int CountUserByMobile(string mobile)
        {
            Check.Argument.IsNotEmpty(mobile, "mobile");

            var count = this.Context.Sql(countUserByMobile_Sql)
                                    .Parameter("@mobile", mobile)
                                    .QuerySingle<int>();

            return count;
        }

        //public UserRealNameModel GetUserRealNameAuthInfoByID(int userID)
        //{
        //    Check.Argument.IsNotNegativeOrZero(userID, "userID");

        //    var cacheKey = CacheKey.USER_REALNAME_INFO_BY_ID + userID.ToString();
        //    var realNameInfo = Cache.Get<UserRealNameModel>(cacheKey);

        //    if (realNameInfo == null)
        //    {
        //        realNameInfo = this.Context.Sql(userRealNameInfo_Sql)
        //                            .Parameter("@userID", userID)
        //                            .QuerySingle<UserRealNameModel>();

        //        if (realNameInfo != null)
        //            Cache.Add(cacheKey, realNameInfo);
        //    }

        //    if (realNameInfo == null || realNameInfo.IdNoType == default(IdNoType))
        //        return null;
        //    else
        //        return realNameInfo;
        //}

        public string GetUserGoogleAuthenticationSecretByID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var ga_secret = string.Empty;
            var results = this.Context.Sql(getGaSecret_Sql)
                                        .Parameter("@userID", userID)
                                        .QueryMany<string>();

            if (results != null && results.Count > 0)
            {
                ga_secret = results.Single();
            }

            return ga_secret;
        }

        public Tuple<string, int> GetUserSmsSecretByID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            var results = this.Context.Sql(getSmsSecret_Sql)
                                        .Parameter("@userID", userID)
                                        .QueryMany<dynamic>();

            if (results != null && results.Count > 0)
            {
                var result = results.Single();

                return new Tuple<string, int>(result.OTPSecret, result.SmsCounter);
            }

            return null;
        }

        public int GetMaxUserID()
        {
            return this.Context.Sql(getMaxUserID_Sql).QuerySingle<int>();
        }
        public int GetUserCommendCounter(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            object counterObj;
            var cacheKey = CacheKey.USER_CONMEND_COUNTER + userID;
            var counter = 0;

            if (Cache.TryGet(cacheKey, out counterObj))
            {
                counter = (int)counterObj;
            }
            else
            {
                counter = this.Context.Sql(getUserCommendCounter_Sql).Parameter("@userID", userID).QuerySingle<int>();
                Cache.Add(cacheKey, counter, new TimeSpan(1, 0, 0));
            }

            return counter;
        }
        public UserVipInfoModel GetUserVipInfo(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            UserVipInfoModel vipInfo;
            var cacheKey = CacheKey.USER_SCORE_BALANCE + userID; 

            if (Config.Debug || !Cache.TryGet(cacheKey, out vipInfo))
            {
                vipInfo = this.Context.Sql(getUserVipInfo_Sql).Parameter("@userID", userID).QuerySingle<UserVipInfoModel>();
                Cache.Add(cacheKey, vipInfo, new TimeSpan(1, 0, 0));
            }

            return vipInfo;
        }
        /***********************************************************************************/
        public int GetUsersCurrencyCountBySearch(int? userID, string email, CurrencyType currencyType)
        {
            var paramters = new object[] { (userID.HasValue ? userID.Value : 0), email.NullSafe()};

            return this.Context.Sql(getUsersCurrencyCountBySearch_Sql.FormatWith(currencyType.ToString()))
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public IEnumerable<DotPay.ViewModel.UsersCurrencyListModel> GetUsersCurrencyBySearch(int? userID, string email, string order, CurrencyType currencyType, int page, int pageCount)
        {
            var paramters = new object[] { userID.HasValue ? userID.Value : 0, email.NullSafe(), (page - 1) * pageCount, pageCount };

            var users = this.Context.Sql(getUsersCurrencyBySearch_Sql.FormatWith(currencyType.ToString(),order))
                                   .Parameters(paramters)
                                   .QueryMany<UsersCurrencyListModel>();

            return users;
        }
        /************************************************************************************/
        #region SQL
        private readonly string getUsersCurrencyCountBySearch_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"{0}account t1 
                               LEFT JOIN    " + Config.Table_Prefix + @"user t2   ON    t1.UserID=t2.ID
                                   WHERE   (@0=0  OR t2.ID=@0)
                                     AND   (@1='' OR t2.Email LIKE concat(@1,'%'))";

        private readonly string getUsersCurrencyBySearch_Sql =
                                @"SELECT    t1.ID,t2.Email,t1.Balance,t1.Locked,t1.Balance+t1.Locked as Total,t1.UpdateAt 
                                    FROM    " + Config.Table_Prefix + @"{0}account  t1 
                               LEFT JOIN    " + Config.Table_Prefix + @"user t2   ON   t1.UserID=t2.ID
                                   WHERE   (@0=0  OR t2.ID=@0)
                                     AND   (@1='' OR t2.Email LIKE concat(@1,'%'))
                                ORDER BY    {1} DESC
                                   LIMIT    @2,@3";

        private readonly string seeUserDepositAmounListModel_Sql =
                                @"SELECT   Currency,Amount 
                                    FROM   " + Config.Table_Prefix + @"user t1 
                               LEFT JOIN   " + Config.Table_Prefix + @"depositauthorization t2 
                                      ON   t1.id=t2.CustomerServiceUserID
                                   WHERE   t1.ID=@0";

        private readonly string getUserCommendCounter_Sql =
                                @"SELECT   CommendCounter 
                                    FROM   " + Config.Table_Prefix + @"User
                                   WHERE   ID=@userID";

        private readonly string getUserVipInfo_Sql =
                                @"SELECT   VipLevel,ScoreBalance,ScoreUsed
                                    FROM   " + Config.Table_Prefix + @"User
                                   WHERE   ID=@userID";

        private readonly string getMaxUserID_Sql =
                                @"SELECT   MAX(ID) 
                                    FROM   " + Config.Table_Prefix + @"User";


        private readonly string userCount_Sql =
                                @"SELECT   COUNT(*)
                                    FROM   " + Config.Table_Prefix + @"User t1 
                                            INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID 
                                   WHERE   (@0=0 OR t1.ID=@0) 
                                     AND   (@1='' OR t1.Email LIKE concat(@1,'%'))
                                     AND   (t2.IsLocked = @2)";

        private readonly string users_Sql =
                                @"SELECT   t1.ID,NickName,t1.Email,VipLevel,Mobile,t2.IsLocked,t1.CreateAt,
                                           t1.ScoreBalance,t1.VipLevel,t2.LastPasswordVerifyAt
                                    FROM   " + Config.Table_Prefix + @"User t1 
                                            INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID 
                                   WHERE   (@0=0 OR t1.ID=@0) 
                                     AND   (@1='' OR t1.Email LIKE concat(@1,'%'))
                                     AND   (t2.IsLocked = @2)
                                   LIMIT    @3,@4";

        private readonly string getUserByID_Sql =
                                 @"SELECT   t1.ID AS UserID,NickName,t1.Email,VipLevel,Mobile,t2.IsLocked,t1.CreateAt ,
                                            t2.IsEmailVerify AS IsVerifyEmail ,t1.TwoFactorFlg,t1.ScoreBalance,
                                            t1.VipLevel,t2.TradePassword
                                    FROM   " + Config.Table_Prefix + @"User t1 
                                            INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID   
                                   WHERE   t1.ID=@id ";

        private readonly string getUserByEmail_Sql =
                                 @"SELECT   t1.ID AS UserID,NickName,t1.Role,t1.Email,VipLevel,Mobile,t2.IsLocked,
                                            t1.CreateAt ,t2.IsEmailVerify AS IsVerifyEmail ,t1.TwoFactorFlg,t1.ScoreBalance,t1.VipLevel,
                                            t2.TradePassword,t2.RealName,t2.IdNoType,t2.IdNo 
                                    FROM   " + Config.Table_Prefix + @"User t1 
                                            INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID  
                                   WHERE   t1.Email=@email";

        private readonly string getUserByMobile_Sql =
                               @"SELECT   t1.ID AS UserID,NickName,t1.Email,VipLevel,Mobile,t2.IsLocked,t1.CreateAt ,
                                          t2.IsEmailVerify AS IsVerifyEmail ,t1.TwoFactorFlg,t1.ScoreBalance,t1.VipLevel
                                    FROM   " + Config.Table_Prefix + @"User t1 
                                            INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID  
                                   WHERE   t1.Mobile=@mobile";

//        private readonly string userRealNameInfo_Sql =
//                               @"SELECT    RealName,IdNoType,IdNo 
//                                    FROM   " + Config.Table_Prefix + @"Membership  
//                                   WHERE   UserID=@userID";

        private readonly string countUserByEmail_Sql =
                               @"SELECT   COUNT(*)
                                    FROM   " + Config.Table_Prefix + @"User 
                                   WHERE   Email=@email";

        private readonly string countUserByMobile_Sql =
                              @"SELECT   COUNT(*)
                                  FROM   " + Config.Table_Prefix + @"User 
                                 WHERE   Mobile=@mobile";

        private readonly string getGaSecret_Sql =
                              @"SELECT   OTPSecret
                                  FROM   " + Config.Table_Prefix + @"GoogleAuthentication 
                                 WHERE   UserID=@userID";

        private readonly string getSmsSecret_Sql =
                              @"SELECT   OTPSecret,SmsCounter
                                   FROM   " + Config.Table_Prefix + @"SmsAuthentication 
                                  WHERE   UserID=@userID";


        public readonly string getUserByOpenID_Sql =
                              @"SELECT   t1.ID AS UserID,NickName,t1.Email,VipLevel,Mobile,t2.IsLocked,
                                         t1.CreateAt ,t2.IsLocked,t2.IsEmailVerify AS IsVerifyEmail ,t1.TwoFactorFlg,
                                         t1.ScoreBalance,t1.VipLevel,t2.TradePassword
                                   FROM    " + Config.Table_Prefix + @"OpenAuthShip t0
                                           INNER JOIN " + Config.Table_Prefix + @"User t1 ON t0.UserID=t1.ID 
                                           INNER JOIN " + Config.Table_Prefix + @"Membership t2 ON t1.ID=t2.UserID   
                                  WHERE   t0.OpenID=@openID AND   t0.Type=@authType";
        #endregion

    }
}
