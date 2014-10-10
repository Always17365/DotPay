using Newtonsoft.Json;
using System;

namespace DotPay.Tools.RippleClient
{
    public class GetAccountTransactionIdsResponse
    { 
        [JsonProperty("transactionIds")]
        public string[] TransactionIds { get; set; }
    }
}
