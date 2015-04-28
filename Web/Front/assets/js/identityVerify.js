/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleIdentityVerify = function () { 
    $('#identityVerifyForm').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        //err: { container: 'tooltip' },
        fields: {
            idType: {
                validators: {
                    notEmpty: {
                        message: Language.identityNoTypeRequired
                    }
                }
            },
            idno: {
                validators: {
                    notEmpty: {
                        message: Language.identityNoRequired
                    }, callback: {
                        message:"",
                        callback: function (value, validator, $field) {
                    
                            var idtype = $("#identityVerifyForm [name='idNoType']").val();
                            var reg = /^(^\d{18}$|^\d{17}(\d|X|x))$/;
                            if (idtype == 1 && !reg.test(value)) {
                                return {
                                    valid: false,
                                    message: Language.identityNoInvalid
                                };
                            }
                            return true;
                        }
                    }
                }
            },
            fullname: {
                validators: {
                    notEmpty: {
                        message: Language.identityFullNameRequired
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/profile/identityverify", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#noticeBox").html('<h4>' + Language.identityVerifySuccess +
                                     '</h4><p>' + Language.identityVerifySuccessDesc +
                                     '</p>').addClass("note-success");
                setTimeout(function () { window.location.reload() }, 1500)
            } else {
                $("#noticeBox").html('<h4>' + Language.identityVerifyFail +
                                     '</h4><p>' + result.Message +
                                     '</p>').addClass("note-danger");
            }
        });
    });
};

var IdentityVerify = function () {
    "use strict";
    return {
        init: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleIdentityVerify(); 
            });
        }
    };
}();