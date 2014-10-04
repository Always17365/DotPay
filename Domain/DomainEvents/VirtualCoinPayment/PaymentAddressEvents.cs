using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.Domain.Events;

namespace DotPay.Domain
{
    public class PaymentAddressCreated : DomainEvent
    {
        public PaymentAddressCreated(int userID, int accountID, string address)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.Address = address;
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public string Address { get; private set; }
    }

    public class NXTPaymentAddressCreated : DomainEvent
    {
        public NXTPaymentAddressCreated(int userID, int accountID, UInt64 nxtAccountID, string nxtAccountRS,string nxtPublicKey)
        {
            this.UserID = userID;
            this.AccountID = accountID;
            this.NXTAccountID = nxtAccountID;
            this.NxtAccountRS = nxtAccountRS;
            this.NxtPublicKey = nxtPublicKey;
        }
        public int UserID { get; private set; }
        public int AccountID { get; private set; }
        public UInt64 NXTAccountID { get; private set; }
        public string NxtAccountRS { get; private set; }
        public string NxtPublicKey { get; private set; }
    }

}
