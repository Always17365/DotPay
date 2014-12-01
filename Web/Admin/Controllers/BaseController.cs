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

                if (currentUser == null && DotPay.Common.Config.Debug) return new LoginUser { UserID = 3, Email = "123@123.com", Mobile = "13988888888", Role = (int)ManagerType.SuperManager };

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
                title = "直转待处理页面",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="支付宝", url="/transfertransaction/pending/alipay", Jurisdiction=new int[]{8,32}} ,                
                    new GroupItemsModel { name="财付通", url="/transfertransaction/pending/tenpay", Jurisdiction=new int[]{9,32}} ,                   
                },
                Jurisdiction = new int[] { 8, 9, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "后台直转成功记录页面",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="支付宝", url="/transfertransaction/success/alipay", Jurisdiction=new int[]{8,32}} ,                
                    new GroupItemsModel { name="财付通", url="/transfertransaction/success/tenpay", Jurisdiction=new int[]{9,32}} ,                   
                },
                Jurisdiction = new int[] { 8, 9, 32 }
            });
            Menu.Add(new GroupListModel()
            {
                title = "后台直转记录失败页面",
                items = new List<GroupItemsModel> 
                { 
                    new GroupItemsModel { name="支付宝", url="/transfertransaction/fail/alipay", Jurisdiction=new int[]{8,32}} ,                
                    new GroupItemsModel { name="财付通", url="/transfertransaction/fail/tenpay", Jurisdiction=new int[]{9,32}} ,                   
                },
                Jurisdiction = new int[] { 8, 9, 32 }
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
            return true;
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
