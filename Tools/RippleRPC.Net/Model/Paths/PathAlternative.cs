using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RippleRPC.Net.Model.Paths
{
    [Serializable]
    public class PathAlternative
    {
        [JsonProperty("paths_canonical")]
        [JsonConverter(typeof(PathConverter))]
        public List<List<object>> CanonicalPaths { get; set; }

        [JsonProperty("paths_computed")]
        [JsonConverter(typeof(PathConverter))]
        public List<List<object>> ComputedPaths { get; set; }

        [JsonProperty("source_amount")]
        [JsonConverter(typeof(RippleCurrencyValueConverter))]
        public RippleCurrencyValue SourceAmount { get; set; }
    }
}
