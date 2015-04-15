using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;

namespace Dotpay.Admin.ViewModel
{
    [Serializable]
    public class UserIdentity
    {
        public Guid UserId { get; set; }
        public string LoginName { get; set; } 
    }

    [Serializable]
    public class UserLoginViewModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    } 
    public class CreateUserViewModel
    {
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
    }
 
    [Serializable]
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedAt { get; set; }
        public string LastLoginIp { get; set; }
        public string Reason { get; set; }
        public DateTime? LastLoginAt { get; set; }  
    }
}
