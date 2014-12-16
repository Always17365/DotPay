using Newtonsoft.Json;
using System;
using System.Runtime;
namespace RippleRPC.Net.Model.Paths
{
    [Serializable]
    public class PathAccount
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("type_hex")]
        public string TypeHex { get; set; }
    }
}
