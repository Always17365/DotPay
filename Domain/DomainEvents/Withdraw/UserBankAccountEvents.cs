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
    public class UserBankAccountCreated : DomainEvent
    {
        public UserBankAccountCreated(int userID, Bank bank, string bankAccount, string receiverName)
        {
            this.BankAccountOwnerID = userID;
            //this.BankID = bankID;
            this.Bank = bank;
            this.BankAccount = bankAccount;
            this.ReceiverName = receiverName;
        }

        public int BankAccountOwnerID { get; private set; }
        public Bank Bank { get; private set; }
        //public int BankID { get; private set; }
        public string BankAccount { get; private set; }
        public string ReceiverName { get; private set; }
    }

    public class UserBankAccountMarkAsValid : DomainEvent
    {
        public UserBankAccountMarkAsValid(int userBankAccountID, int byUserID)
        {
            this.UserBankAccountID = userBankAccountID;
            this.MarkBy = byUserID;
        }

        public int UserBankAccountID { get; private set; }
        public int MarkBy { get; private set; }
    }

    public class UserBankAccountMarkAsInvalid : DomainEvent
    {
        public UserBankAccountMarkAsInvalid(int userBankAccountID, int byUserID)
        {
            this.UserBankAccountID = userBankAccountID;
            this.MarkBy = byUserID;
        }

        public int UserBankAccountID { get; private set; }
        public int MarkBy { get; private set; }
    }
}
