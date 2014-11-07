using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using FC.Framework;
using DotPay.ViewModel;
using DotPay.Common;
using System.Threading.Tasks;
using FC.Framework.Utilities;

namespace DotPay.QueryService.Impl
{
    public class CurrencyQuery : AbstractQuery, ICurrencyQuery
    {
        private object _locker = new object();

        public int GetCurrencyCountBySearch(string code, string name)
        {
            var resultCount = 0;
            var currencies = this.GetAllCurrencies();

            resultCount = currencies.Count(c => c.Code.Contains(code.NullSafe()) && c.Name.Contains(name));

            return resultCount;
        }
        public CurrencyListModel GetCurrency(CurrencyType currency)
        {
            return this.GetAllCurrencies().Single(c => c.Code == currency.ToString());
        }
        public IEnumerable<CurrencyListModel> GetCurrenciesBySearch(string code, string name, int page, int pageCount)
        {
            Check.Argument.IsNotNegativeOrZero(page, "page");
            Check.Argument.IsNotNegativeOrZero(pageCount, "pageCount");

            var currencies = this.GetAllCurrencies();

            return currencies.Where(c => c.Code.Contains(code.NullSafe()) && c.Name.Contains(name))
                             .Skip((page - 1) * pageCount)
                             .Take(pageCount);
        }

        public Tuple<bool, bool> ExistCurrency(string code, string name)
        {
            Check.Argument.IsNotEmpty(code, "code");
            Check.Argument.IsNotEmpty(name, "name");

            var currencies = this.GetAllCurrencies();
            int existNameCount = 0, existCodeCount = 0;


            existNameCount = currencies != null ? currencies.Count(c => c.Name == name.NullSafe()) : 0;
            existCodeCount = currencies != null ? currencies.Count(c => c.Code == code.NullSafe()) : 0;

            return new Tuple<bool, bool>(existCodeCount > 0, existNameCount > 0);
        }

        public IEnumerable<CurrencyListModel> GetAllCurrencies()
        {
            var currencies = Cache.Get<IEnumerable<CurrencyListModel>>(CacheKey.CURRENCY_LIST);
            if (Config.Debug) currencies = null;
            if (currencies == null)
            {
                lock (_locker)
                {
                    currencies = Cache.Get<IEnumerable<CurrencyListModel>>(CacheKey.CURRENCY_LIST);
                    if (Config.Debug) currencies = null;
                    if (currencies == null)
                    {
                        currencies = this.Context.Sql(currencies_Sql).QueryMany<CurrencyListModel>();
                        if (currencies.Count() > 0)
                            Cache.Add(CacheKey.CURRENCY_LIST, currencies);
                    }
                }
            }

            return currencies;
        }

        private readonly string currencies_Sql =
                         @"SELECT   ID,Code,Name,WithdrawFeeRate,
                                    WithdrawFixedFee,WithdrawOnceLimit,WithdrawDayLimit,WithdrawVerifyLine,
                                    IsEnable,IsLocked,CreateAt,WithdrawOnceMin
                             FROM   " + Config.Table_Prefix + @"Currency
                            WHERE   IsEnable=1
                            ";
    }
}
