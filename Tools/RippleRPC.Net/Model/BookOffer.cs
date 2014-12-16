using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
    public class BookOffer
    {
        public string Account { get; set; }
        public string BookDirectory { get; set; }
        public string BookNode { get; set; }
        public long Flags { get; set; }
        public string LedgerEntryType { get; set; }
        public string OwnerNode { get; set; }
        public string PreviousTxnID { get; set; }
        public int PreviousTxnLgrSeq { get; set; }
        public int Sequence { get; set; }

        [JsonConverter(typeof(OfferItemConverter))]
        public object TakerGets { get; set; }

        [JsonConverter(typeof(OfferItemConverter))]
        public object TakerPays { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

    }
}
