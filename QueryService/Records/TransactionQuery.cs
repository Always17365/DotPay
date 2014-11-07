using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotPay.ViewModel;


namespace DotPay.QueryService
{
    public interface ITransactionQuery
    {
        int CountUserTransactions(int amountIncomeStart = -1, int amountIncomeEnd = -1,
                                  int amountOutputStart = -1, int amountOutputEnd = -1,
                                  int start_date = -1, int end_date = -1, bool includeDeposit = true,
                                  bool includeWithdraw = true, bool includeOther = true);

        IEnumerable<TransactionRecord> GetUserTransactions(int amountIncomeStart = -1, int amountIncomeEnd = -1,
                                  int amountOutputStart = -1, int amountOutputEnd = -1,
                                  int start_date = -1, int end_date = -1, bool includeDeposit = true,
                                  bool includeWithdraw = true, bool includeOther = true);
    }
}
