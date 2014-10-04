using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentData;
using DotPay.ViewModel;
using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;

namespace DotPay.QueryService.Impl
{
    public class CityQuery : AbstractQuery, ICityQuery
    {

        public IEnumerable<CityModel> GetAllProvince()
        {
            return this.Context.Sql(getAllProvince_sql).QueryMany<CityModel>();
        }

        public IEnumerable<CityModel> GetCityByProvinceID(int provinceID)
        {
            Check.Argument.IsNotNegativeOrZero(provinceID, "provinceID");

            return this.Context.Sql(getCityByProvinceID_Sql)
                               .Parameter("@provinceID", provinceID)
                               .QueryMany<CityModel>();
        }

        #region SQL
        private readonly string getAllProvince_sql =
                                @"SELECT   ID, Name
                                    FROM   " + Config.Table_Prefix + @"Province ";

        private readonly string getCityByProvinceID_Sql =
                                @"SELECT   ID, Name
                                    FROM   " + Config.Table_Prefix + @"City
                                   WHERE   FatherID = @provinceID";
        #endregion

    }
}
