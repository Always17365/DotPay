/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleModifyPassword = function () { 
    $('#modifyLoginPwdForm').formValidation({
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
                        message: Language.oldLoginPasswordRequired
                    }
                }
            },
            newPassword: {
                validators: {
                    notEmpty: {
                        message: Language.newLoginPasswordRequired
                    }, stringLength: {
                        min: 6,
                        message: Language.newLoginPasswordNotMatch
                    }
                }
            },
            confirmPassword: {
                validators: {
                    notEmpty: {
                        message: Language.newConfirmLoginPasswordRequired
                    },
                    identical: {
                        field: 'newPassword',
                        message: Language.newConfirmLoginPasswordNotMatch
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/profile/modifyloginpwd", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#noticeBox").html('<h4>' + Language.modifyLoginPasswordSuccess +
                                     '</h4>').addClass("note-success").show(); 
            } else {
                $("#noticeBox").html('<h4>' + Language.modifyLoginPasswordFail +
                                     '</h4><p>' + result.Message +
                                     '</p>').addClass("note-danger").show();
            }
        });
    });
};

var ModifyLoginPassword = function () {
    "use strict";
    return {
        init: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleModifyPassword();
            });
        }
    };
}();