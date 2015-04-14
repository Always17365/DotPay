using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Actor.Service;
using Dotpay.Common;
using Orleans;

namespace Dotpay.Actor.Implementations.Service
{

    public class ManagerService : Grain, IManagerService
    {
        async Task<ErrorCode> IManagerService.AddManager(string loginName, string loginPassword, Guid createBy)
        {
            if (!await CheckManagerPermission(createBy)) return ErrorCode.HasNoPermission;

            var managerId = Guid.NewGuid();
            var manager = GrainFactory.GetGrain<IManager>(managerId);
            var twofactorKey = Utilities.GenerateOTPKey();
            await manager.Initialize(loginName, loginPassword, twofactorKey, createBy);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.AssginManagerRoles(Guid managerId, IEnumerable<ManagerType> roles, Guid assignBy)
        {
            if (!await CheckManagerPermission(assignBy)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.AssignRoles(assignBy, roles);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.Lock(Guid managerId, string reason, Guid lockBy)
        {
            if (!await CheckManagerPermission(lockBy)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.Lock(lockBy, reason);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.Unlock(Guid managerId, Guid unlockBy)
        {
            if (!await CheckManagerPermission(unlockBy)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.Unlock(unlockBy);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.ResetLoginPassword(Guid managerId, string newLoginPassword, Guid resetBy)
        {
            if (!await CheckManagerPermission(resetBy)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.ResetLoginPassword(newLoginPassword, resetBy);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.ResetTwofactorKey(Guid managerId, Guid resetBy)
        {
            if (!await CheckManagerPermission(resetBy)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.ResetTwofactorKey(resetBy);

            return ErrorCode.None;
        }

        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.MaintenanceManager);
        }
    }
}
