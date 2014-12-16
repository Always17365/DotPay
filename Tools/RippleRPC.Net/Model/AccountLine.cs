using Newtonsoft.Json;

namespace RippleRPC.Net.Model
{
    public class AccountLine
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        
        [JsonProperty("balance")]     
        public double Balance { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("limit")]
        public string Limit { get; set; }

        [JsonProperty("limit_peer")]
        public string LimitPeer { get; set; }

        [JsonProperty("no_ripple")]
        public bool NoRipple { get; set; }

        [JsonProperty("quality_in")]
        public int QualityIn { get; set; }

        [JsonProperty("quality_out")]
        public int QualityOut { get; set; }

        [JsonProperty("no_ripple_peer")]
        public bool? NoRipplePeer { get; set; }
    }
}
