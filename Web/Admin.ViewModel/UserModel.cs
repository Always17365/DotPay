using System;
using Dotpay.Common;

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
    }

    public class IdentityInfo
    {
        public IdentityInfo(string fullName, string idNo, IdNoType idType)
        {
            this.FullName = fullName;
            this.IdNo = idNo;
            this.IdType = IdType;
        }
        public string FullName { get; set; }
        public string IdNo { get; set; }
        public IdNoType IdType { get; set; }
    } 
}
