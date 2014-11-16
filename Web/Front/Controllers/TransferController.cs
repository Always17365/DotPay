using FC.Framework;
using DotPay.Common;
using DotPay.Command;
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotPay.QueryService;
using QConnectSDK.Context;
using QConnectSDK;
using System.IO;

namespace DotPay.Web.Controllers
{
    public class TransferController : BaseController
    {

        #region Views
        [Route("~/transfer/{currency}/payment")]
        public ActionResult TransferCNY(CurrencyType currency)
        {
            ViewBag.Currency = currency.ToString();
            return View("Inside");
        }

        #endregion

        #region Posts

        #region 查询账号
        [Route("~/transfer/queryacct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QueryAccount(string account)
        {
            account = account.NullSafe().Trim();

            if (account.IsEmail())
            {
                var user = IoC.Resolve<IUserQuery>().GetUserByEmail(account);
                if (user != null)
                {
                    return Json(new { Account = user.Email, RealName = user.RealName, Valid = true });
                }
                else
                {
                    return Json(new { Message = "该账户未注册，不能转账", Valid = false });
                }
            }
            return Json(new { Valid = false });
        }
        #endregion

        #region 内存转账

        [Route("~/transfer/inside/{currency}/submit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsideTransfer(string account, decimal amount, CurrencyType currency, string description)
        {
            account = account.NullSafe().Trim();
            description = description.NullSafe().Trim();
            amount = amount.ToFixed(2);

            if (account.IsEmail())
            {
                var user = IoC.Resolve<IUserQuery>().GetUserByEmail(account);

                var transferCMD = new InsideTransfer(this.CurrentUser.UserID, user.UserID, currency, amount, description);
                this.CommandBus.Send(transferCMD);
            }
            return Json(new { Valid = false });
        }
        #endregion
        #endregion
    }
}
