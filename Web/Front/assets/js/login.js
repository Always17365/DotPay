/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleLoginForm = function () {
    $("#btnIndexLogin").click(function () {
        $("#signinDialog").modal("show");
        $("#formSignin")[0].reset();
        $("#signinMessage").html("");
    });
    $('#formSignin').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        //err: { container: 'tooltip' },
        fields: {
            loginName: {
                validators: {
                    notEmpty: {
                        message: Language.loginNameRequired
                    }
                }
            },
            password: {
                validators: {
                    notEmpty: {
                        message: Language.loginPasswordRequited
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/signin", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#signupMessage").html('<div id="loginMessage" class="alert alert-success fade in m-b-15"><strong>' + Language.loginSuccess + '</strong><p></div>');
                setTimeout(function () { window.location.href = "/i" }, 1500)
            } else if (result.Code === -2) {
                $("#signinMessage").html('<div id="loginMessage" class="alert alert-info fade in m-b-15">' + Language.loginUnactive + '<strong><a href="#" onclick="LoginAndRegister.handleReactiveEmailSend(\'' + result.Message + '\');"> ' + Language.resend + '</a></strong>' + '<p></div>');
            } else if (result.Code === -3) {
                $("#signinMessage").html('<div id="loginMessage" class="alert alert-info fade in m-b-15">' + Language.loginAccountLocked + '<p></div>');
            } else {
                $("#signinMessage").html('<div class="alert alert-danger fade in m-b-15">' + result.Message + '<span class="close" data-dismiss="alert">×</span></div>');
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
            //loginName: {
            //    validators: {
            //        notEmpty: {
            //            message: Language.loginNameRequired
            //        },
            //        stringLength: {
            //            min: 6,
            //            max: 30,
            //            message: Language.registerNameLengthNotMatch
            //        },
            //        regexp: {
            //            regexp: /^[a-zA-Z0-9_\.]+$/,
            //            message: Language.registerNameRegexNotMatch
            //        },
            //        remote: {
            //            type: 'POST',
            //            url: '/validate/loginname',
            //            message: Language.registerNameExist,
            //            delay: 500
            //        }
            //    }
            //},
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

var LoginAndRegister = function () {
    "use strict";
    return {
        handleLoginForm: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleLoginForm();
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