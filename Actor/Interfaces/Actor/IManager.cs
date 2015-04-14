
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;

namespace Dotpay.Actor
{
    public interface IManager : Orleans.IGrainWithGuidKey
    {
        Task Initialize(string loginName, string loginPassword, string twofactorKey, Guid createBy);
        Task<ErrorCode> Login(string loginPassword, string ip);
        Task Lock(Guid lockBy, string reason);
        Task Unlock(Guid unlockBy);
        Task<bool> CheckLoginPassword(string loginPassword);
        Task<bool> CheckTwofactor(string tfPassword);
        Task<ErrorCode> ChangeLoginPassword(string oldLoginPassword, string newLoginPassword);
        Task ResetLoginPassword(string newLoginPassword, Guid resetBy);
        Task ResetTwofactorKey(Guid resetBy);
        Task AssignRoles(Guid assignBy, IEnumerable<ManagerType> roles);

        Task<bool> HasRole(ManagerType role); 
        Task<bool> HasInitialized();
    }
}
