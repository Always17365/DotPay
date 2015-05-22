/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleModifyPayPassword = function () { 
    $('#modifyPayPwdForm').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        //err: { container: 'tooltip' },
        fields: {
            oldPassword: {
                validators: {
                    notEmpty: {
                        message: Language.oldPaymentPasswordRequired
                    }
                }
            },
            newPassword: {
                validators: {
                    notEmpty: {
                        message: Language.newPaymentPasswordRequired
                    }, stringLength: {
                        min: 6,
                        message: Language.newPaymentPasswordNotMatch
                    }
                }
            },
            confirmPassword: {
                validators: {
                    notEmpty: {
                        message: Language.newConfirmPayamentPasswordRequired
                    },
                    identical: {
                        field: 'newPassword',
                        message: Language.newConfirmPayamentPasswordNotMatch
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/profile/modifypaypwd", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#noticeBox").html('<h4>' + Language.modifyPaymentPasswordSuccess +
                                     '</h4>').addClass("note-success").show(); 
            } else {
                $("#noticeBox").html('<h4>' + Language.modifyPaymentPasswordFail +
                                     '</h4><p>' + result.Message +
                                     '</p>').addClass("note-danger").show();
            }
        });
    });
};

var ModifyPayPassword = function () {
    "use strict";
    return {
        init: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleModifyPayPassword();
            });
        }
    };
}();