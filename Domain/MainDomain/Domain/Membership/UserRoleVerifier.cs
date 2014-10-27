using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Repository;
using FC.Framework.Repository;
namespace DotPay.MainDomain
{
    [Component]
    public class UserRoleVerifier : IEventHandler<CurrencyCreated>,                         //币种创建
    #region 币种相关
                                    IEventHandler<CurrencyEnabled>,                         //币种启用
                                    IEventHandler<CurrencyDisabled>,                        //币种禁用 
                                    IEventHandler<CurrencyWithdrawRateModified>,            //币种提现费率修改 
    #endregion

    #region 角色分配相关
                                    IEventHandler<UserAssignedRole>,                        //管理员新建(用户分配管理角色)
                                    IEventHandler<ManagerRemoved>,                          //管理员删除(取消用户管理角色) 
    #endregion
          
    #region 人民币充值相关
                                    IEventHandler<CNYDepositVerified>,                      //充值审核
                                    IEventHandler<CNYDepositCompleted>,                     //充值完成
                                    IEventHandler<CNYDepositUndoComplete>,                  //撤销充值完成 
    #endregion

    #region 资金账户相关

                                    IEventHandler<CapitalAccountCreated>,                   //资金帐户的创建
                                    IEventHandler<CapitalAccountEnabled>,                   //资金帐户的创建
                                    IEventHandler<CapitalAccountDisabled>,                  //资金帐户的创建 
    #endregion

    #region 人民币提现相关
                                    IEventHandler<CNYWithdrawVerified>,                     //提现超限额审核
                                    IEventHandler<CNYWithdrawSubmitedToProcess>,            //提现提交打款申请
                                    IEventHandler<CNYWithdrawCompleted>,                    //提现打款数据录入
                                    IEventHandler<CNYWithdrawTransferFailed>,               //提现打款失败记录
                                    IEventHandler<CNYWithdrawModifiedReceiverBankAccount>,  //提现修改收款人信息 
    #endregion

    #region 操盘账户提现限制相关

                                    IEventHandler<CNYWithdrawCreated>,                      //申请CNY提现
                                    IEventHandler<VirtualCoinWithdrawCreated>,              //申请虚拟币提现
    #endregion

    #region 虚拟币提现相关
                                    IEventHandler<VirtualCoinWithdrawVerified>,             //提现超限额审核 
    #endregion 

    #region Vip 设置相关

                                    IEventHandler<VipSettingModifyScoreLine>,               //设置VIP级别积分额度 
                                    IEventHandler<VipSettingModifyDiscount>,                //设置VIP折扣 
                                    IEventHandler<VipSettingModifyVoteCount>,               //设置VIP投票数
    #endregion

    #region 文章公告相关

