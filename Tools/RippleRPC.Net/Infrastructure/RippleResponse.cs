using Newtonsoft.Json;
using RippleRPC.Net.Exceptions;

namespace RippleRPC.Net.Infrastructure
{
    public class RippleResponse
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        //[JsonProperty("result")]
        //public T Result { get; set; }
        public RippleError Error { get; set; }

        [JsonProperty("ledger_current_index")]
        public long LedgerCurrentIndex { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }

    }

}
