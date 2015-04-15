using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;
using Dotpay.Common.Enum;

namespace Dotpay.AdminQueryService
{
    public interface ITransferTransactionQuery
    {
        Task<int> CountPendingDotpayToFiTransferTx(string userLoginName);
        Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetPendingDotpayToFiTransferTx(string userLoginName, int start, int pagesize);
        Task<int> CountDotpayToFiTransferTx(string userLoginName, string sequenceNo, string transferNo, TransferTransactionStatus status);
        Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetDotpayToFiTransferTx(string userLoginName, string sequenceNo, string transferNo, TransferTransactionStatus status, int start, int pagesize);
    }
}
