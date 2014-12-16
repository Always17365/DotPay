using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
    public class AccountOffer
    {
        [JsonProperty("flags")]
        public long Flags { get; set; }

        [JsonProperty("seq")]
        public int Sequence { get; set; }

        [JsonProperty("taker_gets")]
        [JsonConverter(typeof(OfferItemConverter))]
        public object TakerGets { get; set; }

        [JsonProperty("taker_pays")]
        [JsonConverter(typeof(OfferItemConverter))]
        public object TakerPays { get; set; }

    }
}
