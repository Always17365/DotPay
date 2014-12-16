using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RippleRPC.Net.Model
{
//    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class FieldInformation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }

        [JsonConverter(typeof(RippleValueConverter))]
        public double Balance { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? Flags { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? OwnerCount { get; set; }

        public int Sequence { get; set; }
    }
}
