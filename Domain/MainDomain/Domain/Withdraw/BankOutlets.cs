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
    public  class BankOutlets : DomainBase, IAggregateRoot,
                                        IEventHandler<BankOutletsCreated>,
                                        IEventHandler<BankOutletsMarkAsDelete>
    {
        #region ctor
        protected BankOutlets() { }
        public BankOutlets(int provinceID, int cityID, Bank bank, string bankName)
        {
            this.RaiseEvent(new BankOutletsCreated(provinceID, cityID, bank, bankName));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual Bank Bank { get; protected set; }
        public virtual int ProvinceID { get; protected set; }
        public virtual int CityID { get; protected set; }
        public virtual bool IsDelete { get; protected set; }
        public virtual int DeleteBy { get; protected set; }
        public virtual string Name { get; protected set; }
        #endregion

        public virtual void MarkAsDelete(int byUserID)
        {
            if (!this.IsDelete)
                this.RaiseEvent(new BankOutletsMarkAsDelete(this.ID, byUserID));
        }

        void IEventHandler<BankOutletsCreated>.Handle(BankOutletsCreated @event)
        {
            this.ProvinceID = @event.ProvinceID;
            this.CityID = @event.CityID;
            this.Bank = @event.Bank;
            this.Name = @event.BankName;
        }

        void IEventHandler<BankOutletsMarkAsDelete>.Handle(BankOutletsMarkAsDelete @event)
        {
            this.IsDelete = true;
            this.DeleteBy = @event.ByUserID;
        }
    }
}
