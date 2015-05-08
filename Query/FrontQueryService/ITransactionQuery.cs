using System;
using System.Threading.Tasks;
using Dotpay.Front.ViewModel;
using System.Collections.Generic;

namespace Dotpay.FrontQueryService
{
    public interface ITransactionQuery
    {
        Task<IEnumerable<IndexTransactionListViewModel>> GetLastTenTransationByAccountId(Guid accountId/*, DateTime start, DateTime end*/);
        Task<decimal> SumDepositAmount(Guid accountId, DateTime start, DateTime end);
        Task<Tuple<decimal, decimal>> SumTransferOutAndInAmount(Guid accountId, DateTime start, DateTime end);
        Task<int> CountDepositTransaction(Guid accountId, DateTime start, DateTime end);
        Task<IEnumerable<DepositTransactionListViewModel>> GetDepositTransaction(Guid accountId, DateTime start, DateTime end, int page, int pagesize);
        Task<int> CountTransferTransaction(Guid accountId, DateTime start, DateTime end);
        Task<IEnumerable<TransferTransactionListViewModel>> GetTransferTransaction(Guid accountId, DateTime start, DateTime end, int page, int pagesize);
        Task<Guid> GetDepositTransactionIdBySeqNo(string sequenceNo);
        Task<TransferTransactionSubmitViewModel> GetTransferTransactionSubmitDetailByTxid(Guid txid);
        Task<TransferTransactionDetailViewModel> GetTransferTransactionBySeqNo(string sequenceNo);
    }
}
