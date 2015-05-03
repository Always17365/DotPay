/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
var handleTransferToDotpay = function () {
    var remoteHandle;
    var receiverChange;
    var receiverContent;
    $("#frontAccount").click(function () {
        if ($(this).attr("readonly")) {
            $("#frontAccount").removeAttr("readonly");
            receiverContent = $("#frontAccount").val();
            $("#frontAccount").val($("#realAccount").val());
        }
    });

    $("#frontAccount").blur(function () {
        if (!!!$(this).attr("readonly") && !receiverChange) {
            $("#frontAccount").attr("readonly", "readonly");
            $("#frontAccount").val(receiverContent);
        }
    });
    $("#frontAccount").keyup(function () {
        receiverChange = true;
    });
    $('#transferToDotpayForm').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            frontAccount: {
                validators: {
                    notEmpty: {
                        message: Language.receiverAccountIsRequited
                    },
                    promise: {
                        promise: function (value, validator, $field) {
                            if (remoteHandle) window.clearTimeout(remoteHandle);
                            dfd = new $.Deferred();
                            var data = "receiverAccount=" + $("#frontAccount").val();
                            remoteHandle = setTimeout(function () {
                                $.ajax({
                                    url: "/transfer/validateaccount",
                                    data: data,
                                    type: "post",
                                    success: function (data) {
                                        dfd.resolve(data);
                                        receiverChange = false;
                                    },
                                    error: function () {
                                        dfd.reject({
                                            message: 'valid fail'
                                        });

                                        receiverChange = false;
                                    }
                                });
                            }, 1000);
                            return dfd.promise();
                        }
                    }
                }
            },
            transferAmount: {
                validators: {
                    notEmpty: {
                        message: Language.transferAmountIsRequited
                    },
                    callback: {
                        message: "",
                        callback: function (value, validator, $field) {
                            var reg = /^\d{1,12}(?:\.\d{1,2})?$/;
                            if (!reg.test(value)) {
                                return {
                                    valid: false,
                                    message: Language.transferAmountFormatNotMatch
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
        $.post("/transfer/dotpay/submit", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/dotpay/confirm?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>");
            }
        });
    }).on('success.field.fv', function (e, data) {
        if (data.field === 'frontAccount' && data.validator === 'promise') {
            var result = data.result;
            if (result.valid === true) {
                if (result.realName === "") {
                    $("#frontAccount").val(result.account);
                } else {
                    $("#frontAccount").val(result.realName + "( " + result.account + " )");
                }
                $("#frontAccount").attr("readonly", "readonly");
                $("#realAccount").val(result.account);
                var field = data.field, $field = data.element;

                $field
                    .next('.validMessage[data-field="' + field + '"]')
                    .hide();
            }
        }

    }).on('err.field.fv', function (e, data) {
        if (data.field === 'frontAccount' && data.validator === 'promise') {
            var field = data.field, $field = data.element; t

            $field
                .next('.validMessage[data-field="' + field + '"]')
                .hide();
        }
    });
}
var handleConfrimTransferToDotpay = function () {
    $('#transferConfrimFrom').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            paymentpassword: {
                validators: {
                    notEmpty: {
                        message: Language.paymentPasswordIsRequited
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        e.preventDefault();
        var $form = $(e.target);
        $.post("/transfer/dotpay/confirm", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/dotpay/result?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>");
            }
        });
    })
}
var handelTransferToAlipay = function () {
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

var Transfer = function () {
    "use strict";
    return {
        initTransferToDotpay: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleTransferToDotpay();
            });
        }, initConfrimTransferToDotpay: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleConfrimTransferToDotpay();
            });
        },
        initTransferToAlipay: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handelTransferToAlipay();
            });
        }
    };
}();