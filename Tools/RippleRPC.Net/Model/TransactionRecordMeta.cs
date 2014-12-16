using System.Collections.Generic;
using Newtonsoft.Json;

namespace RippleRPC.Net.Model
{
    /// <summary>
    /// Returns meta information about the transaction.
    /// I don't know how useful this information is, so I took the easy way out and returned it all as dynamic.
    /// If there is a use case where you think it would be valuable to have it strongly typed, please let me know!
    /// </summary>
    public class TransactionRecordMeta
    {
        public List<Node> AffectedNodes { get; set; }
        public int TransactionIndex { get; set; }
        public string TransactionResult { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RippleCurrencyValue DeliveredAmount { get; set; }

    }
}
