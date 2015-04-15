using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
using DFramework.DynamicReflection;
using DFramework.Utilities;
using Dotpay.Common;

namespace Dotpay.AdminCommand
{
#if DEBUG
    public class CreateSuperAdministratorCommand : HasReturnValueCommand<ErrorCode>
    {
        public CreateSuperAdministratorCommand(Guid managerId, string loginName, string loginPassword)
        {
            this.ManagerId = managerId;
            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
        }

        public Guid ManagerId { get; private set; }
        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
    }
#endif

    #region ManagerLoginCommand
    public class ManagerLoginCommand : HasReturnValueCommand<ErrorCode>
    {
        public ManagerLoginCommand(Guid manangerId, string loginPassword, string ip)
        {
            Check.Argument.IsNotEmpty(manangerId, "manangerId");
            Check.Argument.IsNotEmpty(loginPassword, "loginPassword");
            Check.Argument.IsNotEmpty(ip, "ip");

            this.ManangerId = manangerId;
            this.LoginPassword = loginPassword;
            this.Ip = ip;
        }


        public Guid ManangerId { get; private set; }
        public string LoginPassword { get; private set; }
        public string Ip { get; private set; }
    }
    #endregion

    #region Modify Login Password
    public class ModifyLoginPasswordCommand : HasReturnValueCommand<ErrorCode>
    {
        public ModifyLoginPasswordCommand(Guid managerId, string oldLoginPassword, string newLoginPassword)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(oldLoginPassword, "oldLoginPassword");
            Check.Argument.IsNotEmpty(newLoginPassword, "newLoginPassword");

            this.ManagerId = managerId;
            this.OldLoginPassword = oldLoginPassword;
            this.NewLoginPassword = newLoginPassword;
        }

        public Guid ManagerId { get; private set; }
        public string OldLoginPassword { get; private set; }
        public string NewLoginPassword { get; private set; }
    }
    #endregion 

    #region CreateManagerCommand
    public class CreateManagerCommand : HasReturnValueCommand<ErrorCode>
    {
        public CreateManagerCommand(string loginName, string loginPassword, Guid createBy)
        {
            Check.Argument.IsNotEmpty(loginName, "loginName");
            Check.Argument.IsNotEmpty(loginPassword, "loginPassword");
            Check.Argument.IsNotEmpty(createBy, "createBy");

            this.LoginName = loginName;
            this.LoginPassword = loginPassword;
            this.CreateBy = createBy;
        }

        public string LoginName { get; private set; }
        public string LoginPassword { get; private set; }
        public Guid CreateBy { get; private set; }
    }
    #endregion

    #region AssignManagerRolesCommand
    public class AssignManagerRolesCommand : HasReturnValueCommand<ErrorCode>
    {
        public AssignManagerRolesCommand(Guid managerId, IEnumerable<ManagerType> roles, Guid assignBy)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(roles, "roles");
            Check.Argument.IsNotEmpty(assignBy, "createBy");

            this.ManagerId = managerId;
            this.AssignBy = assignBy;
            this.Roles = roles;
        }

        public Guid ManagerId { get; private set; }
        public IEnumerable<ManagerType> Roles { get; private set; }
        public Guid AssignBy { get; private set; }
    }
    #endregion

    #region Lock/Unlock
    public class LockManagerCommand : HasReturnValueCommand<ErrorCode>
    {
        public LockManagerCommand(Guid managerId, string reason, Guid lockBy)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(reason, "reason");
            Check.Argument.IsNotEmpty(lockBy, "lockBy");

            this.ManagerId = managerId;
            this.Reason = reason;
            this.LockBy = lockBy;
        }

        public Guid ManagerId { get; private set; }
        public string Reason { get; private set; }
        public Guid LockBy { get; private set; }
    }
    public class UnlockManagerCommand : HasReturnValueCommand<ErrorCode>
    {
        public UnlockManagerCommand(Guid managerId, Guid unlockBy)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(unlockBy, "unlockBy");

            this.ManagerId = managerId;
            this.UnlockBy = unlockBy;
        }

        public Guid ManagerId { get; private set; }
        public Guid UnlockBy { get; private set; }
    }
    #endregion

    #region Reset Login Password
    public class ResetLoginPasswordCommand : HasReturnValueCommand<ErrorCode>
    {
        public ResetLoginPasswordCommand(Guid managerId,string newLoginPassword  , Guid resetBy)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(newLoginPassword, "newLoginPassword");
            Check.Argument.IsNotEmpty(resetBy, "lockBy");

            this.ManagerId = managerId;
            this.NewLoginPassword = newLoginPassword;
            this.ResetBy = resetBy;
        }

        public Guid ManagerId { get; private set; }
        public string NewLoginPassword { get; private set; }
        public Guid ResetBy { get; private set; }
    }
    
    #endregion

    #region 重置Twofactor-key
    public class ResetTwofactorKeyCommand : HasReturnValueCommand<ErrorCode>
    {
        public ResetTwofactorKeyCommand(Guid managerId, Guid resetBy)
        {
            Check.Argument.IsNotEmpty(managerId, "managerId");
            Check.Argument.IsNotEmpty(resetBy, "lockBy");

            this.ManagerId = managerId;
            this.ResetBy = resetBy;
        }

        public Guid ManagerId { get; private set; } 
        public Guid ResetBy { get; private set; }
    }
    #endregion
}
