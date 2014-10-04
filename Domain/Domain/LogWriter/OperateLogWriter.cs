using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Repository;

namespace DotPay.Domain
{
    [Component]
    public class OperateLogWriter : IEventHandler<CurrencyCreated>,

    #region 币种相关
        //币种创建
         IEventHandler<CurrencyEnabled>,                      //币种启用
         IEventHandler<CurrencyDisabled>,                     //币种禁用
         IEventHandler<CurrencyDepositRateModified>,          //币种充值费率修改
         IEventHandler<CurrencyWithdrawRateModified>,         //币种提现费率修改   
         IEventHandler<CurrencyWithdrawVerifyLineModified>,   //币种提现审核额度 
         IEventHandler<CurrencyWithdrawDayLimitModified>,     //币种提现日限额
         IEventHandler<CurrencyConfirmationsModified>,        //币种需要的确认次数修改   
         IEventHandler<CurrencyWithdrawOnceLimitModified>,    //币种需要的确认次数修改
    #endregion

    #region 资金帐户相关
         IEventHandler<CapitalAccountCreated>,             //资金帐户创建
         IEventHandler<CapitalAccountEnabled>,             //资金帐户启用
         IEventHandler<CapitalAccountDisabled>,            //资金帐户禁用
    #endregion

    #region 管理角色相关
         IEventHandler<UserAssignedRole>,                  //管理员新建(用户分配管理角色)
         IEventHandler<ManagerRemoved>,                    //管理员删除(取消用户管理角色)
         IEventHandler<UserLocked>,                        //用户锁定
         IEventHandler<UserUnlocked>,                      //用户解除锁定
    #endregion
         
    #region 充值相关
         IEventHandler<CNYDepositVerified>,                 //RMB充值审核
         IEventHandler<CNYDepositCompleted>,                //RMB充值完成
         IEventHandler<VirtualCoinDepositVerified>,         //虚拟币充值审核
         IEventHandler<VirtualCoinDepositCompleted>,        //虚拟币充值完成
    #endregion

    #region 提现相关
         IEventHandler<CNYWithdrawVerified>,                       //RMB提现审核
         IEventHandler<CNYWithdrawSubmitedToProcess>,              //RMB提现提交处理
         IEventHandler<CNYWithdrawCompleted>,                      //RMB提现完成
         IEventHandler<CNYWithdrawTransferFailed>,                 //RMB提现转账失败
         IEventHandler<CNYWithdrawModifiedReceiverBankAccount>,    //RMB提现收款人信息修改

         IEventHandler<VirtualCoinWithdrawVerified>,               //虚拟币提现审核
         IEventHandler<VirtualCoinWithdrawCompleted>,              //虚拟币提现完成
    #endregion 

