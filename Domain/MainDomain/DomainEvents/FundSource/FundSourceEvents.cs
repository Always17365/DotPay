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
    public class FundSourceCreated : DomainEvent
    {
        public FundSourceCreated(int capitalAccountID, Bank bank, string transferTxNo,
                                 PayWay payway, decimal amount, string extra, int createBy,
                                 FundSource fundSourceEntity)
        {
            this.CapitalAccountID = capitalAccountID;
            this.Bank = bank;
            this.TransferTxNo = transferTxNo;
            this.Payway = payway;
            this.Amount = amount;
            this.Extra = extra;
            this.CreateBy = createBy;
            this.FundSourceEntity = fundSourceEntity;
        }

        public int CapitalAccountID { get; private set; }
        public Bank Bank { get; private set; }
        public string TransferTxNo { get; private set; }
        public PayWay Payway { get; private set; }
        public decimal Amount { get; private set; }
        public string Extra { get; private set; }
        public int CreateBy { get; private set; }
        public FundSource FundSourceEntity { get; private set; }
    }

    public class FundSourceVerified : DomainEvent
    {
        public FundSourceVerified(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceUndoVerified : DomainEvent
    {
        public FundSourceUndoVerified(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceSubmitToProcess : DomainEvent
    {
        public FundSourceSubmitToProcess(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceCancelSubmitToProcess : DomainEvent
    {
        public FundSourceCancelSubmitToProcess(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceCompleted : DomainEvent
    {
        public FundSourceCompleted(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceUndoComplete : DomainEvent
    {
        public FundSourceUndoComplete(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }

    public class FundSourceDeleted : DomainEvent
    {
        public FundSourceDeleted(int fundSourceID, int byUserID)
        {
            this.FundSourceID = fundSourceID;
            this.ByUserID = byUserID;
        }

        public int FundSourceID { get; private set; }
        public int ByUserID { get; private set; }
    }
}
