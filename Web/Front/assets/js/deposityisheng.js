/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleYiShengDeposit = function() {

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
                        callback: function(value, validator, $field) {
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

var DepositYisheng = function () {
    "use strict";
    return {
        init: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleYiShengDeposit();
            });
        }
    };
}();