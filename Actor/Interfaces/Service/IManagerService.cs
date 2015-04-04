 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Orleans;

namespace Dotpay.Actor.Service
{

    public interface IManagerService : IGrainWithIntegerKey
    {
        Task<ErrorCode> AddManager(string loginName, string loginPassword, Guid operatorId);
        Task<ErrorCode> AssginManagerRoles(Guid managerId, IEnumerable<ManagerType> roles, Guid operatorId);
        Task<ErrorCode> Lock(Guid managerId, string reason, Guid operatorId);
        Task<ErrorCode> Unlock(Guid managerId, Guid operatorId);
        Task<ErrorCode> ResetLoginPassword(Guid managerId, string newLoginPassword, Guid operatorId);
    }
}
