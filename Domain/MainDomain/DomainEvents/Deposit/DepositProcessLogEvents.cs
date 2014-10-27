using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class DepositProcessLogCreated : DomainEvent
    {
        public DepositProcessLogCreated(int depositID, int processBy, string memo)
        {
            this.DepositID = depositID;
            this.ProcessByWho = processBy;
            this.Memo = memo;
        }
        public int DepositID { get; private set; }
        public int ProcessByWho { get; private set; }
        public string Memo { get; private set; }
    }
}
