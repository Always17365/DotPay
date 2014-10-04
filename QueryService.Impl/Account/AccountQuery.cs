using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Text;
using FluentData;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Common;
using System.Threading.Tasks;
using FC.Framework.Utilities;

namespace DotPay.QueryService.Impl
{
    public class AccountQuery : AbstractQuery, IAccountQuery
    {
        private object _locker = new object();
        public AccountModel GetAccountByUserID(int userID, CurrencyType currency)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            return this.GetAccountsByUserID(userID)
                       .SingleOrDefault(ac => ac.Code == currency.ToString());
        }

        public IEnumerable<AccountModel> GetAccountsByUserID(int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");

            var user_accounts_cache_key = CacheKey.USER_ACCOUNTS + userID.ToString();

            var accounts = Cache.Get<List<AccountModel>>(user_accounts_cache_key);

            if (accounts == null)
            {
                var currencies = IoC.Resolve<ICurrencyQuery>().GetAllCurrencies();
                accounts = new List<AccountModel>();

                Enum.GetValues(typeof(CurrencyType))
                    .Cast<CurrencyType>()
                    .AsParallel().ForAll(currency =>
                    {
                        var currencyModel = currencies.SingleOrDefault(c => c.ID == (int)currency);

                        if (currencyModel != null)
                        {
                            var account = default(AccountModel);
                            var table_name = currency.ToString() + "Account";
                            var sql = account_tmp_Sql.FormatWith(table_name);
                            var queryResult = this.Context.Sql(sql).Parameter("@userID", userID).QueryMany<AccountModel>();

                            if (queryResult == null || queryResult.Count == 0) account = new AccountModel() { ID = 0, Balance = 0, Locked = 0 };
                            else account = queryResult.SingleOrDefault();

                            account.CurrencyID = currencyModel.ID;
                            account.Code = currencyModel.Code;
                            account.Name = currencyModel.Name;
                            account.Sum = account.Balance + account.Locked;

                            lock (_locker)
                            {
                                accounts.Add(account);
                            }
                        }
                    });
                Cache.Add(user_accounts_cache_key, accounts, new TimeSpan(1, 0, 0));
            }

            return accounts.OrderBy(ac => ac.CurrencyID);
        }

        private readonly string account_tmp_Sql =
                         @"SELECT   ID,Balance,Locked
                             FROM   " + Config.Table_Prefix + @"{0}
                            WHERE   UserID=@userID
                            ";
    }
}
