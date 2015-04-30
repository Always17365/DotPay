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
    #region Lock/Unlock
    public class LockUserCommand : Command<ErrorCode>
    {
        public LockUserCommand(Guid userId, string reason, Guid lockBy)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(reason, "reason");
            Check.Argument.IsNotEmpty(lockBy, "lockBy");

            this.UserId = userId;
            this.Reason = reason;
            this.LockBy = lockBy;
        }

        public Guid UserId { get; private set; }
        public string Reason { get; private set; }
        public Guid LockBy { get; private set; }
    }
    public class UnlockUserCommand : Command<ErrorCode>
    {
        public UnlockUserCommand(Guid userId, Guid unlockBy)
        {
            Check.Argument.IsNotEmpty(userId, "userId");
            Check.Argument.IsNotEmpty(unlockBy, "unlockBy");

            this.UserId = userId;
            this.UnlockBy = unlockBy;
        }

        public Guid UserId { get; private set; }
        public Guid UnlockBy { get; private set; }
    }
    #endregion
}
