using FC.Framework.NHibernate;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.MainDomain;
using DotPay.MainDomain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.Persistence.Repository
{
    public class InboundTransferToThirdPartyPaymentTxRepository : FC.Framework.NHibernate.Repository, IInboundTransferToThirdPartyPaymentTxRepository
    {
        public InboundTransferToThirdPartyPaymentTxRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }

        public InboundTransferToThirdPartyPaymentTx FindTransferTxByIDAndPayway(int transferTxID, Common.PayWay payway)
        {
            var result = default(InboundTransferToThirdPartyPaymentTx);

            switch (payway)
            {
                case Common.PayWay.Alipay:
                    result = this._session.QueryOver<ToAlipayTransferTransaction>().Where(tx => tx.ID == transferTxID).SingleOrDefault();
                    break;
                case Common.PayWay.Tenpay:
                    result = this._session.QueryOver<ToTenpayTransferTransaction>().Where(tx => tx.ID == transferTxID).SingleOrDefault();
                    break;
                default:
                    result = this._session.QueryOver<ToBankTransferTransaction>().Where(tx => tx.ID == transferTxID).SingleOrDefault();
                    break;
            }

            return result;
        }

        public InboundTransferToThirdPartyPaymentTx FindTransferTxByTxIDAndPayway(string txid, Common.PayWay payway)
        {
            var result = default(InboundTransferToThirdPartyPaymentTx);

            switch (payway)
            {
                case Common.PayWay.Alipay:
                    result = this._session.QueryOver<ToAlipayTransferTransaction>().Where(tx => tx.TxId == txid).SingleOrDefault();
                    break;
                case Common.PayWay.Tenpay:
                    result = this._session.QueryOver<ToTenpayTransferTransaction>().Where(tx => tx.TxId == txid).SingleOrDefault();
                    break;
                default:
                    result = this._session.QueryOver<ToBankTransferTransaction>().Where(tx => tx.TxId == txid).SingleOrDefault();
                    break;
            }

            return result;
        }
    }
}
