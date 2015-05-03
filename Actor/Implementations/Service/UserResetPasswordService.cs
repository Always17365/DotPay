using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Actor.Implementations;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;
using Orleans.Concurrency;
using RabbitMQ.Client;

namespace Dotpay.Actor.Service.Implementations
{
    [StatelessWorker]
    public class UserResetPasswordService : Grain, IUserResetPasswordService
    {
        private const string USER_MQ_EXCHANGE = Constants.UserMQName + Constants.ExechangeSuffix;
        private const string USER_EMAIL_MQ_ROUTE_KEY = Constants.UserEmailRouteKey;
        private const string USER_EMAIL_MQ_QUEUE = Constants.UserEmailMQName + Constants.QueueSuffix;

        async Task<ErrorCode> IUserResetPasswordService.ForgetLoginPassword(Guid userId, Lang lang)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            var userInfo = await user.GetUserInfo();
            var resetToken = GenerteResetLoginPasswordToken(userInfo.Email);
            var resetResult = await user.ForgetLoginPassword(resetToken);
            if (resetResult.Item1 == ErrorCode.None)
            { 
                var forgetLoginPasswordMsg = new UserForgetLoginPasswordMessage(userInfo.Email, userInfo.LoginName,
                    resetResult.Item2, DateTime.Now, lang);

                await MessageProducterManager.GetProducter().PublishMessage(forgetLoginPasswordMsg, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }

            return resetResult.Item1;
        }

        //此处独立出来，是为了以后在用户重置密码后，可以发邮件给用户
        async Task<ErrorCode> IUserResetPasswordService.ResetLoginPassword(Guid userId, string newLoginPassword, string resetToken)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            return await user.ResetLoginPassword(newLoginPassword, resetToken);
        }

        async Task<ErrorCode> IUserResetPasswordService.ForgetPaymentPassword(Guid userId, Lang lang)
        {
            var user = GrainFactory.GetGrain<IUser>(userId); 
            var userInfo = await user.GetUserInfo();
            var resetToken = GenerteResetPaymentPasswordToken(userInfo.Email);
            var resetResult = await user.ForgetPaymentPassword(resetToken);
            if (resetResult.Item1 == ErrorCode.None)
            { 
                var forgetPaymentPasswordMsg = new UserForgetPaymentPasswordMessage(userInfo.Email, userInfo.LoginName,
                    resetResult.Item2, DateTime.Now, lang);
                await MessageProducterManager.GetProducter().PublishMessage(forgetPaymentPasswordMsg, USER_MQ_EXCHANGE, USER_EMAIL_MQ_ROUTE_KEY, true);
            }
            return resetResult.Item1;
        }

        //此处独立出来，是为了以后在用户重置支付密码后，可以发邮件给用户
        async Task<ErrorCode> IUserResetPasswordService.ResetPaymentPassword(Guid userId, string newPaymentPassword, string resetToken)
        {
            var user = GrainFactory.GetGrain<IUser>(userId);
            return await user.ResetPaymentPassword(newPaymentPassword, resetToken);
        }

        public override Task OnActivateAsync()
        {
            MessageProducterManager.RegisterAndBindQueue(USER_MQ_EXCHANGE, ExchangeType.Direct, USER_EMAIL_MQ_QUEUE, USER_EMAIL_MQ_ROUTE_KEY, true);
            return base.OnActivateAsync();
        }

        #region Private Methods
        private string GenerteResetLoginPasswordToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            Random randomNum = new Random();
            var targetBytes = Encoding.UTF8.GetBytes(email + randomNum.Next() + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }
        private string GenerteResetPaymentPasswordToken(string email)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            var targetBytes = Encoding.UTF8.GetBytes(email + Guid.NewGuid() + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var md5Bytes = md5.ComputeHash(targetBytes);

            var result = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();

            return result;
        }

        #endregion
    }
}
