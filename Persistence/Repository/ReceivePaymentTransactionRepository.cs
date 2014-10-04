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
    public class ReceivePaymentTransactionRepository : FC.Framework.NHibernate.Repository, IReceivePaymentTransactionRepository
    {
        public ReceivePaymentTransactionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public ReceivePaymentTransaction FindByIDAndCurrency(int receivePaymentTransactionID, CurrencyType currency)
        {
            ReceivePaymentTransaction paymentTx = null;

            switch (currency)
            {
                case CurrencyType.BTC:
                    paymentTx = this._session.QueryOver<BTCReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    paymentTx = this._session.QueryOver<DOGEReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    paymentTx = this._session.QueryOver<IFCReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    paymentTx = this._session.QueryOver<LTCReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    paymentTx = this._session.QueryOver<NXTReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    paymentTx = this._session.QueryOver<STRReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    paymentTx = this._session.QueryOver<FBCReceivePaymentTransaction>().Where(tx => tx.ID == receivePaymentTransactionID).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return paymentTx;
        }

        public ReceivePaymentTransaction FindByTxIDAndCurrency(string txid, CurrencyType currency)
        {
            ReceivePaymentTransaction paymentTx = null;

            switch (currency)
            {
                case CurrencyType.BTC:
                    paymentTx = this._session.QueryOver<BTCReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    paymentTx = this._session.QueryOver<DOGEReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    paymentTx = this._session.QueryOver<IFCReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    paymentTx = this._session.QueryOver<LTCReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    paymentTx = this._session.QueryOver<NXTReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.STR:
                    paymentTx = this._session.QueryOver<STRReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    paymentTx = this._session.QueryOver<FBCReceivePaymentTransaction>().Where(tx => tx.TxID == txid).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return paymentTx;
        }
    }
}
