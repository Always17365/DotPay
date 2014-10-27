using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;

namespace DotPay.MainDomain
{
    public class FundSource : DomainBase, IAggregateRoot,
                              IEventHandler<FundSourceCreated>,
                              IEventHandler<FundSourceDeleted>
    {
        #region ctor
        protected FundSource() { }

        public FundSource(int capitalAccountID, Bank bank, string transferTxNo, PayWay payway, decimal amount, string extra, int createBy)
        {
            this.RaiseEvent(new FundSourceCreated(capitalAccountID, bank, transferTxNo, payway, amount, extra, createBy, this));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string UniqueID { get; protected set; }
        public virtual string TransferTxNo { get; protected set; }
        public virtual int CapitalAccountID { get; protected set; }
        public virtual PayWay PayWay { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual string Extra { get; protected set; }
        public virtual bool IsDeleted { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        public virtual int DeleteAt { get; protected set; }
        public virtual int CreateBy { get; protected set; }
        #endregion

        public virtual void Destroy(int byUserID)
        {
            this.RaiseEvent(new FundSourceDeleted(this.ID, byUserID));
        }

        #region inner event handlers
        void IEventHandler<FundSourceCreated>.Handle(FundSourceCreated @event)
        {
            this.CapitalAccountID = @event.CapitalAccountID;
            this.UniqueID = Guid.NewGuid().Shrink();
            this.TransferTxNo = @event.TransferTxNo;
            this.PayWay = @event.Payway;
            this.Amount = @event.Amount;
            this.IsDeleted =false;
            this.Extra = @event.Extra;
            this.CreateBy = @event.CreateBy;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
            this.DeleteAt = 0;
        }


        void IEventHandler<FundSourceDeleted>.Handle(FundSourceDeleted @event)
        {
            this.IsDeleted = true;
            this.DeleteAt = @event.UTCTimestamp.ToUnixTimestamp();
        }

        #endregion
    }
}
