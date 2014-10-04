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
    public class VipSettingListModel
    { 
        public int ID { get; set; }
        public int VipLevel { get; set; }
        public int ScoreLine { get; set; }
        public decimal Discount { get; set; }
        public int VoteCount { get; set; }
        public int CreateAt { get; set; }
        public int UpdateAt { get; set; }
    }
}
