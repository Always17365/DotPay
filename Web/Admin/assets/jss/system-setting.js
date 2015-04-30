var HandleFiFormInit= function() { 
    $("#tofisettingForm").submit(function () {
        $.post("/ajax/systemsetting/tofi/save", $(this).serialize(), function (result, status) {
            if (result.Code === 1)
                Notification.notice("保存成功", "");
            else
                Notification.notice("保存失败", result.Message);
        });
        return false;
    }); 
}
var HandleDotpayFormInit = function () {
    $("#toDotpaySettingForm").submit(function () {
        $.post("/ajax/systemsetting/todotpay/save", $(this).serialize(), function (result, status) {
            if (result.Code === 1)
                Notification.notice("保存成功", "");
            else
                Notification.notice("保存失败", result.Message);
        });
        return false;
    });
}

var SystemSetting = function () {
    "use strict";
    return {
        //main function
        fiFormInit: function () {
            HandleFiFormInit();
        },
        dotpayFormInit: function(title, text) {
            HandleDotpayFormInit();
        }
    };
}();