                                    IEventHandler<ArticleCreated>,
                                    IEventHandler<ArticleEdit>,
                                    IEventHandler<ArticleTop>,
                                    IEventHandler<ArticleUnsignTop>,
                                    IEventHandler<AnnouncementCreated>,
                                    IEventHandler<AnnouncementEdit>,
                                    IEventHandler<AnnouncementTop>,
                                    IEventHandler<AnnouncementUnsignTop>
    #endregion
    {
        #region 角色相关
        public void Handle(UserAssignedRole @event) { this.VerifyManagerIsSuperManager(@event.ByManagerUserID); }
        public void Handle(ManagerRemoved @event) { this.VerifyManagerIsSuperManager(@event.RemoveBy); }
        #endregion

        #region 币种相关
        public void Handle(CurrencyCreated @event) { this.VerifyManagerIsSystemManager(@event.CreateBy); }
        public void Handle(CurrencyEnabled @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); }
        public void Handle(CurrencyDisabled @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); } 
        public void Handle(CurrencyWithdrawRateModified @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); } 
        #endregion 

        #region 人民币充值相关
        public void Handle(CNYDepositVerified @event) { if (@event.ByUserID > 0) this.VerifyManagerIsDepositOfficer(@event.ByUserID); }
        public void Handle(CNYDepositCompleted @event) { if (@event.ByUserID > 0)this.VerifyManagerIsDepositOfficer(@event.ByUserID); }
        public void Handle(CNYDepositUndoComplete @event) { this.VerifyManagerIsDepositOfficer(@event.ByUserID); }
        #endregion

        #region 资金账户相关
        public void Handle(CapitalAccountCreated @event) { this.VerifyManagerIsWithdrawMonitor(@event.CreateBy); }
        public void Handle(CapitalAccountEnabled @event) { this.VerifyManagerIsWithdrawMonitor(@event.ByUserID); }
        public void Handle(CapitalAccountDisabled @event) { this.VerifyManagerIsWithdrawMonitor(@event.ByUserID); }
        #endregion


        #region 操盘账户相关
        public void Handle(CNYWithdrawCreated @event) { VerifyManagerIsTrader(@event.WithdrawUserID); }
        public void Handle(VirtualCoinWithdrawCreated @event) { VerifyManagerIsTrader(@event.WithdrawUserID); }
        #endregion

        #region 人民币提现相关
        public void Handle(CNYWithdrawVerified @event) { this.VerifyManagerIsWithdrawMonitor(@event.ByUserID); }
        public void Handle(CNYWithdrawSubmitedToProcess @event) { this.VerifyManagerIsFinanceWithdrawTransferGenerator(@event.ByUserID); }
        public void Handle(CNYWithdrawCompleted @event) { this.VerifyManagerIsFinanceWithdrawTransferOfficer(@event.ByUserID); }
        public void Handle(CNYWithdrawTransferFailed @event) { this.VerifyManagerIsFinanceWithdrawTransferOfficer(@event.ByUserID); }
        public void Handle(CNYWithdrawModifiedReceiverBankAccount @event) { this.VerifyManagerIsWithdrawMonitor(@event.ByUserID); }
        #endregion 

        #region 虚拟币提现相关

        public void Handle(VirtualCoinWithdrawVerified @event) { this.VerifyManagerIsWithdrawMonitor(@event.ByUserID); }
        #endregion

        #region Vip 设置相关
        public void Handle(VipSettingModifyScoreLine @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); }
        public void Handle(VipSettingModifyDiscount @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); }
        public void Handle(VipSettingModifyVoteCount @event) { this.VerifyManagerIsSystemManager(@event.ByUserID); }
        #endregion


        #region 文章和公告相关
        public void Handle(ArticleCreated @event) { this.VerifyManagerIsEditor(@event.CreateBy); }
        public void Handle(ArticleEdit @event) { this.VerifyManagerIsEditor(@event.EditBy); }
        public void Handle(ArticleUnsignTop @event) { this.VerifyManagerIsEditor(@event.ByUserID); }
        public void Handle(ArticleTop @event) { this.VerifyManagerIsEditor(@event.ByUserID); }
        public void Handle(AnnouncementCreated @event) { this.VerifyManagerIsEditor(@event.CreateBy); }
        public void Handle(AnnouncementTop @event) { this.VerifyManagerIsEditor(@event.ByUserID); }
        public void Handle(AnnouncementUnsignTop @event) { this.VerifyManagerIsEditor(@event.ByUserID); }
        public void Handle(AnnouncementEdit @event) { this.VerifyManagerIsEditor(@event.EditBy); }
        #endregion

        #region private method

        private void VerifyManagerIsSystemManager(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsSystemManager())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsFinanceRecorder(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsFinanceBankTransferRecorder())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsFinanceVerifier(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsFinanceDepositVerifier())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsDepositOfficer(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsDepositOfficer())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsSuperManager(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);

                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsSuperManager())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsFinanceWithdrawTransferGenerator(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);

                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsFinanceWithdrawTransferGenerator())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsFinanceWithdrawTransferOfficer(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var byManager = managerRepos.FindByUserID(userID);
                var userRoles = managerRepos.FindByUserID(userID);

                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsFinanceWithdrawTransferOfficer())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsWithdrawMonitor(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsWithdrawMonitor())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsCustomerService(int userID)
        {
            if (!Config.Debug)
            {
                var managerRepos = IoC.Resolve<IManagerRepository>();

                var userRoles = managerRepos.FindByUserID(userID);
                var canOperate = false;

                foreach (var role in userRoles)
                {
                    if (role.IsCustomerService())
                    {
                        canOperate = true;
                        break;
                    }
                }

                if (!canOperate)
                {
                    throw new NoPermissionException();
                }
            }
        }

        private void VerifyManagerIsTrader(int userID)
        {
            if (!Config.Debug)
            {
                var user = IoC.Resolve<IRepository>().FindById<User>(userID);

                if (user.Role > 0)
                {
                    var managerRepos = IoC.Resolve<IManagerRepository>();

                    var userRoles = managerRepos.FindByUserID(userID);
                    var canOperate = false;

                    foreach (var role in userRoles)
                    {
                        if (role.IsTrader())
                        {
                            canOperate = true;
                            break;
                        }
                    }

                    if (!canOperate)
                    {
                        throw new NoPermissionException();
                    }
                }
            }
        }

        private void VerifyManagerIsEditor(int userID)
        {
            if (!Config.Debug)
            {
                var user = IoC.Resolve<IRepository>().FindById<User>(userID);

                if (user.Role > 0)
                {
                    var managerRepos = IoC.Resolve<IManagerRepository>();

                    var userRoles = managerRepos.FindByUserID(userID);
                    var canOperate = false;

                    foreach (var role in userRoles)
                    {
                        if (role.IsEditor())
                        {
                            canOperate = true;
                            break;
                        }
                    }

                    if (!canOperate)
                    {
                        throw new NoPermissionException();
                    }
                }
            }
        }
        #endregion
         
    }
}
