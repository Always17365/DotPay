using System;
using System.Threading.Tasks;
using Dotpay.Front.ViewModel; 
using System.Collections.Generic;

namespace Dotpay.FrontQueryService
{
    public interface IAccountQuery
    {
        Task<List<AccountBalanceViewModel>> GetAccountBalanceByOwnerId(Guid userId);
        Task<Guid> GetAccountIdByOwnerId(Guid userId); 
    }
}
