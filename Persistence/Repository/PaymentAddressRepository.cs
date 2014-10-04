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
    public class PaymentAddressRepository : FC.Framework.NHibernate.Repository, IPaymentAddressRepository
    {
        public PaymentAddressRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public PaymentAddress FindByAddressAndCurrency(string address, CurrencyType currency)
        {
            PaymentAddress paymentAddress = null;

            switch (currency)
            {
                case CurrencyType.BTC:
                    paymentAddress = this._session.QueryOver<BTCPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                case CurrencyType.DOGE:
                    paymentAddress = this._session.QueryOver<DOGEPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                case CurrencyType.IFC:
                    paymentAddress = this._session.QueryOver<IFCPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                case CurrencyType.LTC:
                    paymentAddress = this._session.QueryOver<LTCPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                case CurrencyType.NXT:
                    paymentAddress = this._session.QueryOver<NXTPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                case CurrencyType.FBC:
                    paymentAddress = this._session.QueryOver<FBCPaymentAddress>().Where(pa => pa.Address == address).SingleOrDefault();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return paymentAddress;
        }

    }
}
