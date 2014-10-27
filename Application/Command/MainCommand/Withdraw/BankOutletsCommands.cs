using FC.Framework;
using FC.Framework.Utilities;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command
{
    #region Create Bank Outlets
    public class CreateBankOutlets : FC.Framework.Command
    {
        public CreateBankOutlets(Bank bank, int provinceID, int cityID, string bankName,int currentUserID)
        {
            Check.Argument.IsNotNegativeOrZero((int)bank, "bank");
            Check.Argument.IsNotNegativeOrZero(provinceID, "provinceID");
            Check.Argument.IsNotNegativeOrZero(cityID, "cityID");
            Check.Argument.IsNotEmpty(bankName, "bankName");

            this.Bank = bank;
            this.ProvinceID = provinceID;
            this.CityID = cityID;
            this.BankName = bankName;
            this.ByUserID = currentUserID;
        }

        public Bank Bank { get; private set; }
        public int ProvinceID { get; private set; }
        public int CityID { get; private set; }
        public string BankName { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion

    #region Remove Bank Outlets
    public class RemoveBankOutlets : FC.Framework.Command
    {
        public RemoveBankOutlets(int bankOutletsId, int byUserID)
        {
            Check.Argument.IsNotNegativeOrZero(bankOutletsId, "bankOutletsId");
            Check.Argument.IsNotNegativeOrZero(byUserID, "byUserID");

            this.BankOutletsId = bankOutletsId;
            this.ByUserID = byUserID;
        }

        public int BankOutletsId { get; private set; }
        public string Memo { get; private set; }
        public int ByUserID { get; private set; }
    }
    #endregion
}
