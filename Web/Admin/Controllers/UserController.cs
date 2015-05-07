using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DFramework;
using Dotpay.Admin.Fliter;
using Dotpay.Admin.Validators;
using Dotpay.Admin.ViewModel;
using Dotpay.AdminCommand;
using Dotpay.AdminQueryService;
using Dotpay.Common;

namespace Dotpay.Admin.Controllers
{ 
    public class UserController : BaseController
    { 
        #region UserList
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/user")]
        public ActionResult List()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.TransferManager, Role1 = ManagerType.DepositManager, Role2 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/user/list")]
        public async Task<ActionResult> GetUser(string email, int draw, int start, int length)
        {
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IUserQuery>();
            IEnumerable<UserListViewModel> dataList = null;
            var count = await query.CountUserBySearch(email);
            if (count > 0)
            {
                dataList = await query.GetUserBySearch(email, page, pagesize);
            } 
            var result = PagerListModel<UserListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        #endregion 
        #region View Twofactor Key
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/user/identity")]
        public async Task<ActionResult> ViewUserIdentityInfo(string managerId)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid userGuidId;

            if (Guid.TryParse(managerId, out userGuidId))
            {
                try
                {
                    var query = IoC.Resolve<IUserQuery>();
                    var identityInfo = await query.GetIdentityInfoById(userGuidId);
                    result = DotpayJsonResult<IdentityInfo>.CreateSuccessResult(identityInfo);
                }
                catch (Exception ex)
                {
                    Log.Error("ViewTwofactorKey Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion 
    }
}
