using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;
using Dotpay.Common.Enum;

namespace Dotpay.AdminQueryService
{
    public interface IDepositTransactionQuery
    {
        Task<int> CountDepositTransactionBySearch(string email,string sequenceNo, Payway? payway, DepositStatus status);
        Task<IEnumerable<DepositListViewModel>> GetDepositTransactionBySearch(string email, string sequenceNo, Payway? payway, DepositStatus status, int start, int pagesize);
    }
}
