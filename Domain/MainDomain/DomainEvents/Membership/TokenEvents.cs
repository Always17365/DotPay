using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Events
{
    #region TokenGeneroted event
    public class TokenGenerated : DomainEvent
    {
        public TokenGenerated(int userID,string token, TokenType tokenType, DateTime expirationTime)
        {
            this.UserID = userID;
            this.Token = token;
            this.TokenType = tokenType;
            this.ExpirationTime = expirationTime;
        }
        public int UserID { get; private set; }
        public string Token { get; private set; }
        public TokenType TokenType { get; private set; }
        public DateTime ExpirationTime { get; private set; }  
    }
    #endregion

    #region TokenUsed event
    public class TokenUsed : DomainEvent
    {
        public TokenUsed(string token, TokenType tokenType)
        {
            this.Token = token;
            this.TokenType = tokenType;
        }
        public string Token { get; private set; }
        public TokenType TokenType { get; private set; }
    }
    #endregion
}
