using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using DotPay.QueryService;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{
    public class UserController : BaseController
    {

        public ActionResult Index()
        {
            ViewData["IsSuperManager"] = IsSuperManager;
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager)]
        public ActionResult Manager()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager)]
        public ActionResult CustomerService()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager)]
        public ActionResult DepositCodeMakeLog()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager)]
        public ActionResult Lock()
        {
            return View();
        }
        
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.Monitor)]
        public ActionResult UserInfo()
        {
            return View();
        }
        /*******************************************************************************************************************************************/
        [HttpPost]
        public ActionResult GetUserCountBySearch(int? userID, string email, bool isLocked)
        {
            var count = IoC.Resolve<IUserQuery>().CountUserBySearch(userID, email.NullSafe(), isLocked);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetUsersBySearch(int? userID, string email, bool isLocked, int page)
        {
            var result = IoC.Resolve<IUserQuery>().GetUsersBySearch(userID, email.NullSafe(), isLocked, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }
        //public ActionResult SeeUserDepositAmoun(int? userID)
        //{
        //    var result = IoC.Resolve<IUserQuery>().SeeUserDepositAmoun(userID.HasValue ? userID.Value : this.CurrentUser.UserID);

        //    return Json(result);
        //}     /*******************************************************************************************************************************************/
        [HttpPost]
        public ActionResult GetUsersCurrencyCountBySearch(int? userID, string email, CurrencyType currencyType)
        {
            var count = IoC.Resolve<IUserQuery>().GetUsersCurrencyCountBySearch(userID, email, currencyType);

            return Json(count);
        }

        //[HttpPost]
        //public ActionResult GetUsersCurrencyBySearch(int? userID, string email, string order, CurrencyType currencyType, int page)
        //{
        //    var result = IoC.Resolve<IUserQuery>().GetUsersCurrencyBySearch(userID, email, order, currencyType, page, Constants.DEFAULT_PAGE_COUNT);

        //    return Json(result);
        //}
        /*******************************************************************************************************************************************/
        [HttpPost]
        public ActionResult GetUseLogCountBySearch(int userlogID, DateTime? starttime, DateTime? endtime)
        {
            var count = IoC.Resolve<ILogsQuery>().GetUserLogCountBySearch(userlogID, starttime, endtime);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetUseLogsBySearch(int userlogID, DateTime? starttime, DateTime? endtime, int page)
        {
            var result = IoC.Resolve<ILogsQuery>().GetUseLogsBySearch(userlogID, starttime, endtime, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }

        /*******************************************************************************************************************************************/

        [HttpPost]
        public ActionResult AssignUserRole(int userID, ManagerType role)
        {
            if (userID <= 0) return Json(new JsonResult(-1));

            bool exist = IoC.Resolve<IManagerQuery>().ExistManager(userID, role);

            if (exist) return Json(new JsonResult(-2));
            else
            {
                try
                {
                    var cmd = new UserAssignRole(userID, role, this.CurrentUser.UserID);
                    this.CommandBus.Send(cmd);
                    return Json(JsonResult.Success);
                }
                catch (CommandExecutionException ex)
                {
                    if (ex.ErrorCode == (int)ErrorCode.NoPermission)
                        return Json(new JsonResult(-3));
                    else return Json(new JsonResult(ex.ErrorCode));
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveManager(int userId, int managerID)
        {
            if (managerID <= 0) return Json(new JsonResult(-1));

            try
            {
                var cmd = new RemoveManager(userId, managerID, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);
                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                if (ex.ErrorCode == (int)ErrorCode.NoPermission)
                    return Json(new JsonResult(-3));
                else return Json(new JsonResult(ex.ErrorCode));
            }

        }

        [HttpPost]
        public ActionResult GetManagerCountBySearch(int? userID, string email, int page)
        {
            var count = IoC.Resolve<IManagerQuery>().GetManagerCountBySearch(email.NullSafe());

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetManagersBySearch(int? userID, string email, int page)
        {
            var result = IoC.Resolve<IManagerQuery>().GetManagersBySearch(email.NullSafe(), page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }

        /*******************************************************************************************************************************************/
        [HttpPost]
        public ActionResult GetCustomerServiceCountBySearch(string email)
        {
            var count = IoC.Resolve<IManagerQuery>().GetCustomerServiceCountBySearch(email.NullSafe());

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetCustomerServicesBySearch(string email, int page)
        {
            var result = IoC.Resolve<IManagerQuery>().GetCustomerServicesBySearch(email.NullSafe(),page, Constants.DEFAULT_PAGE_COUNT);

            return Json(result);
        }
        [HttpPost]
        public ActionResult Lock(int userID, string reason)
        {
            var cmd = new LockUser(userID, reason, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        [HttpPost]
        public ActionResult AuthorizeCustomerServiceUserDepositAmount(int authTo,string currencyType, decimal amount)
        {
            try
            {
                var cmd = new AuthorizeCustomerServiceUserDepositAmount(authTo,  (CurrencyType)Enum.Parse(typeof(CurrencyType), currencyType), amount, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                //if (ex.ErrorCode == (int)ErrorCode.DepositAuthorizationAmountNotEnough)
                //    return Json(new JsonResult(-3));
                //else return Json(new JsonResult(ex.ErrorCode));
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        [HttpPost]
        public ActionResult Unlock(int userID, string reason)
        {
            var cmd = new UnlockUser(userID, reason, this.CurrentUser.UserID);
            this.CommandBus.Send(cmd);

            return Json(JsonResult.Success);
        }

        public ActionResult GetManagerRoles()
        {
            return Json(this.GetSelectList<ManagerType>(ManagerType.SuperManager), JsonRequestBehavior.AllowGet);
        }
    }
}
