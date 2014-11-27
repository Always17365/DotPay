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
    public class UserReceiverAccountCreated : DomainEvent
    {
        public UserReceiverAccountCreated(int userID, PayWay payway, string bankAccount, string receiverName)
        {
            this.BankAccountOwnerID = userID;
            //this.BankID = bankID;
            this.PayWay = payway;
            this.BankAccount = bankAccount;
            this.ReceiverName = receiverName;
        }

        public int BankAccountOwnerID { get; private set; }
        public PayWay PayWay { get; private set; }
        //public int BankID { get; private set; }
        public string BankAccount { get; private set; }
        public string ReceiverName { get; private set; }
    }

    public class UserReceiverAccountMarkAsValid : DomainEvent
    {
        public UserReceiverAccountMarkAsValid(int userBankAccountID, int byUserID)
        {
            this.UserBankAccountID = userBankAccountID;
            this.MarkBy = byUserID;
        }

        public int UserBankAccountID { get; private set; }
        public int MarkBy { get; private set; }
    }

    public class UserReceiverAccountMarkAsInvalid : DomainEvent
    {
        public UserReceiverAccountMarkAsInvalid(int userBankAccountID, int byUserID)
        {
            this.UserBankAccountID = userBankAccountID;
            this.MarkBy = byUserID;
        }

        public int UserBankAccountID { get; private set; }
        public int MarkBy { get; private set; }
    }
}
