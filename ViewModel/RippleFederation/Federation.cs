using DotPay.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.ViewModel
{
    [Serializable]
    public class FederationResponse : FederationError
    {
        [JsonProperty("federation_json")]
        public OutsideGatewayUserInfo UserAccount { get; set; }
    }

    [Serializable]
    public class FederationError
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty("error_message", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public FederationRequest OriginRequest { get; set; }
    }

    [Serializable]
    public class OutsideGatewayUserInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("destination_address")]
        public string DestinationAddress { get; set; }

        [JsonProperty("destination_tag")]
        public int DestinationTag { get; set; }

        [JsonProperty("currencies")]
        public IEnumerable<RippleCurrency> AcceptCurrencys { get; set; }

        [JsonProperty("expires", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Expires { get; set; }
    }

    [Serializable]
    public class RippleCurrency
    {
        [JsonProperty("currency")]
        public string Symbol { get; set; }

        [JsonProperty("issuer", NullValueHandling = NullValueHandling.Ignore)]
        public string Issuer { get; set; }
    }

    [Serializable]
    public class FederationRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }
}
