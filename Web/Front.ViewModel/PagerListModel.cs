using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dotpay.ViewModel
{
    [Serializable]
    public class PagerListModel<T> where T : class
    {
        private PagerListModel() { }
        [JsonProperty("recordsFiltered")]
        public int RecordCountFiltered { get; set; }
        [JsonProperty("recordsTotal")]
        public int RecordCount { get; set; }
        [JsonProperty("draw")]
        public int PageNo { get; set; }
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }

        public static PagerListModel<T> Create(IEnumerable<T> data, int currentPageNo, int recordCount, int recordCountFiltered)
        {
            if (data == null) data = new List<T>();

            return new PagerListModel<T>()
            {
                PageNo = currentPageNo,
                RecordCount = recordCount,
                RecordCountFiltered = recordCountFiltered,
                Data = data
            };
        }
    } 
}
