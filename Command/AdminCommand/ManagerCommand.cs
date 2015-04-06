using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFramework;
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
}
