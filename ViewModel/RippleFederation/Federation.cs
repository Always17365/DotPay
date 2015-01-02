using DotPay.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;

namespace DotPay.ViewModel
{
    #region Federation
    [Serializable]
    public class FederationResponse : FederationError
    {
        [JsonProperty("federation_json", NullValueHandling = NullValueHandling.Ignore)]

        public OutsideGatewayFederationInfo UserAccount { get; set; }
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
    public class OutsideGatewayFederationInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("destination_address", NullValueHandling = NullValueHandling.Ignore)]
        public string DestinationAddress { get; set; }

        [JsonProperty("destination_tag", NullValueHandling = NullValueHandling.Ignore)]
        public int? DestinationTag { get; set; }

        [JsonProperty("dt", NullValueHandling = NullValueHandling.Ignore)]
        public int? DT { get { return DestinationTag; } }

        [JsonProperty("quote_url", NullValueHandling = NullValueHandling.Ignore)]
        public string QuoteUrl { get; set; }

        [JsonProperty("currencies")]
        public IEnumerable<RippleCurrency> AcceptCurrencys { get; set; }
        [JsonProperty("extra_fields")]
        public IEnumerable<ExtraFiled> ExtraFields { get; set; }

        [JsonProperty("expires", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Expires { get; set; }
    }

    [Serializable]
    public class ExtraFiled
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("hint")]
        public string Hint { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ExtraSelectFiledOption> Options { get; set; }

    }

    [Serializable]
    public class ExtraSelectFiledOption
    {
        public ExtraSelectFiledOption(string label, string value, bool selected = false, bool disabled = false)
        {
            this.Label = label;
            this.Value = value;
            this.Selected = selected;
            this.Disabled = disabled;
        }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("selected")]
        public bool Selected { get; set; }
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
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
    #endregion

    #region Quote
    [Serializable]
    public class QuoteError
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty("error_message", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public QuoteRequest OriginRequest { get; set; }
    }
    [Serializable]
    public class QuoteRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        public string __dot_use_this_amount { get; set; }

        [JsonIgnore]
        public RippleAmount Amount { get { return RippleAmount.Parse(this.__dot_use_this_amount); } }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("alipay_account", NullValueHandling = NullValueHandling.Ignore)]
        public string AlipayAccount { get; set; }
        [JsonProperty("tenpay_account", NullValueHandling = NullValueHandling.Ignore)]
        public string TenpayAccount { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

    }

    [Serializable]
    public class QuoteResponse : QuoteError
    {
        [JsonProperty("timestamp")]
        public int Timestamp { get { return DateTime.Now.ToUnixTimestamp(); } }

        [JsonProperty("quote", NullValueHandling = NullValueHandling.Ignore)]

        public QuoteInfo QuoteInfo { get; set; }
    }


    [Serializable]
    public class QuoteInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("destination_address")]
        public string DestinationAddress { get; set; }

        [JsonProperty("address")]
        public string Address { get { return this.DestinationAddress; } }
        [JsonProperty("destination_tag")]
        public int DestinationTag { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonProperty("send")]
        public IEnumerable<RippleAmount> Send { get; set; }
        [JsonProperty("expires")]
        public int Expires { get { return DateTime.Now.ToUnixTimestamp(); } }
    }

    /// <summary>
    /// 只能用作非XRP的货币
    /// </summary>
    public class RippleAmount
    {
        public RippleAmount(decimal value, string currency)
        {
            this.Value = value;
            this.Currency = currency;
        }
        public RippleAmount(decimal value, string issuer, string currency)
        {
            this.Value = value;
            this.Issuer = issuer;
            this.Currency = currency;
        }
        public static RippleAmount Parse(string human_amount)
        {
            var rippleAmount = default(RippleAmount);
            var split_char = '/';
            if (human_amount.Contains("/"))
            {
                var strs = human_amount.Split(new char[] { split_char }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 2)
                {
                    var amount = 0M;
                    if (decimal.TryParse(strs[0], out amount))
                    {
                        rippleAmount = new RippleAmount(amount, strs[1]);
                    }
                }
            }

            return rippleAmount;
        }

        [JsonProperty("value")]
        public string __dot_use_this_Value { get { return this.Value.ToString(); } }

        [JsonIgnore]
        public decimal Value { get; set; }
        [JsonProperty("issuer", NullValueHandling = NullValueHandling.Ignore)]
        public string Issuer { get; set; }

        [JsonProperty("currency", NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }
    }
    #endregion
}
