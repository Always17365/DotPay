
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ITokenQuery
    {
        int CountByTokenAndUserIDWhichIsNotUsed(string token, int userID, TokenType tokenType);
        Tuple<int,int> GetIdAndExpiredAtByTokenAndUserIDWhichIsNotUsed(string token, int userID, TokenType tokenType);
    }
}
