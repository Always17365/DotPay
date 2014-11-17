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
    public class PreRegistratorQuery : AbstractQuery, IPreRegistratorQuery
    {
        public bool ExistRegisterEmail(string email)
        { 

            var results = this.Context.Sql(getExistRegisterEmail_Sql)
                                        .Parameter("@email", email)
                                        .QuerySingle<int>();

            return results>0?true:false;
        }
        bool ExistRegisterEmailWithToken(string email, string token)
        {
            var results = this.Context.Sql(getExistRegisterEmail_Sql)
                                       .Parameter("@email", email)
                                       .Parameter("@token", token)
                                       .QuerySingle<int>();

            return results > 0 ? true : false;
        }


        private readonly string getExistRegisterEmail_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"{0}preregistration   
                                   WHERE   Email = @email";

        private readonly string getExistRegisterEmailWithToken_Sql =
                                @"SELECT    COUNT(*)
                                    FROM    " + Config.Table_Prefix + @"{0}preregistration   
                                   WHERE   Email = @email
                                     AND   EmailValidateToken = @token";
    }
}
