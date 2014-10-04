
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;     

namespace DotPay.QueryService
{
    public interface IManagerQuery
    {
        bool ExistManager(int userID, ManagerType managerType);
        int GetManagerCountBySearch(string email);
        IEnumerable<DotPay.ViewModel.ManagerModel> GetManagersBySearch(string email, int page, int pageCount);
        int GetCustomerServiceCountBySearch(string email);
        IEnumerable<DotPay.ViewModel.ManagerModel> GetCustomerServicesBySearch(string email, int page, int pageCount);
        IEnumerable<DotPay.ViewModel.ManagerModel> GetManagersByUserID(int UserID);

        
    }
}
