using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.Domain;
using DotPay.Domain.Repository;
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

        public WithdrawReceiverBankAccount FindByAccountAndBank(string bankAccount, Bank bank)
        {
            return this._session.QueryOver<WithdrawReceiverBankAccount>()
                                .Where(wrbc => wrbc.BankAccount == bankAccount && wrbc.Bank == bank)
                                .SingleOrDefault(); 
        }
    }
}
