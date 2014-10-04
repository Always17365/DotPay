using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DotPay.Common;
using FC.Framework;

namespace DotPay.ViewModel
{
    public class CityModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class Bankoutlets
    {
        public int ID { get; set; }
        public string CNBank { get { return this.Bank.GetDescription(); } }
        public Bank Bank { get; set; }
        public string Cnprovince { get; set; }
        public int ProvincelID { get; set; }
        public string Cncity { get; set; }
        public int CityID { get; set; }
        public bool IsDelete { get; set; }
        public string DeleteBy { get; set; }
        public string Name { get; set; }
    }
}
