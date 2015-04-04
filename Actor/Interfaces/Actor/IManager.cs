
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;

namespace Dotpay.Actor
{
    public interface IManager : Orleans.IGrainWithGuidKey
    {
        Task Initialize(string loginName, string loginPassword, string twofactorKey, Guid operatorId);
        Task<ErrorCode> Login(string loginPassword, string ip);
        Task Lock(Guid operatorId, string reason);
        Task Unlock(Guid operatorId);
        Task<bool> CheckLoginPassword(string loginPassword);
        Task<bool> CheckTwofactor(string tfPassword);
        Task<ErrorCode> ChangeLoginPassword(string oldLoginPassword, string newLoginPassword);
        Task ResetLoginPassword(string newLoginPassword, Guid operatorId);
        Task AssignRoles(Guid operatorId, IEnumerable<ManagerType> roles);

        Task<bool> HasRole(ManagerType role);
    }
}
