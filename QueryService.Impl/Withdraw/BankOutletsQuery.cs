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
    public class BankOutletsQuery : AbstractQuery, IBankOutletsQuery
    {
        public IEnumerable<WithdrawBankOutletsModel> GetBankoutletsByProvinceIDAndCityID(Bank bank, int provinceID, int cityID)
        {
            Check.Argument.IsNotNegativeOrZero((int)bank, "bank");
            Check.Argument.IsNotNegativeOrZero(provinceID, "provinceID");
            Check.Argument.IsNotNegativeOrZero(cityID, "cityID");

            return this.Context.Sql(getBankoutletsByProvinceIDAndCityID_Sql)
                               .Parameter("@provinceID", provinceID)
                               .Parameter("@cityID", cityID)
                               .Parameter("@bank", bank)
                               .QueryMany<WithdrawBankOutletsModel>();
        }

        public int GetBankInfoCountBySearch(int? bankType, int? province, int? city)
        {
            var paramters = new object[] { bankType.HasValue ? bankType.Value : 0, province.HasValue ? province.Value : 0, city.HasValue ? city.Value : 0, };

            var result = this.Context.Sql(getBankInfoCountBySearch_Sql)
                                   .Parameters(paramters)
                                   .QuerySingle<int>();

            return result;
        }


        public IEnumerable<DotPay.ViewModel.Bankoutlets> GetBankInfoBySearch(int? bankType, int? province, int? city, int page, int pageCount)
        {
            var paramters = new object[] { bankType.HasValue ? bankType.Value : 0, province.HasValue ? province.Value : 0, city.HasValue ? city.Value : 0, (page - 1) * pageCount, pageCount };

            var result = this.Context.Sql(getBankInfoBySearch_Sql)
                                   .Parameters(paramters)
                                   .QueryMany<Bankoutlets>();

            return result;
        }
        #region SQL
        private readonly string getBankoutletsByProvinceIDAndCityID_Sql =
                                @"SELECT   ID, Name
                                    FROM   " + Config.Table_Prefix + @"BankOutlets
                                   WHERE   ProvinceID = @provinceID
                                     AND   CityID=@cityID AND Bank=@bank";

        private readonly string getBankInfoCountBySearch_Sql =
                                @"SELECT   COUNT(*) 
                                    FROM   " + Config.Table_Prefix + @"bankoutlets
                                   WHERE   (@0=0 OR Bank = @0)
                                     AND   (@1=0 OR ProvinceID = @1)
                                     AND   (@2=0 OR CityID = @2)
                                     AND   (IsDelete=0)";

        private readonly string getBankInfoBySearch_Sql =
                                @"SELECT   t1.ID, t1.Bank,t2.Name as cnprovince,t3.Name as cncity,t1.IsDelete, t1.DeleteBy, t1.Name  
                                    FROM   " + Config.Table_Prefix + @"bankoutlets  t1 
                               LEFT JOIN   " + Config.Table_Prefix + @"province t2 on t1.ProvinceID=t2.ID 
                               LEFT JOIN   " + Config.Table_Prefix + @"city t3 on t1.CityID=t3.ID
                                   WHERE   (@0=0 OR t1.Bank = @0)
                                     AND   (@1=0 OR t1.ProvinceID = @1)
                                     AND   (@2=0 OR t1.CityID = @2)
                                     AND   (IsDelete=0)";

        #endregion
    }
}
