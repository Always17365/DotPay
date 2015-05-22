/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleForgetLoginPasswordForm = function (lang) {
    $('#formForgetLoginPassword').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        //err: { container: 'tooltip' },
        fields: {
            email: {
                validators: {
                    notEmpty: {
                        message: Language.loginNameRequired
                    }, emailAddress: {
                        message: Language.emailAdressRegexNotMatch
                    }
                }
            } 
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/forget/loginpwd/sendmail", $(this).serialize()+"&lang="+lang, function (result, status) {
            if (result.Code === 1) {
                window.location.href = "/forget/loginpwd/result?rid=" + result.Data;
            }  else {
                $("#resetMessage").html('<div class="alert alert-danger fade in m-b-15">' + result.Message + '<span class="close" data-dismiss="alert">×</span></div>');
            }
        });
    });
};
var handleRegisterForm = function () {
    $('#formSignup').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        //err: { container: 'tooltip' },
        fields: {
            email: {
                validators: {
                    notEmpty: {
                        message: Language.emailAddressRequired
                    },
                    emailAddress: {
                        message: Language.emailAdressRegexNotMatch
                    },
                    remote: {
                        type: 'POST',
                        url: '/validate/email',
                        message: Language.registerEmailExist,
                        delay: 500
                    }
                }
            },
            loginPassword: {
                validators: {
                    notEmpty: {
                        message: Language.loginPasswordRequited
                    }, stringLength: {
                        min: 6,
                        max: 30,
                        message: Language.registerPasswordLengthNotMatch
                    }
                }
            },
            confirmPassword: {
                validators: {
                    notEmpty: {
                        message: Language.registerConfirmPasswordRequited
                    },
                    identical: {
                        field: 'loginPassword',
                        message: Language.registerConfirmPasswordNotEqual
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/signup", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#signupMessage").html('<div id="loginMessage" class="alert alert-info fade in m-b-15"><strong>' + Language.registerSuccess + '</strong><p>' + Language.activeEmailHasSend + '</p></div>');
            } else {
                console.log(1);
                $("#signupMessage").html('<div class="alert alert-danger fade in m-b-15"><strong>' + Language.registerFail + '</strong>' + result.Message + '<span class="close" data-dismiss="alert">×</span></div>');
            }
        });
    });

};

var ForgetPassword = function () {
    "use strict";
    return {
        initForgetLoginPasswordForm: function (lang) {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleForgetLoginPasswordForm(lang);
            });
        },
        handleRegisterForm: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleRegisterForm();
            });
        },
        handleReactiveEmailSend: function (loginName) {
            $.post("/resend", { loginName: loginName }, function (result, status) {
                if (result.Code === 1) {
                    $("#signinMessage").html('<div id="loginMessage" class="alert alert-success fade in m-b-15"><strong>' + Language.resendSuccess + '</strong><p></div>');
                } else {
                    $("#signinMessage").html('<div class="alert alert-danger fade in m-b-15">' + result.Message + '<span class="close" data-dismiss="alert">×</span></div>');
                }
            });
        }
    };
}();