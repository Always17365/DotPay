using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;

namespace Dotpay.AdminQueryService
{
    public interface ISystemSettingQuery
    {
        //Task<SystemSettingViewModel> GetSystemSetting();
        Task<ToFISettingViewModel> GetToFISetting();
        Task<ToDotpaySettingViewModel> GetToDotpaySetting();
    }
}
