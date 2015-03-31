using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
﻿using Dotpay.Actor.Interfaces;
﻿using Dotpay.Actor.Service.Interfaces;
﻿using Dotpay.Common;
﻿using Orleans;

namespace Dotpay.Actor.Implementations.Service
{

    public class ManagerService : Orleans.Grain, IManagerService
    {
        async Task<ErrorCode> IManagerService.AddManager(string loginName, string loginPassword, Guid operatorId)
        {
            if (!await CheckManagerPermission(operatorId)) return ErrorCode.HasNoPermission;

            var managerId = Guid.NewGuid();
            var manager = GrainFactory.GetGrain<IManager>(managerId);
            var twofactorKey = Utilities.GenerateOTPKey();
            await manager.Initialize(loginName, loginPassword, twofactorKey, operatorId);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.AssginManagerRoles(Guid managerId, IEnumerable<ManagerType> roles, Guid operatorId)
        {
            if (!await CheckManagerPermission(operatorId)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.AssignRoles(operatorId, roles);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.Lock(Guid managerId, string reason, Guid operatorId)
        {
            if (!await CheckManagerPermission(operatorId)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.Lock(operatorId, reason);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.Unlock(Guid managerId, Guid operatorId)
        {
            if (!await CheckManagerPermission(operatorId)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.Unlock(operatorId);

            return ErrorCode.None;
        }

        async Task<ErrorCode> IManagerService.ResetLoginPassword(Guid managerId, string newLoginPassword, Guid operatorId)
        {
            if (!await CheckManagerPermission(operatorId)) return ErrorCode.HasNoPermission;

            var manager = GrainFactory.GetGrain<IManager>(managerId);
            await manager.ResetLoginPassword(newLoginPassword, operatorId);

            return ErrorCode.None;
        }

        private Task<bool> CheckManagerPermission(Guid operatorId)
        {
            var @operator = GrainFactory.GetGrain<IManager>(operatorId);

            return @operator.HasRole(ManagerType.SuperUser);
        }
    }
}
