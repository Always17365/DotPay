using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using FC.Framework;
using DotPay.Common;
using DotPay.ViewModel;
using FC.Framework.Utilities;

namespace DotPay.QueryService.Impl
{
    public class SmsInterfaceQuery : AbstractQuery, ISmsInterfaceQuery
    {
        public SmsInterfaceModel GetSmsInterface()
        {
            return this.Context.Sql(get_sms_interface_sql)
                               .QuerySingle<SmsInterfaceModel>();
        }

        public bool ExistSmsInterface()
        {
            return this.Context.Sql(count_sms_interface_sql)
                               .QuerySingle<int>() > 0;
        }

        private readonly string get_sms_interface_sql =
                                @"SELECT   * 
                                    FROM   " + Config.Table_Prefix + @"SmsInterface";

        private readonly string count_sms_interface_sql =
                                @"SELECT   COUNT(*)
                                    FROM   " + Config.Table_Prefix + @"SmsInterface";
    }
}
