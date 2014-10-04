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
    public class MarketeOrderListModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int TargetCurrency { get; set; }
        public int BaseCurrency { get; set; }
        public decimal Volume { get; set; }
        public decimal OriginVolume { get; set; }
        public string CNType { get { return LangHelpers.Lang(this.Type.GetDescription()); } }
        public OrderType Type { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int CreateAt { get; set; }
    }
    public class OrderListModel
    {
        public int ID { get; set; }
        public string Price { get; set; }
        public string CNTargetCurrency { get { return LangHelpers.Lang(this.TargetCurrency.GetDescription()); } }
        public string CNBaseCurrency { get { return LangHelpers.Lang(this.BaseCurrency.GetDescription()); } }
        public CurrencyType TargetCurrency { get; set; }
        public CurrencyType BaseCurrency { get; set; }
        public string TargetCurrencyCode { get { return this.TargetCurrency.ToString(); } }
        public string BaseCurrencyCode { get { return this.BaseCurrency.ToString(); } }
        public string Volume { get; set; }
        public string VolumePercent { get; set; }
        public string CNType { get { return LangHelpers.Lang(this.Type.GetDescription()); } }
        public OrderType Type { get; set; }
        public string Amount { get; set; }
        public int CreateAt { get; set; }
    }
}
