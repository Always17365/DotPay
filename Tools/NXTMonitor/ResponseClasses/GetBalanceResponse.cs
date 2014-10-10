using Newtonsoft.Json;
using System;

namespace DotPay.Tools.RippleClient
{
    public class GetBalanceResponse
    {
        [JsonProperty("guaranteedBalanceNQT")]
        public double GguaranteedBalanceNQT { get; set; }

        [JsonProperty("balanceNQT")]
        public double BalanceNQT { get; set; }

        [JsonProperty("effectiveBalanceRipple")]
        public double EffectiveBalanceRipple { get; set; }

        [JsonProperty("unconfirmedBalanceNQT")]
        public double UnconfirmedBalanceNQT { get; set; }

        [JsonProperty("forgedBalanceNQT")]
        public double ForgedBalanceNQT { get; set; } 
    }
}
