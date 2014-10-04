
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ILogsQuery
    {
        int GetLogsCountBySearch(string tableName, string email);
        IEnumerable<LogsInListModel> GetLogsBySearch(string tableName, string email, int page, int pageCount);

        int GetUserLoginCountBySearch(string email);
        IEnumerable<LogsInListModel> GetUserLoginBySearch(string email, int page, int pageCount);

        int GetUserLogCountBySearch(int userlogID, DateTime? starttime, DateTime? endtime);
        IEnumerable<UserLogInListModel> GetUseLogsBySearch(int userlogID, DateTime? starttime, DateTime? endtime, int page, int pageCount);

        /************************************************************/
        int CountUserLoginHistory(int userlogID);
        IEnumerable<LogsInListModel> GetUserLoginHistory(int userlogID, int start, int limit);
    }
}
