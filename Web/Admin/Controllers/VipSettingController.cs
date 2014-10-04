using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using FC.Framework;
using System.Web.Helpers;
using DotPay.ViewModel;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Controllers
{

    public class VipSettingController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.SystemManager, ManagerType.Monitor)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetVipSettingBySearch(){

            var currencies = IoC.Resolve<IVipSettingQuery>().GetVipSettingBySearch();

            return Json(currencies);
        }
        [HttpPost]
        public ActionResult SetVipLevelDiscount(int vipLevelId, decimal value)
        {
            try
            {
                var cmd = new SetVipLevelDiscount(vipLevelId, value, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult SetVipLevelScoreLine(int vipLevelId, int value)
        {
            try
            {
                var cmd = new SetVipLevelScoreLine(vipLevelId, value, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult SetVipLevelVoteCount(int vipLevelId, int value)
        {
            try
            {
                var cmd = new SetVipLevelVoteCount(vipLevelId, value, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
    }
}
