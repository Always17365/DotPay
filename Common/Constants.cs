using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Common
{
    public partial class Constants
    {

        /// <summary>current user key
        /// </summary>
        public const string CurrentUserKey = "__CURRENTUSER_ID_EMAIL";
        public const string TmpUserKey = "__CURRENTUSER_ID_EMAIL_TMP";
        public const string TmpUserVerifyHash= "__CURRENTUSER_HASH_TMP";
        /// <summary>split char is ','
        /// </summary>
        public static char[] DefaultSplitChars { get { return new char[] { ',' }; } }


        /// <summary>split char is ','
        /// </summary>
        public const char DefaultSplitChar = ',';

        /// <summary>the cache key prefix for coin
        /// </summary>
        public const string COIN_PREFIX = "___Coin_";

        /// <summary>the default page item count
        /// </summary>
        public const int DEFAULT_PAGE_COUNT = 10;

        /// <summary>reset password relative expiration minutes 
        /// <remarks>e.g:if user reset password, the user will have 30 minutes to change password</remarks>
        /// </summary>
        public const int RESET_PASSWORD_RELATIVE_EXPIRATION_TIME_MINUTES = 30;

        /// <summary>reset password relative expiration minutes 
        /// <remarks>e.g:if user reset trade password, the user will have 30 minutes to change trade password</remarks>
        /// </summary>
        public const int RESET_TRADE_PASSWORD_RELATIVE_EXPIRATION_TIME_MINUTES = 30;

        public const float SCORE_PERCENT_DEPOSIT_CNY = 1.0F;
        public const float SCORE_PERCENT_DEPOSIT_BTC = 2000.0F;
        public const float SCORE_PERCENT_DEPOSIT_LTC = 20.0F;
        public const float SCORE_PERCENT_DEPOSIT_NXT = 0.5F;
        public const float SCORE_PERCENT_DEPOSIT_IFC = 1 / 5000.0F;
        public const float SCORE_PERCENT_DEPOSIT_DOGE = 1 / 2000.0F;
        public const float SCORE_PERCENT_DEPOSIT_STR = 1 / 100.0F;
        public const float SCORE_PERCENT_DEPOSIT_FBC = 1 / 2000.0F;

        public const int SCORE_PERCENT_TRADE_FIXED = 10;
        public const float SCORE_PERCENT_TRADE_CNY = 1.0F;
        public const float SCORE_PERCENT_TRADE_BTC = 2000.0F;
        public const float SCORE_PERCENT_TRADE_LTC = 20.0F;
        public const float SCORE_PERCENT_TRADE_NXT = 0.5F;
        public const float SCORE_PERCENT_TRADE_IFC = 1 / 5000.0F;
        public const float SCORE_PERCENT_TRADE_DOGE = 1 / 2000.0F;
        public const float SCORE_PERCENT_TRADE_STR = 1 / 100.0F;
        public const float SCORE_PERCENT_TRADE_FBC = 1 / 2000.0F;
    }
}
