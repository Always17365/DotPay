using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;

namespace Dotpay.AdminQueryService
{
    public interface IUserQuery
    {
        //Task<Guid?> GetManagerIdByLoginName(string loginName);
        //Task<string> GetManagerTwofactorKeyById(Guid managerId);
        //Task<ManagerIdentity> GetManagerIdentityById(Guid managerId);
        Task<int> CountUserBySearch(string loginName);
        Task<IEnumerable<UserListViewModel>> GetUserBySearch(string email, int start, int pagesize);
        Task<IdentityInfo> GetIdentityInfoById(Guid userId);
    }
}
