using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class WithdrawReceiverAccountRepository : FC.Framework.NHibernate.Repository, IWithdrawReceiverAccountRepository
    {
        public WithdrawReceiverAccountRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public WithdrawReceiverAccount FindByAccountAndBank(string bankAccount, PayWay payway)
        {
            return this._session.QueryOver<WithdrawReceiverAccount>()
                                .Where(wrbc => wrbc.BankAccount == bankAccount && wrbc.PayWay == payway)
                                .SingleOrDefault(); 
        }
    }
}
