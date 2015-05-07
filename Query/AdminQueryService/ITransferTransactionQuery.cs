using System.Collections.Generic;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;
using Dotpay.Common.Enum;

namespace Dotpay.AdminQueryService
{
    public interface ITransferTransactionQuery
    {
        Task<int> CountPendingDotpayToFiTransferTx(string email);
        Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetPendingDotpayToFiTransferTx(string email, int start, int pagesize);
        Task<int> CountDotpayToFiTransferTx(string email, string sequenceNo, string transferNo, TransferTransactionStatus status);
        Task<IEnumerable<TransferFromDotpayToFiListViewModel>> GetDotpayToFiTransferTx(string email, string sequenceNo, string transferNo, TransferTransactionStatus status, int start, int pagesize);
    }
}
