using System.Threading.Tasks;
using DFramework;
using Dotpay.Actor;
using Dotpay.Actor.Service;
using Dotpay.Command;
using Orleans;

namespace Dotpay.CommandExecutor
{
    public class UserCommandExecutor : ICommandExecutor<UserRegisterCommand>,
                                      ICommandExecutor<UserActiveCommand>,
                                      ICommandExecutor<UserResendActiveEmailCommand>,
                                      ICommandExecutor<UserLoginCommand>,
                                      ICommandExecutor<InitalizePaymentPasswordCommand>,
                                      ICommandExecutor<ForgetLoginPasswordCommand>,
                                      ICommandExecutor<ResetLoginPasswordCommand>,
                                      ICommandExecutor<ModifyLoginPasswordCommand>,
                                      ICommandExecutor<ForgetPaymentPasswordCommand>,
                                      ICommandExecutor<ResetPaymentPasswordCommand>,
                                      ICommandExecutor<ModifyPaymentPasswordCommand>,
                                      ICommandExecutor<UserIdentityVerifyCommand>
    {
        public Task ExecuteAsync(UserRegisterCommand cmd)
        {
            var userRegisterService = GrainFactory.GetGrain<IUserRegisterService>(0);
            return userRegisterService.Register(cmd.Email, cmd.LoginPassword, cmd.Lang);
        }
        public Task ExecuteAsync(UserResendActiveEmailCommand cmd)
        {
            var userRegisterService = GrainFactory.GetGrain<IUserRegisterService>(0);
            return userRegisterService.ResendActiveEmail(cmd.UserId);
        }
        public async Task ExecuteAsync(UserActiveCommand cmd)
        {
            var userRegisterService = GrainFactory.GetGrain<IUserRegisterService>(0);
            cmd.CommandResult = await userRegisterService.ActiveUser(cmd.UserId, cmd.Token);
        }
        public async Task ExecuteAsync(UserLoginCommand cmd)
        {
            var user = GrainFactory.GetGrain<IUser>(cmd.UserId);
            cmd.CommandResult = await user.Login(cmd.LoginPassword, cmd.Ip);
        }

        public async Task ExecuteAsync(ModifyLoginPasswordCommand cmd)
        {
            var user = GrainFactory.GetGrain<IUser>(cmd.UserId);
            cmd.CommandResult = await user.ModifyLoginPassword(cmd.OldLoginPassword, cmd.NewLoginPassword);
        }

        public async Task ExecuteAsync(InitalizePaymentPasswordCommand cmd)
        {
            var user = GrainFactory.GetGrain<IUser>(cmd.UserId);
            await user.InitializePaymentPassword(cmd.PaymentPassword);
        }

        public async Task ExecuteAsync(ForgetLoginPasswordCommand cmd)
        {
            var resetPwdService = GrainFactory.GetGrain<IUserResetPasswordService>(0);
            cmd.CommandResult = await resetPwdService.ForgetLoginPassword(cmd.UserId, cmd.Lang);
        }

        public async Task ExecuteAsync(ResetLoginPasswordCommand cmd)
        {
            var resetPwdService = GrainFactory.GetGrain<IUserResetPasswordService>(0);
            cmd.CommandResult = await resetPwdService.ResetLoginPassword(cmd.UserId, cmd.NewLoginPassword, cmd.Token);
        }

        public async Task ExecuteAsync(ForgetPaymentPasswordCommand cmd)
        {
            var resetPwdService = GrainFactory.GetGrain<IUserResetPasswordService>(0);
            cmd.CommandResult = await resetPwdService.ForgetPaymentPassword(cmd.UserId, cmd.Lang);
        }

        public async Task ExecuteAsync(ResetPaymentPasswordCommand cmd)
        {
            var resetPwdService = GrainFactory.GetGrain<IUserResetPasswordService>(0);
            cmd.CommandResult = await resetPwdService.ResetPaymentPassword(cmd.UserId, cmd.NewPaymentPassword, cmd.Token);
        }

        public async Task ExecuteAsync(ModifyPaymentPasswordCommand cmd)
        {
            var user = GrainFactory.GetGrain<IUser>(cmd.UserId);
            cmd.CommandResult = await user.ModifyPaymentPassword(cmd.OldPaymentPassword, cmd.NewPaymentPassword);
        }

        public  Task ExecuteAsync(UserIdentityVerifyCommand cmd)
        {
            var user = GrainFactory.GetGrain<IUser>(cmd.UserId);
            return user.VeirfyIdentity(cmd.FullName, cmd.IdNoType, cmd.IdNo);
        }
    }
}
