using Newtonsoft.Json;
using System;

namespace RippleRPC.Net.Exceptions
{
    [Serializable]
    public class RippleError
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_message")]
        public string Message { get; set; }
    }
}
