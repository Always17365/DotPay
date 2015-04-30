using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;

namespace Dotpay.AdminQueryService
{
    public interface IUserQuery
    { 
        Task<int> CountUserBySearch(string email);
        Task<IEnumerable<UserListViewModel>> GetUserBySearch(string email, int start, int pagesize);
        Task<IdentityInfo> GetIdentityInfoById(Guid userId);
    }
}
