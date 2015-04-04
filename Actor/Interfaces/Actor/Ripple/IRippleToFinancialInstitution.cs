using System.Threading.Tasks;
using Dotpay.Actor;
using Dotpay.Common;
using Orleans;
using Orleans.Concurrency;

namespace Dotpay.Actor.Ripple.Interfaces
{
    public interface IRippleToFinancialInstitution : IGrainWithIntegerKey
    {
        Task Initialize(string invoiceId, TransferToFinancialInstitutionTargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Complete(string invoiceId, string txId, decimal sendAmount);
    } 
}
