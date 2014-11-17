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
        public ActionResult InsideTransferCNY(CurrencyType currency)
        {
            ViewBag.Currency = currency.ToString();
            return View("Inside");
        }

        [Route("~/transfer/{currency}/confirm")]
        public ActionResult InsideTransferConfirm(CurrencyType currency, string orderID)
        {
            ViewBag.Currency = currency.ToString();

            //try
            //{
            //    var transfer = IoC.Resolve<IInsideTransferQuery>().GetInsideTransferBySequenceNo(orderID, currency);
            //    var user = IoC.Resolve<IUserQuery>().GetUserByID(transfer.ToUserID);
            //    ViewBag.Transfer = transfer;
            //    ViewBag.Receiver = user;
            //}
            //finally
            //{

            //}

            return View("InsideConfirm");
        }

        [Route("~/transfer/{currency}/complete")]
        public ActionResult InsideTransferConfirm(CurrencyType currency, string orderID, string tradePassword)
        {
            ViewBag.Currency = currency.ToString();

            //try
            //{
            //    var transfer = IoC.Resolve<IInsideTransferQuery>().GetInsideTransferBySequenceNo(orderID, currency);
            //    var user = IoC.Resolve<IUserQuery>().GetUserByID(transfer.ToUserID);
            //    ViewBag.Transfer = transfer;
            //    ViewBag.Receiver = user;
            //}
            //finally
            //{

            //}

            return View("InsideConfirm");
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

        #region 内存转账提交

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

                return Redirect("~/transfer/{0}/confirm?orderid={1}".FormatWith(currency, transferCMD.Result));
            }

            return Redirect("/Error");
        }
        #endregion

        #region 内存转账确认

        [Route("~/transfer/{currency}/confirm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsideTransferConfirm(CurrencyType currency, int transferId)
        {

            return Redirect("/Error");
        }
        #endregion
        #endregion
    }
}
