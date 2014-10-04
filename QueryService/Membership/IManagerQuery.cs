
using FullCoin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;     

namespace FullCoin.QueryService
{
    public interface IManagerQuery
    {
        int GetManagerCountBySearch(string email);
        bool ExistManager(int userID,ManagerType managerType);
        IEnumerable<FullCoin.ViewModel.ManagerModel> GetManagersBySearch(string email, int page, int pageCount);
    }
}
