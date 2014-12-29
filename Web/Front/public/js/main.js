var smsUseFor = { SwitchLoginTwofactor: 1, BindMobile: 2, Withdraw: 3, ModifyLoginPassword: 4, ModifyTradePwd: 5, ResetTradePwd: 6, Login2FA: 7, ResetLoginPwd: 8 }
function sendsms(src, useFor) {
    var btn_text = $(src).text();
    var btn = $(src);
    $.ajax({
        type: 'POST',
        url: '/sendsms',
        data: "usefor=" + useFor,
        success: function (data) {
            if (data.Code == 1) {
                var interval = 120;
                var intervalSms = window.setInterval(function () {
                    if (interval-- > 0) {
                        btn.attr("disabled", true).text(btn_text + "(已发送" + interval + ")").addClass("disabled");
                    }
                    else {
                        btn.attr("disabled", false).text(btn_text).removeClass("disabled");
                        window.clearInterval(intervalSms);
                    }
                }, 1000);
            } else {
                commonFormFailCallback(data);
            }
        }
    });
}
function commonFormCallback(data) {
    if (data.Code == 1) {
        commonFormSuccessCallback(data);
    } else {
        commonFormFailCallback(data);
    }
}
function commonFormSuccessCallback(data) {
    $('#spanSuccessBox').html(data.Message);
    if ($('#sucessBox:visible')) {
        $('#sucessBox').hide()
        $('#sucessBox').show(500);
    }
    else {
        $('#sucessBox').show();
    }
    $('#failBox').hide();
    $('html, body').animate({
        scrollTop: $("#sucessBox").offset().top - 95
    }, 1000);
}

function commonFormFailCallback(data) {
    $('#spanFailBox').html(data.Message);
    if ($('#failBox:visible')) {
        $('#failBox').hide()
        $('#failBox').show(500);
    }
    else {
        $('#failBox').show();
    }
    $('#sucessBox').hide();
    $('html, body').animate({
        scrollTop: $("#failBox").offset().top - 95
    }, 1000);
}

function switchLang(lang) {
    $.cookie('lanaguage', lang, { path: "/" });
    console.log("lang");
    window.location.reload();
}

Number.prototype.div = function (arg) {
    return accDiv(this, arg);

}
function accMul(arg1, arg2) {

    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();

    try { m += s1.split(".")[1].length } catch (e) { }

    try { m += s2.split(".")[1].length } catch (e) { }

    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m)

}

Number.prototype.mul = function (arg) {
    return accMul(arg, this);

}

function accAdd(arg1, arg2) {

    var r1, r2, m, c;

    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }

    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }

    c = Math.abs(r1 - r2);
    m = Math.pow(10, Math.max(r1, r2))
    if (c > 0) {
        var cm = Math.pow(10, c);
        if (r1 > r2) {
            arg1 = Number(arg1.toString().replace(".", ""));
            arg2 = Number(arg2.toString().replace(".", "")) * cm;
        }
        else {
            arg1 = Number(arg1.toString().replace(".", "")) * cm;
            arg2 = Number(arg2.toString().replace(".", ""));
        }
    }
    else {
        arg1 = Number(arg1.toString().replace(".", ""));
        arg2 = Number(arg2.toString().replace(".", ""));
    }
    return (arg1 + arg2) / m

}

Number.prototype.add = function (arg) {
    ///	<summary>
    ///	    加法
    ///	</summary>
    return accAdd(arg, this);

}
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}