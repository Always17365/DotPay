using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;

namespace Dotpay.AdminQueryService
{
    public interface IManagerQuery
    {
        Task<Guid?> GetManagerIdByLoginName(string loginName);
        Task<string> GetManagerTwofactorKeyById(Guid managerId);
        Task<ManagerIdentity> GetManagerIdentityById(Guid managerId);
        Task<int> CountManagerBySearch(string loginName);
        Task<IEnumerable<ManagerListViewModel>> GetManagerBySearch(string loginName, int page, int pagesize);
    }
}
