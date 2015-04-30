/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleOnlineDeposit = function () {
    $('#depositForm').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            bank: {
                validators: {
                    notEmpty: {
                        message: Language.bankIsRequited
                    }
                }
            },
            amount: {
                validators: {
                    notEmpty: {
                        message: Language.depositAmountIsRequited
                    },
                    callback: {
                        message: "",
                        callback: function (value, validator, $field) {
                            var reg = /^\d{1,12}(?:\.\d{1,2})?$/;
                            if (!reg.test(value)) {
                                return {
                                    valid: false,
                                    message: Language.depositAmountFormatNotMatch
                                };
                            }
                            return true;
                        }
                    }
                }
            }
        }
    });
}

var handelAlipayForm = function () {
    $('#depositAlipayForm').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            bank: {
                validators: {
                    notEmpty: {
                        message: Language.bankIsRequited
                    }
                }
            },
            amount: {
                validators: {
                    notEmpty: {
                        message: Language.depositAmountIsRequited
                    },
                    callback: {
                        message: "",
                        callback: function (value, validator, $field) {
                            var reg = /^\d{1,12}(?:\.\d{1,2})?$/;
                            if (!reg.test(value)) {
                                return {
                                    valid: false,
                                    message: Language.depositAmountFormatNotMatch
                                };
                            }
                            return true;
                        }
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        var amount = $form.find("[name='amount']").val();
        $.post("/deposit/alipay/submit", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var seq = result.Data;
                window.location.href = "/deposit/alipayredirect?amount=" + amount + "&seq=" + seq;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>");
            }
        });
    });
}

var Deposit = function () {
    "use strict";
    return {
        initOnlineDeposit: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleOnlineDeposit();
            });
        }, initAlipayDeposit: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handelAlipayForm();
            });
        }
    };
}();