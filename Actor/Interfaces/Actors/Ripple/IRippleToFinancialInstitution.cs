using System.Threading.Tasks;
using Dotpay.Common;
using Orleans;

namespace Dotpay.Actor.Interfaces.Ripple
{
    public interface IRippleToFinancialInstitution : IGrainWithIntegerKey
    {
        Task Initialize(string invoiceId, TransferTargetInfo transferTargetInfo, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Complete(string invoiceId, string txId, decimal sendAmount);
    } 
}
