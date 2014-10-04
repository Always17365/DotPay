
using DotPay.Common;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface IUserQuery
    {
        int CountUserBySearch(int? userID, string email, bool IsLocked);
        IEnumerable<UserInListModel> GetUsersBySearch(int? userID, string email, bool IsLocked, int page, int pageCount);
        int GetMaxUserID();
        string GetUserGoogleAuthenticationSecretByID(int userID);
        Tuple<string, int> GetUserSmsSecretByID(int userID);
        //UserRealNameModel GetUserRealNameAuthInfoByID(int userID);
        LoginUser GetUserByID(int userID);
        LoginUser GetUserByEmail(string email);
        LoginUser GetUserByMobile(string mobile);
        LoginUser GetUserByOpenID(string openID, OpenAuthType authType);

        int CountUserByEmail(string email);
        int CountUserByMobile(string mobile);
        int GetUserCommendCounter(int userID);
        UserVipInfoModel GetUserVipInfo(int userID);
        IEnumerable<SeeUserDepositAmounModel> SeeUserDepositAmoun(int userID);


        int GetUsersCurrencyCountBySearch(int? userID, string email, CurrencyType currencyType);
        IEnumerable<UsersCurrencyListModel> GetUsersCurrencyBySearch(int? userID, string email, string order, CurrencyType currencyType, int page, int pageCount);
    }
}
