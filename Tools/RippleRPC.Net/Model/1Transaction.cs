//using System;
//using System.Collections.Generic;
//using System.Text;
//using Newtonsoft.Json;
//using RippleRPC.Net.Crypto;
//using RippleRPC.Net.Crypto.Encodings;
//using Newtonsoft.Json.Converters;

//namespace RippleRPC.Net.Model
//{
//    public class Transaction
//    {
//        [JsonProperty]
//        public string TransactionType { get; set; }

//        [JsonProperty]
//        public string Account { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public string SigningPubKey { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public string TxnSignature { get; set; } 

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public int DestinationTag { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public int SourceTag { get; set; }


//        [JsonProperty]
//        public string Destination { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public TransactionFlags Flags { get; set; }

//        [JsonConverter(typeof(PathConverter))]
//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public List<object> Paths { get; set; }

//        [JsonConverter(typeof(RippleCurrencyValueConverter))]
//        [JsonProperty]
//        public RippleCurrencyValue Amount { get; set; }

//        [JsonProperty]
//        public int Fee { get; set; }

//        [JsonIgnore]
//        public int Sequence { get; set; }

//        [JsonConverter(typeof(RippleCurrencyValueConverter))]
//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public RippleCurrencyValue SendMax { get; set; }

//        [JsonIgnore]
//        public bool Signed { get { return !string.IsNullOrEmpty(Hash); } }
//        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]  
//        public string Hash { get; set; }
//        [JsonProperty("tx_blob", NullValueHandling = NullValueHandling.Ignore)]
//        public string TxBlob { get; set; }

//        public void Sign(string secret)
//        {
//            string s = ToJson();
//            byte[] bytes = Seed.PassPhraseToSeedBytes(secret);
//            KeyPair keyPair = (KeyPair)Seed.CreateKeyPair(bytes);
//            byte[] transaction = Encoding.ASCII.GetBytes(s);
//            byte[] signedTransaction = keyPair.Sign(transaction);
//            TxBlob = B16.ToString(signedTransaction);
//        }

//        public string ToJson()
//        {
//            var amount_json = string.Empty;
//            if (this.Amount != null)
//            {
//                //convert amount to json
//                if (this.Amount.Currency.ToUpper() != "XRP")
//                    amount_json = JsonConvert.SerializeObject(this.Amount);
//                //convert xrp amount to ripple drops
//                else
//                    amount_json = (this.Amount.Value * 1000000).ToString();
//            }
//            var transaction = "{" +
//            "\"TransactionType\": \"" + this.TransactionType.ToString() + "\", " +
//            "\"Account\": \"" + this.Account + "\"," +
//            "\"Destination\": \"" + this.Destination + "\"," +
//            "\"Amount\": \"" + amount_json + "\", " +
//            "\"Fee\": \"10\"}";
//            return transaction;
//        }
//    }

//    public enum TransactionFlags : uint
//    {
//        tfFullyCanonicalSig = 0x80000000,
//        tfNoDirectRipple = 0x00010000,
//        tfPartialPayment = 0x00020000,
//    }
//}
