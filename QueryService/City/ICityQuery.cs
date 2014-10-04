
using DotPay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.QueryService
{
    public interface ICityQuery
    {
        IEnumerable<CityModel> GetAllProvince();
        IEnumerable<CityModel> GetCityByProvinceID(int provinceID);
    }
}
