using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class Currency : DomainBase, IAggregateRoot,
                           IEventHandler<CurrencyCreated>,                       //创建币种
                           IEventHandler<CurrencyEnabled>,                       //币种启用
                           IEventHandler<CurrencyDisabled>,                      //币种禁用
                           IEventHandler<CurrencyDepositRateModified>,           //修改币种的充值费率
                           IEventHandler<CurrencyWithdrawRateModified>,          //修改币种的提现费率   
                           IEventHandler<CurrencyWithdrawOnceMinModified>,       //修改币种的单次最小提现额
                           IEventHandler<CurrencyWithdrawOnceLimitModified>,     //修改币种的单次提现限额
                           IEventHandler<CurrencyWithdrawVerifyLineModified>,    //修改币种的提现审核额度
                           IEventHandler<CurrencyWithdrawDayLimitModified>,      //修改币种的提现日限额
                           IEventHandler<CurrencyConfirmationsModified>          //修改虚拟币对应的确认次数(多少个确认后认为交易完成)
    {
        #region ctor
        protected Currency() { }

        public Currency(int currencyID, string code, string name, int createBy)
        {
            this.RaiseEvent(new CurrencyCreated(currencyID, code, name, createBy));
        }
        #endregion

        #region properties
        public virtual int ID { get; protected set; }
        public virtual string Code { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual decimal DepositFixedFee { get; protected set; }
        public virtual decimal DepositFeeRate { get; protected set; }
        public virtual decimal WithdrawFeeRate { get; protected set; }
        public virtual decimal WithdrawFixedFee { get; protected set; }
        public virtual decimal WithdrawDayLimit { get; protected set; }
        public virtual decimal WithdrawOnceMin { get; protected set; }
        public virtual decimal WithdrawOnceLimit { get; protected set; }
        public virtual decimal WithdrawVerifyLine { get; protected set; }
        public virtual int NeedConfirm { get; protected set; }
        public virtual bool IsEnable { get; protected set; }
        public virtual bool IsLocked { get; protected set; }

        public virtual int CreateAt { get; protected set; }
        public virtual int CreateBy { get; protected set; }
        #endregion

        #region public method

        public virtual void Enable(int byUserID)
        {
            if (this.IsLocked)
                throw new CurrencyIsLockedException();

            if (this.IsEnable) return;

            this.RaiseEvent(new CurrencyEnabled(this.ID, this.Code, byUserID));
        }

        public virtual void Disable(int byUserID)
        {
            if (this.IsLocked)
                throw new CurrencyIsLockedException();

            if (!this.IsEnable) return;

            this.RaiseEvent(new CurrencyDisabled(this.ID, this.Code, byUserID));
        }

        public virtual void ModifyDepositFeeRate(decimal depositFixedFee, decimal depositFeeRate, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyDepositRateModified(this.ID, depositFixedFee, depositFeeRate, byUserID));
        }

        public virtual void ModifyWithdrawFeeRate(decimal withdrawFixedFee, decimal withdrawFeeRate, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyWithdrawRateModified(this.ID, withdrawFixedFee, withdrawFeeRate, byUserID));
        }

        public virtual void ModifyNeedConfirm(int needConfirm, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyConfirmationsModified(this.ID, needConfirm, byUserID));
        }

        public virtual void ModifyWithdrawVerifyLine(decimal verifyLine, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyWithdrawVerifyLineModified(this.ID, verifyLine, byUserID));
        }

        public virtual void ModifyWithdrawDayLimit(decimal dayLimit, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyWithdrawDayLimitModified(this.ID, dayLimit, byUserID));
        }

        public virtual void ModifyWithdrawOnceLimit(decimal onceLimit, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyWithdrawOnceLimitModified(this.ID, onceLimit, byUserID));
        }
        public virtual void ModifyWithdrawOnceMin(decimal onceMin, int byUserID)
        {
            if (!this.IsEnable) throw new CurrencyIsDisabledException();

            this.RaiseEvent(new CurrencyWithdrawOnceMinModified(this.ID, onceMin, byUserID));
        }
        #endregion

        #region inner event handlers
        void IEventHandler<CurrencyCreated>.Handle(CurrencyCreated @event)
        {
            this.ID = @event.CurrencyID;
            this.Code = @event.CurrencyCode;
            this.Name = @event.CurrencyName;
            this.CreateAt = DateTime.Now.ToUnixTimestamp();
            this.DepositFixedFee = 0;
            this.DepositFeeRate = 0;
            this.WithdrawFixedFee = 0;
            this.WithdrawFeeRate = 0;
            this.WithdrawDayLimit = 0;
            this.WithdrawOnceLimit = 0;
            this.WithdrawVerifyLine = 0;
            this.IsEnable = true;
            this.IsLocked = false;
            this.CreateBy = @event.CreateBy;
        }

        void IEventHandler<CurrencyEnabled>.Handle(CurrencyEnabled @event)
        {
            this.IsEnable = true;
        }

        void IEventHandler<CurrencyDisabled>.Handle(CurrencyDisabled @event)
        {
            this.IsEnable = false;
        }

        void IEventHandler<CurrencyDepositRateModified>.Handle(CurrencyDepositRateModified @event)
        {
            this.DepositFixedFee = @event.DepositFixedFee;
            this.DepositFeeRate = @event.DepositFeeRate;
        }

        void IEventHandler<CurrencyWithdrawRateModified>.Handle(CurrencyWithdrawRateModified @event)
        {
            this.WithdrawFixedFee = @event.WithdrawFixedFee;
            this.WithdrawFeeRate = @event.WithdrawFeeRate;
        }

        void IEventHandler<CurrencyConfirmationsModified>.Handle(CurrencyConfirmationsModified @event)
        {
            this.NeedConfirm = @event.NeedConfirm;
        }

        void IEventHandler<CurrencyWithdrawOnceLimitModified>.Handle(CurrencyWithdrawOnceLimitModified @event)
        {
            this.WithdrawOnceLimit = @event.OnceLimit;
        }

        void IEventHandler<CurrencyWithdrawVerifyLineModified>.Handle(CurrencyWithdrawVerifyLineModified @event)
        {
            this.WithdrawVerifyLine = @event.VerifyLine;
        }

        void IEventHandler<CurrencyWithdrawDayLimitModified>.Handle(CurrencyWithdrawDayLimitModified @event)
        {
            this.WithdrawDayLimit = @event.DayLimit;
        }
        void IEventHandler<CurrencyWithdrawOnceMinModified>.Handle(CurrencyWithdrawOnceMinModified @event)
        {
            this.WithdrawOnceMin = @event.OnceMin;
        }
        #endregion


    }
}
