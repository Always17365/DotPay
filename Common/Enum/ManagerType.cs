using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotpay.Common
{
    public enum ManagerType
    {
        SuperUser = 1,
        MaintenanceManager = 2,
        DepositManager = 3,
        TransferManager = 4,
        ToFiTransferManager =5
    }
}
