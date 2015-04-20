using System.Threading.Tasks;
using Dotpay.Front.ViewModel;

namespace Dotpay.FrontQueryService
{
    public interface IUserQuery
    {
        Task<UserIdentity> GetUserByEmail(string email);
        Task<UserIdentity> GetUserByLoginName(string loginName); 
    }
}
