using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{   
    public class LoginLogCreated : DomainEvent
    {
        public LoginLogCreated(int userID,string ip)
        {
            this.UserID = userID;
            this.IP = ip;
        }
        public int UserID { get; private set; }
        public string IP { get; private set; }
    }      
}
