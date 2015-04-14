using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

    public class AccountController : BaseController
    {

        #region For Debug
#if DEBUG
        private Guid superManagerId = new Guid("87D8E41F-10CA-4F95-B7BA-15A52FF0CA32");
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> InitSuperUser()
        {
            var result = DotpayJsonResult.UnknowFail;
            try
            {
                var cmd = new CreateSuperAdministratorCommand(superManagerId, "admin", "123456");
                await this.CommandBus.SendAsync(cmd);

                if (cmd.CommandResult == ErrorCode.None)
                    result = DotpayJsonResult.Success;
                else if (cmd.CommandResult == ErrorCode.SuperManagerHasInitialized)
                    result = DotpayJsonResult.CreateFailResult("已初始化过超级管理员了");
            }
            catch (Exception ex)
            {
                result = DotpayJsonResult.CreateFailResult(ex.Message);
            }
            return Json(result);
        }
#endif
        #endregion

        #region Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(ManagerLoginViewModel loginModel)
        {
            var result = DotpayJsonResult.UnknowFail;
            var validator = new ManagerViewModelValidator();

            var validateResult = validator.Validate(loginModel);

            if (validateResult.IsValid)
            {
                var query = IoC.Resolve<IManagerQuery>();
                try
                {
                    var manangerId = await query.GetManagerIdByLoginName(loginModel.LoginName);
                    if (!manangerId.HasValue)
                    {
                        result = DotpayJsonResult.CreateFailResult("账号或密码错误");
                    }
                    else
                    {
                        var cmd = new ManagerLoginCommand(manangerId.Value, loginModel.Password, this.GetUserIpAddress());
                        await this.CommandBus.SendAsync(cmd);

                        if (cmd.CommandResult == ErrorCode.None)
                        {
                            var manager = await query.GetManagerIdentityById(manangerId.Value);
                            this.SetWaitVerifyTwofactorLoginManager(manager);
#if DEBUG
                            this.PassVerifyTwofactor();
#endif
                            result = DotpayJsonResult.Success;
                        }
                        else if (cmd.CommandResult == ErrorCode.LoginNameOrPasswordError)
                            result = DotpayJsonResult.CreateFailResult("账号或密码错误");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Login Excepiton", ex);
                }
            }
            else
            {
                var error = validateResult.Errors.First();
                result = DotpayJsonResult.CreateSuccessResult(error.ErrorMessage);
            }
            return Json(result);
        }
        #endregion

        #region ManagerList
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager")]
        public ActionResult List()
        {
            return PartialView();
        }

        [HttpGet]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/list")]
        public async Task<ActionResult> GetManager(int draw, int start, int length)
        {
            var loginName = Request.QueryString["search[value]"].NullSafe().Trim();
            var page = start;
            var pagesize = length;
            var query = IoC.Resolve<IManagerQuery>();
            IEnumerable<ManagerListViewModel> dataList = null;
            var count = await query.CountManagerBySearch(loginName);
            if (count > 0)
            {
                dataList = await query.GetManagerBySearch(loginName, page, pagesize);
            }

            if (!this.CurrentUser.Roles.Contains(ManagerType.SuperUser) && count > 0)
            {
                count -= 1;
                dataList = dataList.Where(m => !m.Roles.Contains(ManagerType.SuperUser));
            }

            var result = PagerListModel<ManagerListViewModel, Guid>.Create(dataList, draw, count, count);
            return Json(result);
        }

        #endregion

        #region Create Manager
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/create")]
        public async Task<ActionResult> CreateManager(CreateManagerViewModel manager)
        {
            manager.LoginName = manager.LoginName.NullSafe().Trim();
            var result = DotpayJsonResult.UnknowFail;
            var validator = new CreateManagerViewModelValidator();
            var validateResult = validator.Validate(manager);

            if (validateResult.IsValid)
            {

                var query = IoC.Resolve<IManagerQuery>();
                var managerId = await query.GetManagerIdByLoginName(manager.LoginName);
                if (managerId.HasValue)
                {
                    result = DotpayJsonResult.CreateFailResult("用户名" + manager.LoginName + "已存在");
                }
                else
                {
                    try
                    {
                        var cmd = new CreateManagerCommand(manager.LoginName, manager.LoginPassword,
                            this.CurrentUser.ManagerId);
                        await this.CommandBus.SendAsync(cmd);

                        if (cmd.CommandResult == ErrorCode.None)
                            result = DotpayJsonResult.Success;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("CreateManager Exception", ex);
                    }
                }
            }
            return Json(result);
        }

        #endregion

        #region Lock/Unlock Manager
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/lock")]
        public async Task<ActionResult> LockManager(string managerId, string reason)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid managerGuidId;
            reason = reason.NullSafe().Trim();

            if (Guid.TryParse(managerId, out managerGuidId) && !string.IsNullOrEmpty(reason))
            {
                try
                {
                    var cmd = new LockManagerCommand(managerGuidId, reason, this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                    else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                        result = DotpayJsonResult.CreateFailResult("无权锁定");
                }
                catch (Exception ex)
                {
                    Log.Error("LockManager Exception", ex);
                }
            }

            return Json(result);
        }

        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/unlock")]
        public async Task<ActionResult> UnLockManager(string managerId)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid managerGuidId;

            if (Guid.TryParse(managerId, out managerGuidId))
            {
                try
                {
                    var cmd = new UnlockManagerCommand(managerGuidId, this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                    else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                        result = DotpayJsonResult.CreateFailResult("无权解除锁定");
                }
                catch (Exception ex)
                {
                    Log.Error("UnLockManager Exception", ex);
                }
            }

            return Json(result);
        }
        #endregion

        #region Assign Roles
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/assign")]
        public async Task<ActionResult> AssignRoles(AssignRoleViewModel assignRole)
        {
            var result = DotpayJsonResult.UnknowFail;
            var validator = new AssignRoleViewModelValidator();
            var validateResult = validator.Validate(assignRole);

            if (validateResult.IsValid)
            {
                try
                {
                    var cmd = new AssignManagerRolesCommand(assignRole.ManagerId, assignRole.Roles,
                        this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                }
                catch (Exception ex)
                {
                    Log.Error("AssignRoles Exception", ex);
                }
            }
            return Json(result);
        }

        #endregion

        #region View Twofactor Key
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/tfkey")]
        public async Task<ActionResult> ViewTwofactorKey(string managerId)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid managerGuidId;

            if (Guid.TryParse(managerId, out managerGuidId))
            {
                try
                {
                    var query = IoC.Resolve<IManagerQuery>();
                    var tfkey = await query.GetManagerTwofactorKeyById(managerGuidId);
                    result = DotpayJsonResult.CreateSuccessResult(tfkey);
                }
                catch (Exception ex)
                {
                    Log.Error("ViewTwofactorKey Excepiton", ex);
                }
            }

            return Json(result);
        }
        #endregion


        #region Reset Login Password
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/resetloginpwd")]
        public async Task<ActionResult> ResetLoginPassword(string managerId, string loginPassword, string confirmLoginPassword)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid managerGuidId;
            loginPassword = loginPassword.NullSafe().Trim();
            confirmLoginPassword = confirmLoginPassword.NullSafe().Trim();

            if (Guid.TryParse(managerId, out managerGuidId) && 
                !string.IsNullOrEmpty(loginPassword)&&
                !string.IsNullOrEmpty(confirmLoginPassword)&&
                loginPassword==confirmLoginPassword)
            {
                try
                {
                    var cmd = new ResetLoginPasswordCommand(managerGuidId, loginPassword, this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                    else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                        result = DotpayJsonResult.CreateFailResult("无权重置管理员密码");
                }
                catch (Exception ex)
                {
                    Log.Error("ResetLoginPassword Exception", ex);
                }
            }

            return Json(result);
        }
        #endregion

        #region Reset TwofactorKey 
        [HttpPost]
        [AllowRoles(Role = ManagerType.SuperUser, Role1 = ManagerType.MaintenanceManager)]
        [Route("~/ajax/manager/resettfkey")]
        public async Task<ActionResult> ResetTwofactorKey(string managerId)
        {
            var result = DotpayJsonResult.UnknowFail;
            Guid managerGuidId;

            if (Guid.TryParse(managerId, out managerGuidId))
            {
                try
                {
                    var cmd = new ResetTwofactorKeyCommand(managerGuidId, this.CurrentUser.ManagerId);
                    await this.CommandBus.SendAsync(cmd);

                    if (cmd.CommandResult == ErrorCode.None)
                        result = DotpayJsonResult.Success;
                    else if (cmd.CommandResult == ErrorCode.HasNoPermission)
                        result = DotpayJsonResult.CreateFailResult("无权重置管理员TF-KEY");
                }
                catch (Exception ex)
                {
                    Log.Error("ResetTwofactorKey Exception", ex);
                }
            }

            return Json(result);
        }
        #endregion
    }
}