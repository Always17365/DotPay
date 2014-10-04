using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Domain.Repository
{
    public interface IDepositRepository : IRepository
    {
        VirtualCoinDeposit FindByTxIDAndCurrency(string txid, CurrencyType currency);
        VirtualCoinDeposit FindByReceivePaymentTxUniqueIDAndCurrency(string txUnqiueId, CurrencyType currency);
    }
}
