using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;

namespace Dotpay.Admin.ViewModel
{
    [Serializable]
    public class ManagerIdentity
    {
        public Guid ManagerId { get; set; }
        public string LoginName { get; set; }
        public IEnumerable<ManagerType> Roles { get; set; }
    }

    [Serializable]
    public class ManagerLoginViewModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    }
    [Serializable]
    public class ManagerModifyLoginPasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class CreateManagerViewModel
    {
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
    }

    public class AssignRoleViewModel
    {
        public Guid ManagerId { get; set; }
        public IEnumerable<ManagerType> Roles { get; set; }
    }

    [Serializable]
    public class ManagerListViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedAt { get; set; }
        public string LastLoginIp { get; set; }
        public string Reason { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public IEnumerable<ManagerType> Roles { get; set; }
        public IEnumerable<string> RoleNames
        {
            get
            {
                if (Roles != null)
                {
                    return this.Roles.Select(r => r.ToString("G"));
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
