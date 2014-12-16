using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RippleRPC.Net.Crypto;
using RippleRPC.Net.Crypto.Encodings;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
    public class PaymentSubmitResult
    {
        [JsonProperty("engine_result")]
        public string EngineResult { get; set; }
        [JsonProperty("engine_result_code")]
        public string EngineResultCode { get; set; }
        [JsonProperty("engine_result_message")]
        public string EngineResultMessage { get; set; }

        [JsonProperty("tx_blob")]
        public string TxBlob { get; set; } 
        [JsonProperty("tx_json")]
        public Transaction Transaction { get; set; }
    }
}
