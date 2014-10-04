
using FullCoin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FullCoin.QueryService
{
    public interface ITokenQuery
    {
        int CountByTokenAndUserIDWhichIsNotUsed(string token, int userID, TokenType tokenType);
    }
}
