using FC.Framework.Domain;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Exceptions
{
    public class ModifyUserRoleToSuperManagerException : DomainException
    {
        public ModifyUserRoleToSuperManagerException() : base((int)ErrorCode.NotAllowAssignSuperManager) { }
    }

    public class NoPermissionException : DomainException
    {
        public NoPermissionException() : base((int)ErrorCode.NoPermission) { }
    }
}
