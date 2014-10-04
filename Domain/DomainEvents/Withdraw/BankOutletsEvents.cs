using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Events
{
    public class BankOutletsCreated : DomainEvent
    {
        public BankOutletsCreated(int provinceID, int cityID, Bank bank, string bankName)
        {
            this.ProvinceID = provinceID;
            this.CityID = cityID;
            this.Bank = bank;
            this.BankName = bankName;
        }

        public int ProvinceID { get; private set; }
        public int CityID { get; private set; }
        public Bank Bank { get; private set; }
        public string BankName { get; private set; }
    }
    public class BankOutletsMarkAsDelete : DomainEvent
    {
        public BankOutletsMarkAsDelete(int bankOutletsID, int byUserID)
        {
            this.BankOutletsID = bankOutletsID;
            this.ByUserID = byUserID;
        }

        public int BankOutletsID { get; private set; }
        public int ByUserID { get; private set; }
    }
}
