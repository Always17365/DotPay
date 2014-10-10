var MSG_TIP_Enter_GA_OTP = "请输入谷歌身份验证码";
var MSG_TIP_Enter_SMS_OTP = "请输入短信验证码";
var MSG_TIP_Enter_Old_PWD = "请输入旧密码";
var MSG_TIP_Input_PWD = "请输入密码";
var MSG_InvalidPasswordMinLength = "密码长度不能少于6位";
var MSG_TIP_Enter_Confirm_PWD = "请再次确认密码";
var MSG_TIP_Enter_Old_Trade_PWD = "请输入旧的资金密码";
var MSG_TIP_Enter_Trade_PWD = "请输入资金密码";
var MSG_TIP_Enter_Confirm_Trade_PWD = "请再次确认资金密码"
var MSG_TIP_Enter_Your_FullName = "请输入您的全名";
var MSG_TIP_Choose_Id_type = "请选择证件类型";
var MSG_TIP_Enter_Your_Id_No = "请输入证件号";
var MSG_TIP_Id_No_Min_Length = "证件号长度不可少于6位";
var MSG_TIP_Id_No_Max_Length = "证件号长度不可超过18位";

var MSG_Please_Login = "请登录后交易"; 
var MSG_Please_Input_Volume = "请输入交易量";
var MSG_Please_Input_Price = "请输入单价";
var MSG_Volume_Too_Large = "超出可买入金额";
var MSG_Fee_Only_Trade_Close = "成交才收";
var MSG_Sell_Price = "卖出价格";
var MSG_Buy_Price = "买入价格";
var MSG_Sell_Volume = "卖出数量";
var MSG_Buy_Volume = "买入数量";
var LABEL_ConfirmOrder = "确认要提交订单吗？";
var LABEL_Confirm = "请确认";
var LABEL_AskOrder = "卖出";
var LABEL_BidOrder = "买入";
var LABEL_Btn_Yes = "确定";
var LABEL_Btn_No = "取消";
var LABEL_Btn_Cancel_Order = "撤销订单";  
var LABEL_INDEX_No_Market_To_Display = "有没可显示的市场!";
var LABEL_INDEX_Market = "交易市场";
var LABEL_INDEX_Coin_Name = "交易币种";
var LABEL_INDEX_Last_Price = "最新价格";
var LABEL_INDEX_Change = "跌涨幅";
var LABEL_INDEX_Volume = "成交量";
var LABEL_INDEX_High = "最高价";
var LABEL_INDEX_Low = "最低价";
var LABEL_INDEX_First_Bid = "买一";
var LABEL_INDEX_First_ASK = "卖一";
var LABEL_INDEX_Display_Market = "显示交易市场";

var LABEL_Deposit_Display_No_Deposit = "没有可显示的充值记录";
var LABEL_Deposit_Display_Deposit = "充值记录";
var LABEL_Deposit_Order_Id = "订单号";
var LABEL_Deposit_Memo = "备注";
var LABEL_Deposit_Status = "状态";
var LABEL_Deposit_Time = "时间";
var LABEL_Deposit_Fee = "手续费";
var LABEL_Deposit_Amount = "金额";

var LABEL_Withdraw_Display_No_Withdraw = "没有可显示的提现记录";
var LABEL_Withdraw_Display_Withdraw = "提现记录";
var LABEL_Withdraw_Address_Or_Code = "提现地址 (充值码)";
var LABEL_Withdraw_Order_Id = "订单号";
var LABEL_Withdraw_Code_Already_Use = "已使用";
var LABEL_Withdraw_Status = "状态";
var LABEL_Withdraw_Time = "时间";
var LABEL_Withdraw_Fee = "手续费";
var LABEL_Withdraw_Amount = "金额";
var LABEL_Withdraw_State_Pending = "处理中";
var LABEL_Withdraw_State_Failed = "失败";
var LABEL_Withdraw_State_Canceled = "已取消";
var LABEL_Withdraw_State_Completed = "已完成";

var LABEL_Order_Display_No = "没有可显示的挂单记录";
var LABEL_Order_Display = "挂单记录";
var LABEL_Order_Market = "市场";
var LABEL_Order_Coin_Name = "交易币种";
var LABEL_Order_Order_Type = "类型";
var LABEL_Order_Price = "单价";
var LABEL_Order_Volume = "挂单量";
var LABEL_Order_Amount = "挂单金额";
var LABEL_Order_Cancel = "撤销";

var LABEL_Trade_Display_No = "没有可显示的成交记录";
var LABEL_Trade_Display = "成交记录";
var LABEL_Trade_Market = "市场"; 
var LABEL_Trade_Time = "时间";
var LABEL_Trade_Order_Type = "类型";
var LABEL_Trade_Price = "单价";
var LABEL_Trade_Volume = "成交量";
var LABEL_Trade_Amount = "成交金额";

var LABEL_Login_Time = "登录时间";
var LABEL_Login_Displaying = "登录日志";
var LABEL_Login_Displaying_No = "没有可显示的成交记录";

var LABEL_SideBar_Markets = "市场";
var LABEL_SideBar_Coin = "币种";
var LABEL_SideBar_Price = "价格";
var LABEL_SideBar_Change = "日涨跌";
var LABEL_SideBar_Volume = "成交量";


String.format = function () {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}