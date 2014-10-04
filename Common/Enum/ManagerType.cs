using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// site manager type
    /// </summary>
    public enum ManagerType
    {
        /// <summary>
        /// 运维人员
        /// </summary>
        [EnumDescription("运维")]
        SystemManager = 1,
        /// <summary>
        /// 转账记录员
        /// </summary>
        [EnumDescription("转账记录员")]
        FinanceBankTransferRecorder = 2,
        /// <summary>
        /// 充值审核员
        /// </summary>
        [EnumDescription("充值审核员")]
        FinanceDepositVerifier = 4,
        /// <summary>
        /// 充值员
        /// </summary>
        [EnumDescription("充值员")]
        DepositOfficer = 8,
        /// <summary>
        /// 监察员
        /// </summary> 
        [EnumDescription("运行监察员")]
        Monitor = 16,
        [EnumDescription("超级管理员")]
        SuperManager = 32,
        /// <summary>
        /// 提现转账审核员
        /// </summary>
        [EnumDescription("提现转账审核员")]
        FinanceWithdrawTransferGenerator = 64,
        /// <summary>
        /// 转账操作员
        /// </summary>
        [EnumDescription("转账操作员")]
        FinanceWithdrawTransferOfficer = 128,
        /// <summary>
        /// 提现财务主管
        /// </summary>
        [EnumDescription("提现财务主管")]
        WithdrawMonitor = 256,
        /// <summary>
        /// 客服
        /// </summary>
        [EnumDescription("客服")]
        CustomerService = 512,
        /// <summary>
        /// 操盘账户
        /// </summary>
        [EnumDescription("操盘手")]
        Trader = 1024,
        /// <summary>
        /// 编辑
        /// </summary>
        [EnumDescription("编辑")]
        Editor = 2048
    }
}
