using Newtonsoft.Json;
using System;

namespace RippleRPC.Net.Model
{
    public class RippleCurrency
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("issuer", NullValueHandling = NullValueHandling.Ignore)]
        public string Issuer { get; set; }
    }

    public class RippleCurrencyValue : RippleCurrency
    {
        [JsonProperty("value")]
        public string _Value { get; set; }
        [JsonIgnore]
        public decimal Value
        {
            get
            {
                var result = 0M;
                if (!string.IsNullOrEmpty(this._Value))
                    decimal.TryParse(this._Value, out result);
                return result;
            }
        }
    }
}
