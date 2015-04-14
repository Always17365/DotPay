var HandleFiFormInit= function() {
    window.ParsleyValidator.setLocale('zh_cn');
    $('#tofisettingForm').parsley();
    $("#tofisettingForm").submit(function () {
        console.log("tofisettingForm")
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
    window.ParsleyValidator.setLocale('zh_cn');
    $('#toDotpaySettingForm').parsley();
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