using FC.Framework;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DotPay.ViewModel;
using DotPay.QueryService;
using System.Collections;

namespace DotPay.Web.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected ICommandBus CommandBus { get { return IoC.Resolve<ICommandBus>(); } }

        public LoginUser CurrentUser
        {
            get
            {
                var currentUser = Session[Constants.CurrentUserKey] != null ? (LoginUser)Session[Constants.CurrentUserKey] : null;

                if (currentUser == null && DotPay.Common.Config.Debug) return new LoginUser { UserID = 3, NickName = "123@123.com", Email = "123@123.com", Mobile = "13988888888", Role = (int)ManagerType.SuperManager };

                return currentUser;
            }
        }

        protected bool IsSuperManager
        {
            get
            {
                bool result = false;

                if (this.CurrentUser != null)
                {
                    var roleCode = (int)ManagerType.SuperManager;

                    if ((this.CurrentUser.Role & roleCode) == roleCode) result = true;
                }

                return result;
            }
        }

        protected bool IsManager(ManagerType managerType)
        {
            bool result = false;

            if (this.CurrentUser != null)
            {
                var roleCode = (int)(managerType);
                result = (this.CurrentUser.Role & roleCode) == roleCode;
            }

            return result;
        }

        public void CurrentUserPassTwoFactoryVerify()
        {
            this.CurrentUser.LoginTwoFactoryVerify = true;
        }


        protected List<KeyValuePair<string, string>> GetSelectList<T>(params Enum[] filter)
        {
            var enumValueList = new List<KeyValuePair<string, string>>();

            foreach (Enum crtEnum in Enum.GetValues(typeof(T)))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                var des = crtEnum.GetDescription();

                enumValueList.Add(new KeyValuePair<string, string>(des == string.Empty ? crtEnum.ToString() : des, crtEnum.ToString("D")));

            }

            return enumValueList;
        }

        #region
        public ActionResult GetGroupList()
        {
            List<GroupListModel> UserMenu = new List<GroupListModel>();
            var result = IoC.Resolve<IManagerQuery>().GetManagersByUserID(this.CurrentUser.UserID);
            ArrayList List = new ArrayList();
            foreach (ManagerModel list in result)
            { 
                List.Add((int)list.Type);
            }
            UserMenu = MenuCombination(List);
            return Json(UserMenu);
        }
        public List<GroupListModel> MenuCombination(ArrayList powers)
        {
            List<GroupListModel> Menu = new List<GroupListModel>();
            #region 列表
           
            Menu.Add(new GroupListModel()
            {
                title = "用户管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="用户列表", url="/user", Jurisdiction=new int[]{1,2,4,8,16,32,64,128,256,512,1024,2048}} ,  
                    new GroupItemsModel { name="已锁定用户", url="/userlock/lock", Jurisdiction=new int[]{1}} ,          
                    new GroupItemsModel { name="管理员列表", url="/manager", Jurisdiction=new int[]{32}} ,      
                    new GroupItemsModel { name="客服员列表", url="/customerservice", Jurisdiction=new int[]{32}} ,  
                },
                Jurisdiction = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 }
            });
            
            Menu.Add(new GroupListModel()
            {
                title = "系统管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="币种列表", url="/currency", Jurisdiction=new int[]{1,32}} ,  
                    new GroupItemsModel { name="短信接口", url="/smsinterface", Jurisdiction=new int[]{1,32}} ,          
                    new GroupItemsModel { name="交易市场设置", url="/market", Jurisdiction=new int[]{1,32}} ,      
                    new GroupItemsModel { name="资金账号设置", url="/capitalaccount", Jurisdiction=new int[]{1,32}} ,       
                    new GroupItemsModel { name="银行信息管理", url="/bankinfo", Jurisdiction=new int[]{1,32}} ,      
                    new GroupItemsModel { name="VIP等级设置", url="/vipsetting", Jurisdiction=new int[]{1,32}} ,      
                    new GroupItemsModel { name="虚拟币格式化小数点位数", url="/VirtualCoinDecimalPlaces", Jurisdiction=new int[]{1,32}}  
                },
                Jurisdiction = new int[] { 1, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "日志列表",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="管理员分配日志", url="/userassignrolelog/userassignrolelog", Jurisdiction=new int[]{1,32}} ,                
                    new GroupItemsModel { name="用户登陆日志", url="/userloginlog", Jurisdiction=new int[]{1,32}} ,                                         
                    new GroupItemsModel { name="用户锁定日志", url="/userlocklog/userlocklog", Jurisdiction=new int[]{1,32}} ,                              
                    new GroupItemsModel { name="用户等级设置日志", url="/vipsettinglog/vipsettinglog", Jurisdiction=new int[]{1,32}} ,                      
                    new GroupItemsModel { name="币种操作日志", url="/currencylog/currencylog", Jurisdiction=new int[]{1,32}} ,                              
                    new GroupItemsModel { name="BTC充值记录", url="/btcdepositprocesslog/btcdepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="BTC取款记录", url="/btcwithdrawprocesslog/btcwithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="CNY充值记录", url="/cnydepositprocesslog/cnydepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="CNY取款记录", url="/cnywithdrawprocesslog/cnywithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="DOGE存款日志", url="/dogedepositprocesslog/dogedepositprocesslog", Jurisdiction=new int[]{1,32}} ,          
                    new GroupItemsModel { name="DOGE提现日志", url="/dogewithdrawprocesslog/dogewithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,        
                    new GroupItemsModel { name="资金来源日志", url="/fundsourcelog/fundsourcelog", Jurisdiction=new int[]{1,32}} ,                          
                    new GroupItemsModel { name="IFC存款日志", url="/ifcdepositprocesslog/ifcdepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="IFC提现日志", url="/ifcwithdrawprocesslog/ifcwithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="LTC存款日志", url="/ltcdepositprocesslog/ltcdepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="LTC提现日志", url="/ltcwithdrawprocesslog/ltcwithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="NTX存款日志", url="/nxtdepositprocesslog/nxtdepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="NXT提现日志", url="/nxtwithdrawprocesslog/nxtwithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="FBC存款日志", url="/fbcdepositprocesslog/fbcdepositprocesslog", Jurisdiction=new int[]{1,32}} ,             
                    new GroupItemsModel { name="FBC提现日志", url="/fbcwithdrawprocesslog/fbcwithdrawprocesslog", Jurisdiction=new int[]{1,32}} ,           
                    new GroupItemsModel { name="充值授权日志", url="/depositauthorizelog/depositauthorizelog", Jurisdiction=new int[]{1,32}} ,              
                    new GroupItemsModel { name="资金账户管理日志", url="/capitalaccountopreatelog/capitalaccountopreatelog", Jurisdiction=new int[]{1,32}} ,
                    new GroupItemsModel { name="市场设置日志", url="/marketsettinglog/marketsettinglog" , Jurisdiction=new int[]{1,32}}                      
                },
                Jurisdiction = new int[] { 1, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "充值管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="人民币充值记录", url="/deposit", Jurisdiction=new int[]{8,32}} ,                
                    new GroupItemsModel { name="待处理银行到款", url="/depositwaitverify/1", Jurisdiction=new int[]{9,32}} ,                                         
                    new GroupItemsModel { name="账户充值", url="/bankrecharge/2", Jurisdiction=new int[]{8,32}} ,                              
                },
                Jurisdiction = new int[] { 8, 9, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "银行到款管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="银行新到款", url="/bankwaitverify/1", Jurisdiction=new int[]{2,32}} ,                
                    new GroupItemsModel { name="待审查银行到款", url="/bankverify/1", Jurisdiction=new int[]{4,32}} ,                                         
                    new GroupItemsModel { name="已审查银行到款", url="/bankundoverify/2", Jurisdiction=new int[]{4,32}} ,                                            
                    new GroupItemsModel { name="已处理银行到款", url="/bankcomplete/4", Jurisdiction=new int[]{2,4,32}} ,                              
                },
                Jurisdiction = new int[] { 2, 4, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "人民币提现管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="待审查人民币提现", url="/withdraw_wv/1", Jurisdiction=new int[]{16,32,64}} ,                
                    new GroupItemsModel { name="待提交人民币提现", url="/withdraw_e/2", Jurisdiction=new int[]{16,32,128}} ,                                         
                    new GroupItemsModel { name="处理中人民币提现", url="/withdraw_p/3", Jurisdiction=new int[]{16,32,256}} ,                                            
                    new GroupItemsModel { name="已处理人民币提现", url="/withdraw_c/4", Jurisdiction=new int[]{16,32,128}} ,                                           
                    new GroupItemsModel { name="被驳回提现", url="/withdraw_f/5", Jurisdiction=new int[]{16,32,64,128,256}} ,                              
                },
                Jurisdiction = new int[] { 16, 32, 64, 128, 256 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "文章管理",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="发布公告", url="/notice", Jurisdiction=new int[]{2048}} ,                
                    new GroupItemsModel { name="公告列表", url="/noticelist", Jurisdiction=new int[]{2048}} ,                                         
                    new GroupItemsModel { name="发布文章", url="/article", Jurisdiction=new int[]{2048}} ,                                            
                    new GroupItemsModel { name="文章列表", url="/articlelist", Jurisdiction=new int[]{2048}} ,                                 
                },
                Jurisdiction = new int[] { 2048 }
            });
              
            #endregion
            List<GroupListModel> UserMenu = new List<GroupListModel>();

            foreach (var temp in Menu)
            {
               if (InArray(temp.Jurisdiction, powers))
               {
                   GroupListModel GroupingColumn = new GroupListModel();
                   GroupingColumn.title = temp.title;
                   GroupingColumn.items = new List<GroupItemsModel>();
                   foreach (var item in temp.items)
                   {
                      if (InArray(item.Jurisdiction, powers))
                       {
                           GroupingColumn.items.Add(item);
                       }
                   }
                   UserMenu.Add(GroupingColumn);
               }  
            }
            return UserMenu;
        }

        public bool InArray(int[] Jurisdiction, ArrayList powers) 
        {
            foreach (int power in powers)
            {
                foreach (var temp in Jurisdiction)
                {
                    if (power == temp)
                    {
                        return true;
                    }

                }
            } 
            return false;
        }
        #endregion
    }
}
