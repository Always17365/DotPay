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
    public class WithdrawRepository : FC.Framework.NHibernate.Repository, IWithdrawRepository
    {
        public WithdrawRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public IWithdraw FindByIdAndCurrency(int id, Common.CurrencyType currency)
        {
            var withdraw = default(IWithdraw);

            switch (currency)
            {
                case CurrencyType.CNY:
                    withdraw = this._session.QueryOver<CNYWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    withdraw = this._session.QueryOver<BTCWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    withdraw = this._session.QueryOver<LTCWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    withdraw = this._session.QueryOver<IFCWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    withdraw = this._session.QueryOver<NXTWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    withdraw = this._session.QueryOver<DOGEWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    withdraw = this._session.QueryOver<STRWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;

                case CurrencyType.FBC:
                    withdraw = this._session.QueryOver<FBCWithdraw>().Where(wd => wd.ID == id).SingleOrDefault();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return withdraw;
        }


        public IWithdraw FindByUniqueIdAndCurrency(string uniqueID, CurrencyType currency)
        {
            var withdraw = default(IWithdraw);

            switch (currency)
            {
                case CurrencyType.CNY:
                    withdraw = this._session.QueryOver<CNYWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    withdraw = this._session.QueryOver<BTCWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    withdraw = this._session.QueryOver<LTCWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    withdraw = this._session.QueryOver<IFCWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    withdraw = this._session.QueryOver<NXTWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    withdraw = this._session.QueryOver<DOGEWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    withdraw = this._session.QueryOver<STRWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    withdraw = this._session.QueryOver<FBCWithdraw>().Where(wd => wd.UniqueID == uniqueID).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return withdraw;
        }
    }
}
