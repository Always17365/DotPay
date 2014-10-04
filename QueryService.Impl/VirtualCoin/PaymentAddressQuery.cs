using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class PaymentAddressQuery : AbstractQuery, IPaymentAddressQuery
    {

        public string GetPaymentAddressByUserID(CurrencyType currency, int userID)
        {
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            var cacheKey = CacheKey.USER_PAYMENT_ADDRESS + currency.ToString() + userID.ToString();
            string address = Cache.Get<string>(cacheKey);

            if (address == null)
            {
                var sql = string.Empty;
                if (currency != CurrencyType.NXT)
                    sql = getPaymentAddress_Sql.FormatWith(currency.ToString());
                else
                    sql = getNXTPaymentAddress_Sql.FormatWith(currency.ToString());

                address = this.Context.Sql(sql)
                                 .Parameter("@userID", userID)
                                 .QuerySingle<string>();

                if (address != null)
                    Cache.Add(cacheKey, address);
            }

            return address;
        }
        public int GetUserIDByPaymentAddress(CurrencyType currency, string paymentAddress)
        {
            Check.Argument.IsNotEmpty(paymentAddress, "paymentAddress");

            var sql = string.Empty;
            if (currency != CurrencyType.NXT)
                sql = getUserIDByPaymentAddress_Sql.FormatWith(currency.ToString());
            else
                sql = getUserIDByNXTPaymentAddress_Sql.FormatWith(currency.ToString());

            var userID = this.Context.Sql(sql)
                             .Parameter("@address", paymentAddress)
                             .QuerySingle<int>();

            return userID;
        }

        #region SQL
        private readonly string getPaymentAddress_Sql =
                                @"SELECT   Address 
                                    FROM   " + Config.Table_Prefix + @"{0}PaymentAddress 
                                   WHERE   UserID=@userID";

        private readonly string getNXTPaymentAddress_Sql =
                                @"SELECT   CONCAT(Address,',',NXTAccountID,',',NXTPublicKey)
                                    FROM   " + Config.Table_Prefix + @"{0}PaymentAddress
                                   WHERE   UserID=@userID";

        private readonly string getUserIDByPaymentAddress_Sql =
                                @"SELECT   IFNULL(UserID,0)
                                    FROM   " + Config.Table_Prefix + @"{0}PaymentAddress
                                   WHERE   Address=@address LIMIT 1";

        private readonly string getUserIDByNXTPaymentAddress_Sql =
                                @"SELECT   IFNULL(UserID,0)
                                    FROM   " + Config.Table_Prefix + @"{0}PaymentAddress
                                   WHERE   Address=@address OR NXTAccountID=@address LIMIT 1";

        #endregion

    }
}