    #region 充值码相关
         IEventHandler<VipSettingModifyScoreLine>,                   //VIP设置修改积分线
         IEventHandler<VipSettingModifyDiscount>,                    //VIP设置修改折扣
         IEventHandler<VipSettingModifyVoteCount>                   //VIP设置修改投票数
    #endregion
    {
        #region 币种相关实现
        public void Handle(CurrencyCreated @event)
        {
            var memo = "新建了币种-" + @event.CurrencyName + "(" + @event.CurrencyCode + ")";
            var log = new CurrencyLog(@event.CurrencyID, @event.CreateBy, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CurrencyEnabled @event)
        {
            var memo = "启用了币种-" + @event.CurrencyCode;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CurrencyDisabled @event)
        {
            var memo = "禁用了币种-" + @event.CurrencyCode;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CurrencyDepositRateModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的充值费率为:" + currency.DepositFixedFee + "/笔+" + currency.DepositFeeRate.ToString("P");
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CurrencyWithdrawRateModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的提现费率为:" + currency.DepositFixedFee + "/笔+" + currency.DepositFeeRate.ToString("P");
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CurrencyConfirmationsModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的确认次数为:" + @event.NeedConfirm;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);
        }

        public void Handle(CurrencyWithdrawVerifyLineModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的提现人工审核额度为:" + @event.VerifyLine;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);
        }

        public void Handle(CurrencyWithdrawDayLimitModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的提现日限额为:" + @event.DayLimit;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);
        }
        public void Handle(CurrencyWithdrawOnceLimitModified @event)
        {
            var currency = IoC.Resolve<IRepository>().FindById<Currency>(@event.CurrencyID);

            var memo = "修改币种-" + currency.Code + "的提现单次限额为:" + @event.OnceLimit;
            var log = new CurrencyLog(@event.CurrencyID, @event.ByUserID, memo);
        }
        #endregion

        #region 管理角色相关实现
        public void Handle(ManagerRemoved @event)
        {
            var manager = IoC.Resolve<IRepository>().FindById<Manager>(@event.UserIDOfManager);
            var roleName = manager.Type.GetDescription();
            var memo = "ID为" + manager.UserID + "的用户被取消" + roleName + "角色";
            var log = new UserAssignRoleLog(@event.UserIDOfManager, memo, @event.RemoveBy, string.Empty);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(UserAssignedRole @event)
        {
            var user = IoC.Resolve<IRepository>().FindById<User>(@event.UserID);
            var roleName = @event.ManagerType.GetDescription();
            var memo = "ID为" + user.ID + "的用户增加了" + roleName + "角色";
            var log = new UserAssignRoleLog(@event.UserID, memo, @event.ByManagerUserID, string.Empty);

            IoC.Resolve<IRepository>().Add(log);
        }
        public void Handle(UserLocked @event)
        {
            var user = IoC.Resolve<IRepository>().FindById<User>(@event.UserID);
            var memo = "ID为" + user.ID.ToString() + " (Email:" + user.Email + ")被锁定,原因:" + @event.Reason;
            var log = new UserLockLog(@event.UserID, memo, @event.ByUserID, string.Empty);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(UserUnlocked @event)
        {
            var user = IoC.Resolve<IRepository>().FindById<User>(@event.UserID);
            var memo = "ID为" + user.ID.ToString() + " (Email:" + user.Email + ")解除锁定,原因:" + @event.Reason;
            var log = new UserLockLog(@event.UserID, memo, @event.ByUserID, string.Empty);

            IoC.Resolve<IRepository>().Add(log);
        }
        #endregion 

        #region 资金帐户相关
        public void Handle(CapitalAccountCreated @event)
        {
            var capitalAccount = @event.CapitalAccountEntity;
            var memo = "创建了收款账户";
            var log = new CapitalAccountOpreateLog(capitalAccount.ID, capitalAccount.UniqueID, memo, @event.CreateBy);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CapitalAccountEnabled @event)
        {
            var capitalAccount = IoC.Resolve<IRepository>().FindById<CapitalAccount>(@event.CapitalAccountID);
            var memo = "启用了收款账户";
            var log = new CapitalAccountOpreateLog(capitalAccount.ID, string.Empty, memo, @event.ByUserID);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CapitalAccountDisabled @event)
        {
            var memo = "禁用了收款账户";
            var log = new CapitalAccountOpreateLog(@event.CapitalAccountID, string.Empty, memo, @event.ByUserID);
            IoC.Resolve<IRepository>().Add(log);
        }
        #endregion

        #region 充值相关

        public void Handle(CNYDepositVerified @event)
        {
            if (@event.ByUserID > 0)
            {
                var memo = "核实了充值";
                var log = DepositProcessLogFactory.CreateLog(CurrencyType.CNY, @event.DepositID, @event.DepositUniqueID, @event.ByUserID, memo);
                IoC.Resolve<IRepository>().Add(log);
            }
        }

        public void Handle(CNYDepositCompleted @event)
        {
            if (@event.ByUserID > 0)
            {
                var memo = "完成了充值";
                var log = DepositProcessLogFactory.CreateLog(CurrencyType.CNY, @event.DepositID, string.Empty, @event.ByUserID, memo);
                IoC.Resolve<IRepository>().Add(log);
            }
        }

        public void Handle(VirtualCoinDepositVerified @event)
        {
            var memo = "完成了核实";
            var log = DepositProcessLogFactory.CreateLog(@event.Currency, @event.DepositID, @event.DepositUniqueID, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }


        public void Handle(VirtualCoinDepositCompleted @event)
        {
            var memo = "完成了充值";
            var log = DepositProcessLogFactory.CreateLog(@event.Currency, @event.DepositID, string.Empty, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }
        #endregion

        #region 提现相关
        public void Handle(CNYWithdrawVerified @event)
        {
            var memo = "审核了提现";
            var log = new CNYWithdrawProcessLog(@event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CNYWithdrawSubmitedToProcess @event)
        {
            var memo = "提交处理";
            var log = new CNYWithdrawProcessLog(@event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CNYWithdrawCompleted @event)
        {
            var memo = "打款完毕";
            var log = new CNYWithdrawProcessLog(@event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CNYWithdrawTransferFailed @event)
        {
            var memo = "打款失败";
            var log = new CNYWithdrawProcessLog(@event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(CNYWithdrawModifiedReceiverBankAccount @event)
        {
            var memo = "修改收款人账号";
            var log = new CNYWithdrawProcessLog(@event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(VirtualCoinWithdrawVerified @event)
        {
            var memo = "审核了虚拟币提现";
            var log = VirtualCoinWithdrawProcessLogFactory.CreateLog(@event.Currency, @event.WithdrawID, @event.ByUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(VirtualCoinWithdrawCompleted @event)
        {
            var memo = "虚拟币打款完毕";
            var nullUserID = 0;
            var log = VirtualCoinWithdrawProcessLogFactory.CreateLog(@event.Currency, @event.WithdrawID, nullUserID, memo);
            IoC.Resolve<IRepository>().Add(log);
        }
        #endregion
         
        #region VIP设置相关
        public void Handle(VipSettingModifyScoreLine @event)
        {
            var memo = "分数线修改为" + @event.ScoreLine;

            var log = new VipSettingLog((int)@event.UserVipLevel, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(VipSettingModifyDiscount @event)
        {
            var memo = "折扣率修改为" + @event.Discount.ToString("P");

            var log = new VipSettingLog((int)@event.UserVipLevel, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }

        public void Handle(VipSettingModifyVoteCount @event)
        {
            var memo = "投票权修改为" + @event.VoteCount;

            var log = new VipSettingLog((int)@event.UserVipLevel, @event.ByUserID, memo);

            IoC.Resolve<IRepository>().Add(log);
        }
        #endregion
    }
}
