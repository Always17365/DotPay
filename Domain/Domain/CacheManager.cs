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
    [AwaitCommitted]
    public class CacheManager :
                 IEventHandler<UserLoginSuccess>,                       //用户登录成功
                 IEventHandler<UserRegisted>,                           //用户注册
                 IEventHandler<UserLocked>,                             //用户锁定
                 IEventHandler<UserUnlocked>,                           //用户解锁
    #region 用户相关

                 IEventHandler<UserNickNameChanged>,                    //修改昵称
                 IEventHandler<VerifiedEmail>,                          //邮件激活
                 IEventHandler<UserModifiedMobile>,                     //修改手机号
                 IEventHandler<UserSetGoogleAuthentication>,            //设置谷歌身份验证
                 IEventHandler<OpenLoginGAVerify>,                      //开启/关闭登录谷歌身份验证
                 IEventHandler<OpenLoginSMSVerify>,                     //开启/关闭登录短信身份验证
                 IEventHandler<OpenTwoFactorGAVerify>,                  //开启/关闭谷歌双重身份验证
                 IEventHandler<OpenTwoFactorSMSVerify>,                 //开启/关闭短信双重身份验证 
                 IEventHandler<RealNameAuthenticated>,                  //实名认证    
                 IEventHandler<UserScoreIncrease>,                      //用户积分增长
                 IEventHandler<UserScoreDecrease>,                      //用户积分减少(撤销充值)
                 IEventHandler<UserScoreUsed>,                          //用户积分被使用掉
                 IEventHandler<UserAssignedRole>,                       //用户被分配了新的角色
                 IEventHandler<UserUnsignedRole>,                       //用户被取消了角色
                 IEventHandler<UserFirstSetTradePassword>,              //用户第一次设置资金密码
                 IEventHandler<VipSettingModifyDiscount>,               //VIP调整折扣率
                 IEventHandler<VipSettingModifyScoreLine>,              //VIP调整分数线
                 IEventHandler<VipSettingModifyVoteCount>,              //VIP调整投票数量
                 IEventHandler<UserCloseGoogleAuthentication>,          //用户关闭谷歌身份验证
    #endregion

    #region 币种相关
                 IEventHandler<CurrencyCreated>,                        //币种创建
                 IEventHandler<CurrencyEnabled>,                        //币种启用
                 IEventHandler<CurrencyDisabled>,                       //币种禁用   
                 IEventHandler<CurrencyConfirmationsModified>,          //币种启用 
                 IEventHandler<CurrencyDepositRateModified>,            //币种充值费率修改 
                 IEventHandler<CurrencyWithdrawRateModified>,           //币种币种提现费率修改 
                 IEventHandler<CurrencyWithdrawOnceMinModified>,        //币种单次提现最小额 
                 IEventHandler<CurrencyWithdrawOnceLimitModified>,      //币种单次提现限额修改 
                 IEventHandler<CurrencyWithdrawVerifyLineModified>,     //币种提现审核额度
                 IEventHandler<CurrencyWithdrawDayLimitModified>,       //币种日提现限额修改
    #endregion

    #region 积分相关
                  IEventHandler<ReceiveScoreDaliyLogin>,              //用户今日首次登录
                  IEventHandler<CNYDepositCompleted>,                 //cny充值完成
                  IEventHandler<CNYDepositUndoComplete>,              //撤销cny充值
                  IEventHandler<CNYWithdrawCompleted>,                //CNY提现完成 
                  IEventHandler<VirtualCoinDepositCompleted>,         //虚拟币充值完成
                  IEventHandler<VirtualCoinWithdrawCompleted>,        //虚拟币提现完成 
    #endregion

    #region 币种账号相关
                 IEventHandler<AccountCreated>,                         //币种创建
                 IEventHandler<AccountChangedByDeposit>,                //充值成功
                 IEventHandler<AccountChangedByCancelDeposit>,          //撤销充值
                 IEventHandler<AccountChangedByWithdrawCreated>,        //提现成功
                 IEventHandler<AccountChangedByWithdrawToDepositCode>,  //提现到充值码 
                 IEventHandler<CNYWithdrawCreated>,                     //人民币提现创建
                 IEventHandler<VirtualCoinWithdrawCreated>,             //虚拟币提现创建
    #endregion

    #region 公告相关
 IEventHandler<AnnouncementCreated>,             //发布公告
                 IEventHandler<AnnouncementEdit>,                //编辑公告
                 IEventHandler<AnnouncementTop>,                 //公告置顶
                 IEventHandler<AnnouncementUnsignTop>,           //公告取消置顶 
    #endregion

    #region 文章相关
                 IEventHandler<ArticleCreated>,             //发布公告
                 IEventHandler<ArticleEdit>,                //公告置顶
                 IEventHandler<ArticleTop>,                 //公告置顶
                 IEventHandler<ArticleUnsignTop>            //公告取消置顶 
    #endregion
    {
        #region 用户相关

        #region 用户登录成功-更新用户最后活跃时间缓存
        public void Handle(UserLoginSuccess @event)
        {
            var cacheKey = CacheKey.USER_LAST_ACTIVE_TIME_PREFIX + @event.UserID;

            Cache.Add(cacheKey, @event.UTCTimestamp);
        }
        public void Handle(UserRegisted @event)
        {
            var cacheKey = CacheKey.USER_LAST_ACTIVE_TIME_PREFIX + @event.RegistUser.ID;
            Cache.Add(cacheKey, @event.UTCTimestamp);

            if (@event.CommendBy > 0)
            {
                var commendCountCacheKey = CacheKey.USER_CONMEND_COUNTER + @event.CommendBy;
                Cache.Remove(commendCountCacheKey);
            }
        }
        #endregion

        #region 用户昵称修改
        public void Handle(UserNickNameChanged @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        #endregion
        public void Handle(UserFirstSetTradePassword @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(UserSetGoogleAuthentication @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(VerifiedEmail @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(OpenLoginGAVerify @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(OpenLoginSMSVerify @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(OpenTwoFactorGAVerify @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(UserModifiedMobile @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(OpenTwoFactorSMSVerify @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(RealNameAuthenticated @event)
        {
            Cache.Remove(CacheKey.USER_REALNAME_INFO_BY_ID + @event.UserID);
        }

        public void Handle(UserScoreIncrease @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(UserScoreDecrease @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(UserScoreUsed @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(UserAssignedRole @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(UserUnsignedRole @event)
        {
            ClearUserInfoCache(@event.UserID);
        }


        public void Handle(UserLocked @event)
        {
            ClearUserInfoCache(@event.UserID);
        }

        public void Handle(UserUnlocked @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        public void Handle(UserCloseGoogleAuthentication @event)
        {
            ClearUserInfoCache(@event.UserID);
        }
        #endregion

        #region reset currency list cache
        //当币种更新后，需要强制缓存失效
        public void Handle(CurrencyCreated @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyDisabled @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyEnabled @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyDepositRateModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyWithdrawRateModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyWithdrawOnceLimitModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyWithdrawVerifyLineModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyWithdrawDayLimitModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyConfirmationsModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        public void Handle(CurrencyWithdrawOnceMinModified @event) { Cache.Remove(CacheKey.CURRENCY_LIST); }
        #endregion

        #region 公告更新后，强制使公告的缓存失效
        public void Handle(AnnouncementCreated @event) { Cache.Remove(CacheKey.ANNOUNCEMENT_TOP); }
        public void Handle(AnnouncementTop @event) { Cache.Remove(CacheKey.ANNOUNCEMENT_TOP); }
        public void Handle(AnnouncementUnsignTop @event) { Cache.Remove(CacheKey.ANNOUNCEMENT_TOP); }
        public void Handle(AnnouncementEdit @event) { Cache.Remove(CacheKey.ANNOUNCEMENT_TOP); }

        #endregion

        #region 用户账户相关
        public void Handle(AccountCreated @event)
        {
            ClearUserAccountsCache(@event.UserID);
        }

        public void Handle(AccountChangedByDeposit @event)
        {
            ClearUserAccountsCache(@event.UserID);
        }

        public void Handle(AccountChangedByCancelDeposit @event)
        {
            ClearUserAccountsCache(@event.UserID);
        }

        public void Handle(AccountChangedByWithdrawCreated @event)
        {
            ClearUserAccountsCache(@event.UserID);
        }

        public void Handle(AccountChangedByWithdrawToDepositCode @event)
        {
            ClearUserAccountsCache(@event.UserID);
        }
        public void Handle(CNYWithdrawCreated @event)
        {
            ClearUserAccountsCache(@event.WithdrawUserID);
        }

        public void Handle(VirtualCoinWithdrawCreated @event)
        {
            ClearUserAccountsCache(@event.WithdrawUserID);
        }
        #endregion

        #region 文章相关
        public void Handle(ArticleCreated @event) { }
        public void Handle(ArticleEdit @event) { ClearArticleCache(@event.ArticleID); }
        public void Handle(ArticleTop @event) { ClearArticleCache(@event.ArticleID); }
        public void Handle(ArticleUnsignTop @event) { ClearArticleCache(@event.ArticleID); }

        private void ClearArticleCache(int articleID)
        {
            var cacheKey = CacheKey.ARTICLE_QUERY + articleID;

            Cache.Remove(cacheKey);
        }
        #endregion

        #region 积分相关
        public void Handle(ReceiveScoreDaliyLogin @event) { ClearUserScoreBalanceCache(@event.UserID); }

        public void Handle(CNYDepositCompleted @event) { ClearUserScoreBalanceCache(@event.DepositUserID); }

        public void Handle(CNYDepositUndoComplete @event) { ClearUserScoreBalanceCache(@event.DepositUserID); }

        public void Handle(CNYWithdrawCompleted @event) { ClearUserScoreBalanceCache(@event.WithdrawUserID); }
        public void Handle(VirtualCoinDepositCompleted @event) { ClearUserScoreBalanceCache(@event.DepositUserID); }
        public void Handle(VirtualCoinWithdrawCompleted @event) { ClearUserScoreBalanceCache(@event.WithdrawUserID); }

        private void ClearUserScoreBalanceCache(int userID)
        {
            var cacheKey = CacheKey.USER_SCORE_BALANCE + userID;
            Cache.Remove(cacheKey);
        }
        #endregion

        #region 私有方法

        private void ClearUserInfoCache(int userID)
        {
            var user = IoC.Resolve<IRepository>().FindById<User>(userID);

            Cache.Remove(CacheKey.USER_INFO_BY_ID + userID.ToString());
            Cache.Remove(CacheKey.USER_INFO_BY_MAIL + user.Email);
            Cache.Remove(CacheKey.USER_INFO_BY_MOBILE + user.Mobile);

            if (user.IsOpenAuth)
            {
                var openAuthShip = IoC.Resolve<IOpenAuthShipRepository>().FindByUserID(userID);
                Cache.Remove(CacheKey.USER_INFO_BY_OPENID + openAuthShip.OpenID + openAuthShip.Type.ToString());
            }
        }
        private void ClearUserAccountsCache(int userID)
        {
            var cache_key = CacheKey.USER_ACCOUNTS + userID.ToString();
            Cache.Remove(cache_key);
        }

        private void ClearUserOrdersCache(int userID, MarketType market)
        {
            var cache_key = CacheKey.ORDER_USER_CURRENT + market.ToString() + userID.ToString();

            Cache.Remove(cache_key);
        }
        private void ClearUserTradeHistoryCache(int userID, MarketType market)
        {
            var cache_key = CacheKey.TRADE_HISTORY_USER_RECENT + market.ToString() + userID.ToString();

            Cache.Remove(cache_key);
        }
        #endregion

        #region VIP设置相关
        public void Handle(VipSettingModifyDiscount @event)
        {
            UpdateVipSetting(@event.UserVipLevel);
        }

        public void Handle(VipSettingModifyScoreLine @event)
        {
            UpdateVipSetting(@event.UserVipLevel);
        }

        public void Handle(VipSettingModifyVoteCount @event)
        {
            UpdateVipSetting(@event.UserVipLevel);
        }

        private void UpdateVipSetting(UserVipLevel vipLevel)
        {
            var vipSetting = IoC.Resolve<IVipSettingRepository>().FindByVipLevel(vipLevel);

            var cacheKey = CacheKey.VIP_SETTING_KEY + "DOMAIN_" + vipLevel;

            Cache.Add(cacheKey, vipSetting);
        }
        #endregion

    }
}
