using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
    public class TransactionRecordDetail
    {
        public string Account { get; set; }
        public decimal Fee { get; set; }
        public long Flags { get; set; }
        public int LastLedgerSequence { get; set; }
        public int Sequence { get; set; }
        public string SigningPubKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Destination { get; set; }
        public int DestinationTag { get; set; }

        [JsonConverter(typeof(RippleCurrencyValueConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RippleCurrencyValue Amount { get; set; }

        [JsonConverter(typeof(OfferItemConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object TakerGets { get; set; }

        [JsonConverter(typeof(OfferItemConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object TakerPays { get; set; }
        public string TransactionType { get; set; }
        public string TxnSignature { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("inLedger")]
        public int InLedger { get; set; }

        [JsonProperty("ledger_index")]
        public int LedgerIndex { get; set; }

    }
}
