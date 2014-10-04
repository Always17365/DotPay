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
    public class LogsInListModel
    {
        public int ID { get; set; }
        public int DomainID { get; set; }
        public int OperatorID { get; set; }
        public string Email { get; set; }
        public string DomainEmail { get; set; }
        public string OperatorEmail { get; set; }
        public int OperateTime { get; set; }
        public string Memo { get; set; }
        public string LoginTime { get; set; }
        public string IP { get; set; }
    }
}
