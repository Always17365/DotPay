using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Common;
using Newtonsoft.Json;

namespace Dotpay.Admin.ViewModel
{
    [Serializable]
    public class PagerListModel<T, TRowId> where T : class
    {
        private PagerListModel() { }
        [JsonProperty("recordsFiltered")]
        public int RecordCountFiltered { get; set; }
        [JsonProperty("recordsTotal")]
        public int RecordCount { get; set; }
        [JsonProperty("draw")]
        public int PageNo { get; set; }
        [JsonProperty("data")]
        public IEnumerable<PageListItemModel<T, TRowId>> Data { get; set; }

        public static PagerListModel<T, TRowId> Create(IEnumerable<T> data, int currentPageNo, int recordCount, int recordCountFiltered, Func<T, TRowId> rowIdFunc = null)
        {
            if (data == null) data = new List<T>();

            return new PagerListModel<T, TRowId>()
            {
                PageNo = currentPageNo,
                RecordCount = recordCount,
                RecordCountFiltered = recordCountFiltered,
                Data = rowIdFunc == null ? data.Select(it => new PageListItemModel<T, TRowId>(it))
                                    : data.Select(it => new PageListItemModel<T, TRowId>(it, rowIdFunc(it)))
            };
        }
    }

    public class PageListItemModel<T, TRowId> where T : class
    {
        [JsonProperty("DT_RowId", NullValueHandling = NullValueHandling.Ignore)]
        public TRowId DtRowId { get; set; }
        public T Item { get; set; }

        public PageListItemModel(T model)
        {
            this.Item = model;
        }
        public PageListItemModel(T model, TRowId rowId)
        {
            this.Item = model;
            this.DtRowId = rowId;
        }
    }
}
