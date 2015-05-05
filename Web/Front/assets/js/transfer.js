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
                console.log(1)
                $field.nextUntil('[data-fv-validator="promise"][data-fv-for="' + field + '"]')
                      .text(result.message).show();
            }
        }

    }).on('err.field.fv', function (e, data) {
        if (data.field === 'frontAccount' && data.validator === 'promise') {
            var field = data.field, $field = data.element;

            $field.nextUntil('[data-fv-validator="promise"][data-fv-for="' + field + '"]')
                  .hide();
        }
    });
}
var handleConfrimTransferToDotpay = function () {
    $('#transferConfrimForm').formValidation({
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
                $("#noticeBox").html("<h4>" + result.Message + "</h4>").attr("class", "note note-danger").show();
            }
        });
    })
}
var handleTransferToAlipay = function () {
    $('#transferToAlipayForm').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            receiverAccount: {
                validators: {
                    notEmpty: {
                        message: Language.receiverAccountIsRequited
                    },
                    callback: {
                        message: "",
                        callback: function (value, validator, $field) {
                            var regMobile = /^1[3|4|5|8][0-9]\d{4,8}$/;
                            var regEmail = /^[a-zA-Z0-9_\.]+@[a-zA-Z0-9-]+[\.a-zA-Z]+$/;
                            console.log(1)
                            if (!regMobile.test(value) && !regEmail.test(value)) {
                                return {
                                    valid: false,
                                    message: Language.receiverAlipayAccountIsInvalid
                                };
                            }
                            return true;
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
        $.post("/transfer/alipay/submit", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/alipay/confirm?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>");
            }
        });
    });
}
var handleConfrimTransferToAlipay = function () {
    $('#transferConfrimForm').formValidation({
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
        $.post("/transfer/alipay/confirm", $(this).serialize(), function (result, status) {
            console.log(1);
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/alipay/result?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>").attr("class", "note note-danger").show();
            }
        });
    })
}
var handleTransferToBank = function () {
    $('#transferToBankForm').formValidation({
        framework: 'bootstrap',
        //err: { container: 'tooltip' },
        fields: {
            bank: {
                validators: {
                    notEmpty: {
                        message: Language.transferBankIsRequited
                    }
                }
            },
            receiverAccount: {
                validators: {
                    notEmpty: {
                        message: Language.receiverAccountIsRequited
                    },
                    callback: {
                        message: "",
                        callback: function (value, validator, $field) {
                            var reg = /^(\d{16}|\d{19})$/;;
                            if (!reg.test(value) ) {
                                return {
                                    valid: false,
                                    message: Language.receiverBankAccountIsInvalid
                                };
                            }
                            return true;
                        }
                    }
                }
            },
            realName: {
                validators: {
                    notEmpty: {
                        message: Language.receiverBankAccountUserRealNameIsInvalid
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
        $.post("/transfer/bank/submit", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/bank/confirm?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>");
            }
        });
    });
}
var handleConfrimTransferToBank = function () {  
    console.log(1);
    $('#transferConfirmForm').formValidation({
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
        $.post("/transfer/bank/confirm", $(this).serialize(), function (result, status) {
            if (result.Code === 1) {
                var txid = result.Data;
                window.location.href = "/transfer/bank/result?txid=" + txid;

            } else {
                $("#noticeBox").html("<h4>" + result.Message + "</h4>").attr("class", "note note-danger").show();
            }
        });
    })
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
        }, initTransferToAlipay: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleTransferToAlipay();
            });
        }, initConfrimTransferToAlipay: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleConfrimTransferToAlipay();
            });
        }, initTransferToBank: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleTransferToBank();
            });
        }, initConfrimTransferToBank: function () {
            $.getScript('/assets/plugins/formvalidation/js/framework/bootstrap.min.js').done(function () {
                handleConfrimTransferToBank();
            });
        }
    };
}();