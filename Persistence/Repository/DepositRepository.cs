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
    public class DepositRepository : FC.Framework.NHibernate.Repository, IDepositRepository
    {
        public DepositRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public VirtualCoinDeposit FindByTxIDAndCurrency(string txid, CurrencyType currency)
        {
            var deposit = default(VirtualCoinDeposit);

            switch (currency)
            {
                case CurrencyType.BTC:
                    deposit = this._session.QueryOver<BTCDeposit>().Where(d => d.TxID==txid).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    deposit = this._session.QueryOver<LTCDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    deposit = this._session.QueryOver<IFCDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    deposit = this._session.QueryOver<NXTDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    deposit = this._session.QueryOver<DOGEDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    deposit = this._session.QueryOver<STRDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    deposit = this._session.QueryOver<FBCDeposit>().Where(d => d.TxID == txid).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }


        public VirtualCoinDeposit FindByReceivePaymentTxUniqueIDAndCurrency(string txUnqiueId, CurrencyType currency)
        {
            var deposit = default(VirtualCoinDeposit);

            switch (currency)
            { 
                case CurrencyType.STR:
                    deposit = this._session.QueryOver<STRDeposit>().Where(d => d.ReceivePaymentTxUniqueID == txUnqiueId).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return deposit;
        }
    }
}
