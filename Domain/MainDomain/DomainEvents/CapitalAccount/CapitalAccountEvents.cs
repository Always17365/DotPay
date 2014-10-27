using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    public class CapitalAccountCreated : DomainEvent
    {
        public CapitalAccountCreated(Bank bank, string bankAccount, string ownerName, int createBy, CapitalAccount capitalAccountEntity)
        {
            this.Bank = bank;
            this.BankAccount = bankAccount;
            this.OwnerName = ownerName;
            this.CreateBy = createBy;
            this.CapitalAccountEntity = capitalAccountEntity;
        }
        public Bank Bank { get; private set; }
        public string BankAccount { get; private set; }
        public string OwnerName { get; private set; }
        public int CreateBy { get; private set; }

        public CapitalAccount CapitalAccountEntity { get; private set; }
    }

    public class CapitalAccountEnabled : DomainEvent
    {
        public CapitalAccountEnabled(int capitalAccountID, Bank bank, string bankAccount, string ownerName, int byUserID)
        {
            this.CapitalAccountID = capitalAccountID;
            this.Bank = bank;
            this.BankAccount = bankAccount;
            this.OwnerName = ownerName;
            this.ByUserID = byUserID;
        }

        public int CapitalAccountID { get; private set; }
        public Bank Bank { get; private set; }
        public string BankAccount { get; private set; }
        public string OwnerName { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class CapitalAccountDisabled : DomainEvent
    {
        public CapitalAccountDisabled(int capitalAccountID, Bank bank, string bankAccount, string ownerName, int byUserID)
        {
            this.CapitalAccountID = capitalAccountID;
            this.Bank = bank;
            this.BankAccount = bankAccount;
            this.OwnerName = ownerName;
            this.ByUserID = byUserID;
        }

        public int CapitalAccountID { get; private set; }
        public Bank Bank { get; private set; }
        public string BankAccount { get; private set; }
        public string OwnerName { get; private set; }
        public int ByUserID { get; private set; }
    }

}
