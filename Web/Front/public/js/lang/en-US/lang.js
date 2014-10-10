var MSG_TIP_Enter_GA_OTP = "Please enter your Google Authentication OTP.";
var MSG_TIP_Enter_SMS_OTP = "Please enter your SMS Authentication OTP.";
var MSG_TIP_Enter_Old_PWD = "Please enter your old password.";
var MSG_TIP_Input_PWD = "Please provide a password.";
var MSG_InvalidPasswordMinLength = "Your password must be at least 6 characters long.";
var MSG_TIP_Enter_Confirm_PWD = "Please enter the same password as above.";
var MSG_TIP_Enter_Old_Trade_PWD = "Please enter your old trade password.";
var MSG_TIP_Enter_Trade_PWD = "Please provide a trade password.";
var MSG_TIP_Enter_Confirm_Trade_PWD = "Please enter the same trade password as above."
var MSG_TIP_Enter_Your_FullName = "Please enter your full name.";
var MSG_TIP_Choose_Id_type = "Please choose identity type";
var MSG_TIP_Enter_Your_Id_No= "Please type your identity no.";
var MSG_TIP_Id_No_Min_Length = "Your identity no. must be at least 6 characters long.";
var MSG_TIP_Id_No_Max_Length = "Your identity no. must be at most 18 characters long .";

var MSG_Please_Login = "Please login to trade"; 
var MSG_Please_Input_Volume = "Please input volume";
var MSG_Please_Input_Price = "Please input price";
var MSG_Volume_Too_Large = "You don't have enough balance to buy"; 
var MSG_Fee_Only_Trade_Close = "";
var MSG_Sell_Price = "Sell Price";
var MSG_Buy_Price = "Buy Price";
var MSG_Sell_Volume = "Sell Volume";
var MSG_Buy_Volume = "Buy Volume";

var LABEL_Confirm_Withdraw = "Are you sure you would like to withdraw this amount?";
var LABEL_Confirm_V_Withdraw = "Are you sure you would like to withdraw this amount?<br /><br />Please ensure the withdraw amount and address are correct before confirming. We will be unable to stop a withdrawal when it has been processed.";
var LABEL_ConfirmOrder = "Are you sure you would like to submit order?";
var LABEL_Confirm = "Please confirm";
var LABEL_AskOrder = "Sell";
var LABEL_BidOrder = "Buy";
var LABEL_Btn_Yes= "Yes";
var LABEL_Btn_No = "No";
var LABEL_Btn_Cancel_Order = "Cancel Order";
var LABEL_INDEX_No_Market_To_Display = "No market to display!";
var LABEL_INDEX_Market = "MARKET";
var LABEL_INDEX_Coin_Name = "COIN NAME";
var LABEL_INDEX_Last_Price = "LAST PRICE";
var LABEL_INDEX_Change = "CHANGE";
var LABEL_INDEX_Volume = "Volume";
var LABEL_INDEX_High = "HIGH";
var LABEL_INDEX_Low = "LOW";
var LABEL_INDEX_First_Bid = "TOP BID";
var LABEL_INDEX_First_ASK = "TOP ASK";
var LABEL_INDEX_Display_Market = "Displaying markets";
var LABEL_Deposit_Display_No_Deposit = "There are no deposits to display";
var LABEL_Deposit_Display_Deposit = "Displaying deposits";
var LABEL_Deposit_Order_Id = "Order ID";
var LABEL_Deposit_Memo = "Memo";
var LABEL_Deposit_Status = "Status";
var LABEL_Deposit_Time = "Time";
var LABEL_Deposit_Fee = "Fee";
var LABEL_Deposit_Amount = "Amount";

var LABEL_Withdraw_Display_No_Withdraw = "There are no withdraws to display";
var LABEL_Withdraw_Display_Withdraw = "Displaying withdraws";
var LABEL_Withdraw_Address_Or_Code = "Address (DotPay Code)";
var LABEL_Withdraw_Order_Id = "Order ID";
var LABEL_Withdraw_Code_Already_Use = "Used";
var LABEL_Withdraw_Status = "Status";
var LABEL_Withdraw_Time = "Time";
var LABEL_Withdraw_Fee = "Fee";
var LABEL_Withdraw_Amount = "Amount";
var LABEL_Withdraw_State_Pending = "Pending";
var LABEL_Withdraw_State_Failed = "Failed";
var LABEL_Withdraw_State_Canceled = "Canceled";
var LABEL_Withdraw_State_Completed = "Completed";

var LABEL_Order_Display_No = "There are no orders to display";
var LABEL_Order_Display = "Displaying orders";
var LABEL_Order_Market = "Market";
var LABEL_Order_Coin_Name = "Coin Name";
var LABEL_Order_Order_Type= "Type";
var LABEL_Order_Price = "Price";
var LABEL_Order_Volume = "Volume";
var LABEL_Order_Amount = "Amount";
var LABEL_Order_Cancel = "Cancel";

var LABEL_Trade_Display_No = "There are no trades to display";
var LABEL_Trade_Display = "Displaying trades";
var LABEL_Trade_Market = "Market";
var LABEL_Trade_Coin_Name = "Coin Name";
var LABEL_Trade_Time = "Time";
var LABEL_Trade_Order_Type = "Type";
var LABEL_Trade_Price = "Price";
var LABEL_Trade_Volume = "Volume";
var LABEL_Trade_Amount = "Amount";

var LABEL_Login_Time= "Login Time";
var LABEL_Login_Displaying = "Displaying login hisotries";
var LABEL_Login_Displaying_No = "There are no login hisotries to display";

var LABEL_SideBar_Markets = "Markets";
var LABEL_SideBar_Coin = "Coin";
var LABEL_SideBar_Price = "Price";
var LABEL_SideBar_Change = "Change";
var LABEL_SideBar_Volume = "Volume"; 


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