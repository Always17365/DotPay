using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace RippleRPC.Net.Model.Paths
{
    [Serializable]
    public class PathSummary
    {
        [JsonProperty("alternatives")]
        public List<PathAlternative> Alternatives { get; set; }

        [JsonProperty("destination_account")]
        public string DestinationAccount { get; set; }

        [JsonProperty("destination_amount")]
        public RippleCurrencyValue DestinationAmount { get; set; }
    }
}
