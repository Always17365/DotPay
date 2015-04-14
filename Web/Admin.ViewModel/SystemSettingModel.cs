 
using System;

namespace Dotpay.Admin.ViewModel
{
    public abstract class ReceiveRippleTransferSettingViewModel
    {
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal FixedFee { get; set; }
        public decimal FeeRate { get; set; }
        public decimal MinFee { get; set; }
        public decimal MaxFee { get; set; }
    }


    [Serializable]
    public class ToFISettingViewModel : ReceiveRippleTransferSettingViewModel
    {

    }

    [Serializable]
    public class ToDotpaySettingViewModel : ReceiveRippleTransferSettingViewModel
    {
    }

    [Serializable]
    public class SystemSettingViewModel
    {
        public ToFISettingViewModel RippleToFISetting { get; set; }
        public ToDotpaySettingViewModel RippleToDotpaySetting { get; set; } 
    }
}
