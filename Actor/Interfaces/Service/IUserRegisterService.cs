using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Dotpay.Common;
using Dotpay.Common.Enum;

namespace Dotpay.Actor.Service.Interfaces
{
    public interface IUserRegisterService : Orleans.IGrainWithIntegerKey
    {
        Task PreRegister(string email, Lang lang);
        Task InitUserInfo(long userId, string userAccount, string loginPassword, string tradePassword);
    }
}
