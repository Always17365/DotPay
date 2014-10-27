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
    #region OpenAuthShipCreated
    public class OpenAuthShipCreated : DomainEvent
    {
        public OpenAuthShipCreated(int userID, string openID, OpenAuthType openAuthType)
        {
            this.UserID = userID;
            this.OpenID = openID;
            this.OpenAuthType = openAuthType;
        }
        public int UserID { get; private set; }
        public string OpenID { get; private set; }
        public OpenAuthType OpenAuthType { get; private set; }
    }
    #endregion

}
