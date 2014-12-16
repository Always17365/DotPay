using Newtonsoft.Json;

namespace RippleRPC.Net.Model
{
    public class Marker
    {
        [JsonProperty("ledger")]
        public int Ledger { get; set; }

        [JsonProperty("seq")]
        public int Sequence { get; set; }

    }
}
