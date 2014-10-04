using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain.Exceptions;
using DotPay.Domain.Events;
namespace DotPay.Domain
{

    public class Manager : DomainBase, IAggregateRoot,
                           IEventHandler<ManagerCreated>
    {
        #region ctor
        protected Manager() { }

        public Manager(int userID, ManagerType managerType)
        {
            this.RaiseEvent(new ManagerCreated(userID, managerType));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual int UserID { get; protected set; }
        public virtual ManagerType Type { get; protected set; }
        public virtual int CreateAt { get; protected set; }
        #endregion

        public virtual bool IsSystemManager() { return this.Type == ManagerType.SystemManager || this.Type == ManagerType.SuperManager; }
        public virtual bool IsFinanceBankTransferRecorder() { return this.Type == ManagerType.FinanceBankTransferRecorder || this.Type == ManagerType.SuperManager; }
        public virtual bool IsFinanceDepositVerifier() { return this.Type == ManagerType.FinanceDepositVerifier || this.Type == ManagerType.SuperManager; }
        public virtual bool IsDepositOfficer() { return this.Type == ManagerType.DepositOfficer || this.Type == ManagerType.SuperManager; }
        public virtual bool IsMonitor() { return this.Type == ManagerType.Monitor || this.Type == ManagerType.SuperManager; }
        public virtual bool IsSuperManager() { return this.Type == ManagerType.SuperManager; }
        public virtual bool IsFinanceWithdrawTransferGenerator() { return this.Type == ManagerType.FinanceWithdrawTransferGenerator || this.Type == ManagerType.SuperManager; }
        public virtual bool IsFinanceWithdrawTransferOfficer() { return this.Type == ManagerType.FinanceWithdrawTransferOfficer || this.Type == ManagerType.SuperManager; }
        public virtual bool IsWithdrawMonitor() { return this.Type == ManagerType.WithdrawMonitor || this.Type == ManagerType.SuperManager; }
        public virtual bool IsCustomerService() { return this.Type == ManagerType.CustomerService; }
        public virtual bool IsTrader() { return this.Type == ManagerType.Trader; }
        public virtual bool IsEditor() { return this.Type == ManagerType.Editor; }

        #region  inner event handlers
        void IEventHandler<ManagerCreated>.Handle(ManagerCreated @event)
        {
            this.UserID = @event.UserID;
            this.Type = @event.ManagerType;
            this.CreateAt = @event.UTCTimestamp.ToUnixTimestamp();
        }
        #endregion
    }
}
