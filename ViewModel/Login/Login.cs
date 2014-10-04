using DotPay.Common;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;

namespace DotPay.ViewModel
{
    public class GroupListModel
    {
        public string title { get; set; }
        public List<GroupItemsModel> items { get; set; }
        public int[] Jurisdiction { get; set; }

    }
    public class GroupItemsModel
    {
        public string name { get; set; }
        public string url { get; set; }
        public int[] Jurisdiction { get; set; }
    }
}
