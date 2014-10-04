using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.Domain.Events;
using DotPay.Common;
using FC.Framework.Utilities;
using FC.Framework.Repository;
using DotPay.Domain.Exceptions;

namespace DotPay.Domain
{
    public class PaymentAddressFactory
    {
        public static PaymentAddress CreatePaymentAddress(int userID, int accountID, string paymentAddress, CurrencyType currencyType)
        {
            PaymentAddress address;

            switch (currencyType)
            {
                case CurrencyType.BTC:
                    address = new BTCPaymentAddress(userID,accountID, paymentAddress);
                    break;
                case CurrencyType.LTC:
                    address = new LTCPaymentAddress(userID, accountID, paymentAddress);
                    break;
                case CurrencyType.IFC:
                    address = new IFCPaymentAddress(userID, accountID, paymentAddress);
                    break;
                case CurrencyType.NXT:
                    var nxtAddress = paymentAddress.Split(',');
                    var nxtAccountID = Convert.ToUInt64(nxtAddress[0]);
                    var nxtAccountRS = nxtAddress[1];
                    var nxtPublicKey = nxtAddress[2];
                    address = new NXTPaymentAddress(userID, accountID, nxtAccountID, nxtAccountRS,nxtPublicKey);
                    break;
                case CurrencyType.DOGE:
                    address = new DOGEPaymentAddress(userID, accountID, paymentAddress);
                    break;
                case CurrencyType.FBC:
                    address = new FBCPaymentAddress(userID, accountID, paymentAddress);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return address;
        }
    }
}
