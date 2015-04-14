var handleLoginPageChangeBackground = function () {

    $('[data-click="change-bg"]').live('click', function () {
        var targetImage = '[data-id="login-cover-image"]';
        var targetImageSrc = $(this).find('img').attr('src');
        var targetImageHtml = '<img src="' + targetImageSrc + '" data-id="login-cover-image" />';

        $('.login-cover-image').prepend(targetImageHtml);
        $(targetImage).not('[src="' + targetImageSrc + '"]').fadeOut('slow', function () {
            $(this).remove();
        });
        $('[data-click="change-bg"]').closest('li').removeClass('active');
        $(this).closest('li').addClass('active');
    });
};
var handleLoginForm = function () {
    $('#loginForm').parsley();
    $("#loginForm").submit(function () {
        $.post("/account/login", $(this).serialize(), function (result, status) {
            if (result.Code === 1) { 
                $("#loginMessage").html('<div id="loginMessage" class="alert alert-info fade in m-b-15"><strong>登陆成功</strong><p><a href="/index">3秒后自动跳转</a><span class="close" data-dismiss="alert">×</span></div>');
                setTimeout(function () { window.location.href = "/index" }, 3000)
            } else {  
                $("#loginMessage").html('<div class="alert alert-danger fade in m-b-15"><strong>登陆失败!</strong>' + result.Message + '<span class="close" data-dismiss="alert">×</span></div>');
            }
        });
        return false;
    });
};

var Login = function () {
    "use strict";
    return {
        //main function
        init: function () {
            handleLoginForm();
            handleLoginPageChangeBackground();
        }
    };
}();