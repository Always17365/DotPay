using System;
using System.Web.Mvc;
using DotPay.QueryService;
using FC.Framework;
using DotPay.Command;
using DotPay.Common;
using DotPay.Web.Admin.Controllers;
using DotPay.Web.Admin.Filters;

namespace DotPay.Web.Admin.Areas.CNYWithdraw.Controllers
{
    public class WithdrawController : BaseController
    {
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.FinanceWithdrawTransferOfficer)]
        public ActionResult WaitSubmit()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.FinanceWithdrawTransferGenerator)]
        public ActionResult Examine()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.FinanceWithdrawTransferGenerator)]
        public ActionResult Processing()
        {
            return View();
        }
        [VerifyIsManager(ManagerType.SuperManager, ManagerType.FinanceWithdrawTransferGenerator, ManagerType.FinanceWithdrawTransferOfficer, ManagerType.WithdrawMonitor)]
        public ActionResult Complete()
        {
            return View();
        }

        [VerifyIsManager(ManagerType.SuperManager, ManagerType.WithdrawMonitor)]
       public ActionResult UndoExamine()
        {
            return View();
        }
        /******************************************************************************/
        [HttpPost]
        public ActionResult GetProvince()
        {
            var currencies = IoC.Resolve<ICityQuery>().GetAllProvince();

            return Json(currencies);
        }
        [HttpPost]
        public ActionResult GetCity(int fatherid)
        {
            var currencies = IoC.Resolve<ICityQuery>().GetCityByProvinceID(fatherid);

            return Json(currencies);
        }
        [HttpPost]
        public ActionResult GetBank(int cityid)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetBank(cityid);

            return Json(currencies);
        }
        /******************************************************************************/
        [HttpPost]
        public ActionResult GetWithdrawCountBySearch(int? amount, string transferId, WithdrawState withdrawState)
        {
            var count = IoC.Resolve<IWithdrawQuery>().CountWithdrawBySearch(amount, transferId, withdrawState);

            return Json(count);
        }

        [HttpPost]
        public ActionResult GetWithdrawBySearch(int? amount, string transferId, WithdrawState withdrawState, int page)
        {
            var currencies = IoC.Resolve<IWithdrawQuery>().GetWithdrawBySearch(amount, transferId, withdrawState, page, Constants.DEFAULT_PAGE_COUNT);

            return Json(currencies);
        }

        [HttpPost]
        public ActionResult Verify(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new CNYWithdrawVerify(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult WaitSubmit(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new SubmitCNYWithdrawToProcess(withdrawID, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult Success(int withdrawID, int transferAccountID, string transferNo)
        {
            try
            {
                var cmd = new CNYWithdrawMarkAsSuccess(withdrawID, transferAccountID, transferNo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult Transferfail(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new CNYWithdrawMarkAsTransferFail(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }
        [HttpPost]
        public ActionResult UndoApplication(int withdrawID, string memo = "")
        {
            try
            {
                var cmd = new CNYWithdrawCancel(withdrawID, memo, this.CurrentUser.UserID);
                this.CommandBus.Send(cmd);

                return Json(JsonResult.Success);
            }
            catch (CommandExecutionException ex)
            {
                return Json(new JsonResult(ex.ErrorCode));
            }
        }

        [HttpPost]
        public ActionResult ChangerUserBank(int withdrawID, Bank bank, string bankAccount)
        {
            try
            {
                 var cmd = new CNYWithdrawModifyReceiverBankAccount(withdrawID, bank, bankAccount, this.CurrentUser.UserID);
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