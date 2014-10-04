using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;

namespace DotPay.ViewModel
{
    public class SmsInterfaceModel
    {
        public int ID { get; set; }
        public SmsInterfaceType SmsType { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
