
using FullCoin.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FullCoin.QueryService
{
    public interface IDepositQuery
    {
        int CountDepositBySearch(int? amountsearch, string email);
        IEnumerable<DepositInListModel> GetDepositBySearch(int? userID, string email, int page, int pageCount);
        int UndoCompleteCNYDeposit(int depositID, int UserID);
        //string GetUserGoogleAuthenticationSecretByID(int userID);
        //UserRealNameModel GetUserRealNameAuthInfoByID(int userID);
        //LoginUser GetUserByID(int userID);
        //LoginUser GetUserByEmail(string email);
        //LoginUser GetUserByMobile(string mobile);

        //int CountUserByEmail(string email);
        //int CountUserByMobile(string mobile);
    }
}
