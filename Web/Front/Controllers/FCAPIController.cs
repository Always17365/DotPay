using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FC.Framework;
using DotPay.QueryService;

namespace DotPay.Web.Controllers
{
    public class FCAPIController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        
        public class Depth
        {
            public bool result { get; set; }
            public IEnumerable<decimal[]> asks { get; set; }
            public IEnumerable<decimal[]> bids { get; set; }
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        #region 格式化小数点
        private IEnumerable<decimal[]> ParseData(IEnumerable<DotPay.ViewModel.OrderListModel> orderList, MarketType marketTypeint)
        {
            var result = new List<decimal[]>();
            int priceFixed = 0;
            int volumeFixed = 0;
            if (marketTypeint.BaseCurrency == CurrencyType.CNY)
            {
                switch (marketTypeint.TargetCurrency)
                {
                    case CurrencyType.IFC: { priceFixed = 8; volumeFixed = 3; } break;
                    case CurrencyType.BTC: { priceFixed = 2; volumeFixed = 5; } break;
                    case CurrencyType.LTC: { priceFixed = 2; volumeFixed = 4; } break;
                    case CurrencyType.NXT: { priceFixed = 4; volumeFixed = 4; } break;
                    case CurrencyType.DOGE: { priceFixed = 8; volumeFixed = 3; } break;
                    default: { priceFixed = 2; volumeFixed = 2; } break;
                }
            }
            else if (marketTypeint.BaseCurrency == CurrencyType.BTC || marketTypeint.BaseCurrency == CurrencyType.LTC)
            {
                switch (marketTypeint.TargetCurrency)
                {
                    case CurrencyType.IFC: { priceFixed = 8; volumeFixed = 3; ;} break;
                    case CurrencyType.NXT: { priceFixed = 8; volumeFixed = 3; ;} break;
                    case CurrencyType.DOGE: { priceFixed = 8; volumeFixed = 3; } break;
                    default: { priceFixed = 2; volumeFixed = 2; } break;
                }
            }
            foreach (var temp in orderList)
            {
                result.Add(new decimal[] { decimal.Parse(temp.Price).ToFixed(priceFixed), decimal.Parse(temp.Volume).ToFixed(volumeFixed) });
            }

            return result;
        }
        #endregion
    }
}