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
    public class TokenQuery : AbstractQuery, ITokenQuery
    {
        public int CountByTokenAndUserIDWhichIsNotUsed(string token, int userID, TokenType tokenType)
        {
            Check.Argument.IsNotEmpty(token, "token");
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNull(tokenType, "tokenType");

            var paramters = new object[] { token, userID, (int)tokenType, false };

            return this.Context.Sql(tokenCount_Sql)
                               .Parameters(paramters)
                               .QuerySingle<int>();
        }

        public Tuple<int, int> GetIdAndExpiredAtByTokenAndUserIDWhichIsNotUsed(string token, int userID, TokenType tokenType)
        {
            Check.Argument.IsNotEmpty(token, "token");
            Check.Argument.IsNotNegativeOrZero(userID, "userID");
            Check.Argument.IsNotNull(tokenType, "tokenType");

            var paramters = new object[] { token, userID, (int)tokenType, false };

            var tokenObj = this.Context.Sql(tokenExprie_Sql)
                                    .Parameters(paramters)
                                    .QuerySingle<dynamic>();

            if (tokenObj != null)
            {
                int id = tokenObj.ID;
                int expiredAt = (int)tokenObj.ExpiredAt;
                return new Tuple<int, int>(id, expiredAt);
            }
            else return null;

        }

        private readonly string tokenCount_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"Token 
                                   WHERE   Value=@0 AND UserID=@1 AND Type=@2 AND IsUsed=@3";


        private readonly string tokenExprie_Sql =
                                @"SELECT   ID,IFNULL(ExpiredAt,0)  AS ExpiredAt 
                                    FROM   " + Config.Table_Prefix + @"Token 
                                   WHERE   Value=@0 AND UserID=@1 AND Type=@2 AND IsUsed=@3";


    }
}
