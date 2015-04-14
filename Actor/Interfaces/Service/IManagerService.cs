 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common;
﻿using Orleans;

namespace Dotpay.Actor.Service
{

    public interface IManagerService : IGrainWithIntegerKey
    {
        Task<ErrorCode> AddManager(string loginName, string loginPassword, Guid createBy);
        Task<ErrorCode> AssginManagerRoles(Guid managerId, IEnumerable<ManagerType> roles, Guid assignBy);
        Task<ErrorCode> Lock(Guid managerId, string reason, Guid lockBy);
        Task<ErrorCode> Unlock(Guid managerId, Guid unlockBy);
        Task<ErrorCode> ResetLoginPassword(Guid managerId, string newLoginPassword, Guid resetBy);
        Task<ErrorCode> ResetTwofactorKey(Guid managerId, Guid resetBy);
    }
}
