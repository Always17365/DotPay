using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
    public class LedgerSummary
    {
        [JsonProperty(PropertyName = "closed")]
        public ClosedLedger Closed { get; set; }

        [JsonProperty(PropertyName = "open")]
        public OpenLedger Open { get; set; }

    }

    public class ClosedLedger
    {
        [JsonProperty(PropertyName = "ledger")]
        public ClosedLedgerDetail Ledger { get; set; }
    }

    public class OpenLedger
    {
        [JsonProperty(PropertyName = "ledger")]
        public OpenLedgerDetail Ledger { get; set; }
    }

    public class OpenLedgerDetail
    {
        [JsonProperty(PropertyName = "closed")]
        public bool Closed { get; set; }
        
        [JsonProperty(PropertyName = "ledger_index")]
        public string LedgerIndex { get; set; }

        [JsonProperty(PropertyName = "parent_hash")]
        public string ParentHash { get; set; }

        [JsonProperty(PropertyName = "seqNum")]
        public int SequenceNumber { get; set; }

    }

    public class ClosedLedgerDetail : OpenLedgerDetail
    {
        [JsonProperty(PropertyName = "accepted")]
        public bool Accepted { get; set; }

        [JsonProperty(PropertyName = "account_hash")]
        public string AccountHash { get; set; }

        [JsonProperty(PropertyName = "close_time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CloseTime { get; set; }

        [JsonProperty(PropertyName = "close_time_human")]
        public string CloseTimeHuman { get; set; }

        [JsonProperty(PropertyName = "close_time_resolution")]
        public int CloseTimeResolution { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "ledger_hash")]
        public string LedgerHash { get; set; }

        [JsonProperty(PropertyName = "totalCoins")]
        public long TotalCoins { get; set; }

        [JsonProperty(PropertyName = "total_coins")]
        public long TotalCoins2 { get; set; }

        [JsonProperty(PropertyName = "transaction_hash")]
        public string TransactionHash { get; set; }
    }


    public class LedgerDetail
    {
        [JsonProperty(PropertyName = "accepted", NullValueHandling = NullValueHandling.Ignore)] 
        public bool Accepted { get; set; }

        [JsonProperty(PropertyName = "account_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountHash { get; set; }

        [JsonProperty(PropertyName = "close_time", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CloseTime { get; set; }

        [JsonProperty(PropertyName = "close_time_human", NullValueHandling = NullValueHandling.Ignore)]
        public string CloseTimeHuman { get; set; }

        [JsonProperty(PropertyName = "close_time_resolution", NullValueHandling = NullValueHandling.Ignore)]
        public int CloseTimeResolution { get; set; }

        [JsonProperty(PropertyName = "closed")]
        public bool Closed { get; set; }

        [JsonProperty(PropertyName = "hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "ledger_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string LedgerHash { get; set; }

        [JsonProperty(PropertyName = "ledger_index")]
        public string LedgerIndex { get; set; }

        [JsonProperty(PropertyName = "parent_hash")]
        public string ParentHash { get; set; }

        [JsonProperty(PropertyName = "seqNum")]
        public int SequenceNumber { get; set; }

        [JsonProperty(PropertyName = "totalCoins", NullValueHandling = NullValueHandling.Ignore)]
        public long TotalCoins { get; set; }

        [JsonProperty(PropertyName = "total_coins", NullValueHandling = NullValueHandling.Ignore)]
        public double TotalCoins2 { get; set; }

        [JsonProperty(PropertyName = "transaction_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string TransactionHash { get; set; }

    }    

}
