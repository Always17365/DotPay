using System;
using Newtonsoft.Json;

namespace DotPay.Tools.RippleClient
{
    public class GetTransactionResponse
    {
        [JsonProperty("sender")]
        public string Sender { get; set; }
        [JsonProperty("senderRS")]
        public string SenderRS { get; set; }
        [JsonProperty("feeNQT")]
        public decimal FeeNQT { get; set; }
        [JsonProperty("amountNQT")]
        public decimal AmountNQT { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
        [JsonProperty("referencedTransaction")]
        public string ReferencedTransaction { get; set; }
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }
        [JsonProperty("subtype")]
        public string Subtype { get; set; }
        [JsonProperty("block")]
        public string Block { get; set; }
        [JsonProperty("blockTimestamp")]
        public string BlockTimestamp { get; set; }
        [JsonProperty("attachment ")]
        public Attachment Attachment { get; set; }
        [JsonProperty("senderPublicKey")]
        public string SenderPublicKey { get; set; }
        [JsonProperty("type")]
        public TransactionType Type { get; set; }
        [JsonProperty("deadline")]
        public int Deadline { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }
        [JsonProperty("recipient")]
        public UInt64 Recipient { get; set; }
        [JsonProperty("recipientRS")]
        public string RecipientRS { get; set; }
        [JsonProperty("fullHash")]
        public string FullHash { get; set; }
        [JsonProperty("signatureHash")]
        public string SignatureHash { get; set; }
        [JsonProperty("hash")]
        public string hash { get; set; }
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
    }

    public class Attachment
    {
        public string Alias { get; set; }
        public string Uri { get; set; }
    }

    public enum TransactionType
    {
        Ordinary = 0,
        Alias = 1,
        Asset = 2
    }

    public enum TransactionAssetSubType
    {
        AssetIssuance = 0,
        AssetTransfer = 1,
        AskOrderPlacement = 2,
        BidOrderPlacement = 3,
        AskOrderCancellation = 4,
        BidOrderCancellation = 5
    }
}
