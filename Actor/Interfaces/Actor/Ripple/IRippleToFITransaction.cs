using System;
using System.Threading.Tasks;
using Dotpay.Common;
using Dotpay.Common.Enum;
using Orleans;

namespace Dotpay.Actor
{
    public interface IRippleToFITransaction : IGrainWithIntegerKey
    {
        Task Initialize(string rippleTxId, string invoiceId, Payway payway,string destination,string realName,CurrencyType currency, decimal amount, decimal sendAmount, string memo);
        Task<ErrorCode> Lock(Guid managerId);
        Task<ErrorCode> Complete(string transferNo, Guid managerId, string memo);
        Task<ErrorCode> Fail(string reason, Guid managerId);
    }
}
