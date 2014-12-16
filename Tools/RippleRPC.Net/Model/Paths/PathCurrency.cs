using Newtonsoft.Json;
using System;

namespace RippleRPC.Net.Model.Paths
{
    [Serializable]
    public class PathCurrency
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("issuer",NullValueHandling=NullValueHandling.Ignore)] 
        public string Issuer { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("type_hex")]
        public string TypeHex { get; set; }
    }
}
