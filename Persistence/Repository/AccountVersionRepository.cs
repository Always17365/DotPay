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
    public class AccountVersionRepository : FC.Framework.NHibernate.Repository, IAccountVersionRepository
    {
        public AccountVersionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public AccountVersion FindByDepositIDAndCurrency(int depositID, CurrencyType currency)
        {
            AccountVersion accountVersion = null;
            var modifyType = Convert.ToInt32(AccountModifyType.Deposit.ToString("D") + currency.ToString("D"));

            switch (currency)
            {
                case CurrencyType.CNY:
                    accountVersion = this._session.QueryOver<CNYAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    accountVersion = this._session.QueryOver<BTCAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    accountVersion = this._session.QueryOver<LTCAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    accountVersion = this._session.QueryOver<NXTAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    accountVersion = this._session.QueryOver<IFCAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    accountVersion = this._session.QueryOver<DOGEAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    accountVersion = this._session.QueryOver<STRAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;

                case CurrencyType.FBC:
                    accountVersion = this._session.QueryOver<FBCAccountVersion>()
                                         .Where(av => av.ModifyID == depositID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return accountVersion;
        }


        public void Remove(AccountVersion accountVersion)
        {
            this._session.Delete(accountVersion);
        }

        public AccountVersion FindByWithdrawIDAndCurrency(string withdrawUniqueID, CurrencyType currency)
        {
            AccountVersion accountVersion = null;

            var modifyType = Convert.ToInt32(AccountModifyType.Withdraw.ToString("D") + currency.ToString("D"));
            switch (currency)
            {
                case CurrencyType.CNY:
                    accountVersion = this._session.QueryOver<CNYAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.BTC:
                    accountVersion = this._session.QueryOver<BTCAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    accountVersion = this._session.QueryOver<LTCAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    accountVersion = this._session.QueryOver<NXTAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    accountVersion = this._session.QueryOver<IFCAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    accountVersion = this._session.QueryOver<DOGEAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break; 
                
                case CurrencyType.STR:
                    accountVersion = this._session.QueryOver<STRAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;

                case CurrencyType.FBC:
                    accountVersion = this._session.QueryOver<FBCAccountVersion>()
                                         .Where(av => av.ModifyUniqueID == withdrawUniqueID && av.ModifyType == modifyType)
                                         .SingleOrDefault();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return accountVersion;
        }
    }
}
