using Newtonsoft.Json;

namespace RippleRPC.Net.Model
{
    public class TransactionSigned
    {
        [JsonProperty("tx_blob")]
        public string TxBlob { get; set; }

        [JsonProperty("tx_json")]
        public Transaction Transaction { get; set; }

        [JsonProperty("validated")]
        public bool Validated { get; set; }
    }
}
