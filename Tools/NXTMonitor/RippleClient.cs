using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotPay.Tools.RippleClient
{
    public partial class RippleClient4Net
    {
        protected string serverHost;
        private string secretPhrase;
        private int genesis_timestamp = (int)((new DateTime(2013, 11, 24, 12, 0, 0, DateTimeKind.Utc)) - (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;

        public RippleClient4Net(string server, string secretPhrase)
        {
            this.serverHost = server;
            this.secretPhrase = secretPhrase;
        }

        protected async Task<string> HttpGetAsync(string method, Dictionary<string, string> parameters)
        {
            HttpClient client = new HttpClient();

            var url = this.serverHost + "?requestType=" + method;

            foreach (var para in parameters)
            {
                url += "&" + para.Key + "=" + para.Value.ToString();
            }

            var rep = await client.GetAsync(url);

            return await rep.Content.ReadAsStringAsync();

        }

        protected async Task<string> HttpPostAsync(string method, Dictionary<string, string> parameters)
        {

            parameters.Add("requestType", method);

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(parameters);

                var rep = await client.PostAsync(this.serverHost, content);

                return await rep.Content.ReadAsStringAsync();
            }
        }

        private async Task<T> HttpGet<T>(string method, Dictionary<string, string> parameters)
        {
            string result = await HttpGetAsync(method, parameters); 

            var callError = JsonConvert.DeserializeObject<CallError>(result);

            if (callError != null)
            {
                throw new RippleCallException(callError);
            }

            return JsonConvert.DeserializeObject<T>(result); ;
        }

        private async Task<T> HttpPost<T>(string method, Dictionary<string, string> parameters)
        {
            string result = await HttpPostAsync(method, parameters);

            var callError = JsonConvert.DeserializeObject<CallError>(result);

            if (callError.errorCode != 0)
            {
                throw new RippleCallException(callError);
            }

            return JsonConvert.DeserializeObject<T>(result); ;
        }
    }
}
