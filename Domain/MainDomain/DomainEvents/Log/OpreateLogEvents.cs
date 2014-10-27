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
    public class OperateLogCreated : DomainEvent
    {
        public OperateLogCreated(int domainID, string uniqueID, string memo, int opreateUserID, string ip)
        {
            this.DomainID = domainID;
            this.UniqueID = uniqueID;
            this.Memo = memo;
            this.OperateUserID = opreateUserID;
            this.IP = ip;
        }
        public int DomainID { get; private set; }
        public string UniqueID { get; private set; }
        public string Memo { get; private set; }
        public int OperateUserID { get; private set; }
        public string IP { get; private set; }
    }
}
