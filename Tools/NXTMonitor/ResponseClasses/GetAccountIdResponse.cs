using Newtonsoft.Json;
using System;

namespace DotPay.Tools.RippleClient
{
    public class GetAccountIdResponse
    {
        [JsonProperty("account")]
        public string AccountId { get; set; }

        [JsonProperty("accountRS")]
        public string AccountRS { get; set; }
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }
        
    }
}
