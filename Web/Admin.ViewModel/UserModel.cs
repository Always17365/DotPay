using System;
using Dotpay.Common;
using Newtonsoft.Json;

namespace Dotpay.Admin.ViewModel
{ 
    [Serializable]
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; } 
        public bool IsLocked { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ActiveAt { get; set; }
        public DateTime? LockedAt { get; set; }
        public string LastLoginIp { get; set; }
        public string Reason { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool VerifyRealName { get; set; }
    }

    [Serializable]
    public class IdentityInfo
    {
        public string FullName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdType { get; set; }
    } 
}
