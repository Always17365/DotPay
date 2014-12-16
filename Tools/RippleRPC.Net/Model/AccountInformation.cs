using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{    
    public class AccountInformation
    {
        public string Account { get; set; }
        
        [JsonConverter(typeof(RippleValueConverter))]
        public double Balance { get; set; }
        public long Flags { get; set; }
        public string LedgerEntryType { get; set; }
        public int OwnerCount { get; set; }
        public string PreviousTxnID { get; set; }
        public int PreviousTxnLgrSeq { get; set; }
        public int Sequence { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }
    }
}
