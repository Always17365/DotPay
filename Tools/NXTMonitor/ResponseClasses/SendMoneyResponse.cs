using Newtonsoft.Json;
using System;

namespace DotPay.Tools.RippleClient
{
    public class SendMoneyResponse
    {
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
    }
}
