using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RippleRPC.Net.Exceptions;
using RippleRPC.Net.Infrastructure;
using RippleRPC.Net.Model;
using RippleRPC.Net.Model.Paths;
using WebSocket4Net;
using System.Linq;

namespace RippleRPC.Net
{
    public static class DateTimeExtension
    {
        private static readonly DateTime MinDate = new DateTime(1970, 1, 1);
        private static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 999);

        public static bool IsValid(this DateTime target)
        {
            return (target >= MinDate) && (target <= MaxDate);
        }

        /// <summary>
        /// convert datetime to unix timestamp
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int ToUnixTimestamp(this DateTime target)
        {
            return (int)((target.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }

        /// <summary>
        /// convert unix timestamp to local datetime
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(this int target)
        {
            DateTime dtDateTime = new DateTime(621355968000000000 + (long)target * (long)10000000, DateTimeKind.Utc);

            return dtDateTime.ToLocalTime();
        }

        /// <summary>
        /// convert unix timestamp to local datetime
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ToUtcDateTime(this int target)
        {
            DateTime dtDateTime = new DateTime(621355968000000000 + (long)target * (long)10000000, DateTimeKind.Utc);

            return dtDateTime;
        }
    }
}
