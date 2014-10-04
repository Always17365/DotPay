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
    public class AccountRepository : FC.Framework.NHibernate.Repository, IAccountRepository
    {
        public AccountRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public Account FindByUserIDAndCurrency(int userID, CurrencyType currency)
        {
            var account = default(Account);

            switch (currency)
            {
                case CurrencyType.CNY:
                    account = this._session.QueryOver<CNYAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    account = this._session.QueryOver<BTCAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    account = this._session.QueryOver<LTCAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    account = this._session.QueryOver<IFCAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    account = this._session.QueryOver<NXTAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    account = this._session.QueryOver<DOGEAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    account = this._session.QueryOver<STRAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    account = this._session.QueryOver<FBCAccount>().Where(act => act.UserID == userID).SingleOrDefault();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return account;
        }


        public Account FindByIDAndCurrency(int accountID, CurrencyType currency)
        {
            var account = default(Account);

            switch (currency)
            {
                case CurrencyType.CNY:
                    account = this._session.QueryOver<CNYAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    account = this._session.QueryOver<BTCAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    account = this._session.QueryOver<LTCAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    account = this._session.QueryOver<IFCAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    account = this._session.QueryOver<NXTAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    account = this._session.QueryOver<DOGEAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    account = this._session.QueryOver<STRAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    account = this._session.QueryOver<FBCAccount>().Where(act => act.ID == accountID).SingleOrDefault();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return account;
        }
    }
}
