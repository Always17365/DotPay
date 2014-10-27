using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
namespace DotPay.MainDomain
{
    public class SmsInterfaceCreated : DomainEvent
    {
        public SmsInterfaceCreated(SmsInterfaceType smsType, string account, string password, int createBy)
        {
            this.SmsType = smsType;
            this.Account = account;
            this.Password = password;
            this.CreateBy = createBy;
        }
        public virtual SmsInterfaceType SmsType { get; private set; }
        public virtual string Account { get; private set; }
        public virtual string Password { get; private set; }
        public virtual int CreateBy { get; private set; }
    }

    public class SmsInterfaceModified : DomainEvent
    {
        public SmsInterfaceModified(SmsInterfaceType smsType, string account, string password, int byUserID)
        {
            this.SmsType = smsType;
            this.Account = account;
            this.Password = password;
            this.ByUserID = byUserID;
        }
        public virtual SmsInterfaceType SmsType { get; private set; }
        public virtual string Account { get; private set; }
        public virtual string Password { get; private set; }
        public virtual int ByUserID { get; private set; }
    }
}
