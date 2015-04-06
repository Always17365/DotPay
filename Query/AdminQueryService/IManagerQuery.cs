using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Admin.ViewModel;

namespace Dotpay.AdminQueryService
{
    public interface IManagerQuery
    {
        //Task<SystemSettingViewModel> GetSystemSetting();
        Task<Guid> GetManagerIdByLoginName(string loginName); 
    }
}
