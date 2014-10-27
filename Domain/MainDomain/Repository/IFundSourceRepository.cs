using FC.Framework.Repository;
using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.MainDomain.Repository
{
    public interface IFundSourceRepository : IRepository
    {
        FundSource FindByDepositID(int depositID); 
    }
}
