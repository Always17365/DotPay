/// <reference path="../plugins/jquery/jquery-1.11.2.js" />

var handleSetPaymentPasswordForm = function () {
    $('#setPaymentPasswordFrom').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            paymentpassword: {
                validators: {
                    notEmpty: {
                        message: Language.paymentPasswordIsRequited_Set
                    }, stringLength: {
                        min: 6 ,
                        message: Language.paymentPasswordNotMatch
                    }
                }
            },
            confirmpassword: {
                validators: {
                    notEmpty: {
                        message: Language.confirmPaymentPasswordIsRequited
                    },
                    identical: {
                        field: 'paymentpassword',
                        message: Language.confirmPaymentPasswordMustEquals
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/set/paymentpassword", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                $("#noticeBox").html("<h4>" + Language.paymentPasswordInitializeSuccess + "</h4>").attr("class", "note note-success").show();
            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>").attr("class","note note-danger").show();
            }
        });
    })
}
var SetPaymentPassword = function () {
    "use strict";
    return {
        init: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleSetPaymentPasswordForm();
            });
        }
    };
}();