var handleDotpayToFiPendingListTable = function () {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
             { "data": "Item.Email" },
             { "data": "Item.SequenceNo" },
             { "data": "Item.Payway" },
             { "data": "Item.Destination" },
             { "data": "Item.Amount" },
             { "data": "Item.Manager" },
             { "data": "Item.Memo" },
             { "data": "Item.CreateAt" }
            ],
            "columnDefs": [{
                "targets": 2,
                "render": function (data, type, full, meta) {
                    return full.Item.Payway + (!!!full.Item.Bank ? "" : "-" + full.Item.BankName)
                }
            }, {
                "targets": 4,
                "render": function (data, type, full, meta) {
                    return full.Item.Amount + full.Item.Currency + "(实名:" + (full.Item.RealName || "无") + ")";
                }
            }, {
                "targets": 5,
                "render": function (data, type, full, meta) {
                    return (!!data) ? ("锁定-" + data) : "待处理";
                }
            }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/dotpaytofi/tx/pending/list",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val()
                    });
                }
            }
        });
        $("div.dataTables_Toolbar").html('<div class="col-sm-6 col-md-6"><button class="btn btn-inverse" id="btnLockTx">锁定</button>' +
                               '<button class="btn btn-inverse" id="btnCompleteTx">转账完成</button>' +
                               '<button class="btn btn-inverse" id="btnFailTx">转账失败</button></div>' +
                               '<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="用户邮箱" /></label></div>');

        $('#data-table tbody').on('click', 'tr', function () {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;
        });

        $("#search_email").keyup(function () {
            datatable.ajax.reload();
            selectedItem = undefined;
        });
        //锁定交易
        $("#btnLockTx").click(function () {
            if (selectedItem && selectedItem.Payway === "Ripple") {
                Notification.notice("操作失败", "Ripple转账无需人工干预");
                return;
            }

            if (selectedItem) {
                $.post("/ajax/dotpaytofi/tx/lock", "txid=" + selectedItem.Id, function (result, status) {
                    if (result.Code === 1) {
                        Notification.notice("操作成功", "已成功锁定该笔交易，您可以继续进行实际转账操作了");
                        datatable.ajax.reload();
                        $('#assignManagerRolesDialog').modal('hide');
                    } else
                        Notification.notice("操作失败", result.Message);
                });
            } else {
                Notification.notice("操作失败", "未选中任何交易，请选中表格中需要操作的交易");
            }
        });

        //转账完成
        $("#btnCompleteTx").click(function () {
            if (selectedItem && selectedItem.Payway === "Ripple") {
                Notification.notice("操作失败", "Ripple转账无需人工干预");
                return;
            }
            if (selectedItem && !selectedItem.Manager) {
                Notification.notice("操作失败", "在进行此操作前，需先锁定该笔交易");
            } else if (selectedItem) {
                $("#formCompleteTx")[0].reset();
                $("#completeTxDialog").modal('show');
                $("#formCompleteTx #transferTo").val(selectedItem.Payway + (!selectedItem.Bank ? "" : "-" + selectedItem.BankName));
                $("#formCompleteTx #toaccount").val(selectedItem.Destination);
                $("#formCompleteTx #transferHideAmount").val(selectedItem.Amount);
            } else {
                Notification.notice("操作失败", "请选择要进行操作的交易");
            }
        });

        $("#formCompleteTx").submit(function () {
            $.post("/ajax/dotpaytofi/tx/confirm", $(this).serialize() + "&txId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    var msg = selectedItem.Payway + (selectedItem.Bank ? "" : "-" + selectedItem.Bank) + " 转入" + selectedItem.Amount +
                              " " + selectedItem.Currency + "操作成功";
                    Notification.notice("操作成功", msg);
                    datatable.ajax.reload();
                    selectedItem = undefined;
                    $('#completeTxDialog').modal('hide');
                }
                else
                    Notification.notice("操作失败", "失败原因:" + result.Message);
            });
            return false;
        });
        //转账失败
        $("#btnFailTx").click(function () {
            if (selectedItem && selectedItem.Payway === "Ripple") {
                Notification.notice("操作失败", "Ripple转账无需人工干预");
                return;
            }
            if (selectedItem && !selectedItem.Manager) {
                Notification.notice("操作失败", "在进行此操作前，需先锁定该笔交易");
                return;
            }
            if (selectedItem) {
                $("#formFailTx")[0].reset();
                $("#failTxDialog").modal('show');
                $("#formFailTx #transferFailTo").val(selectedItem.Payway + (!selectedItem.Bank ? "" : "-" + selectedItem.BankName));
                $("#formFailTx #failToAccount").val(selectedItem.Destination);
                $("#formFailTx #txid").val(selectedItem.Id);
                $("#formFailTx #transferFailAmount").val(selectedItem.Amount);
            } else {
                Notification.notice("操作失败", "请选择要进行操作的交易");
            }
        });

        $("#formFailTx").submit(function () {
            $.post("/ajax/dotpaytofi/tx/confirmfail", $(this).serialize() + "&txId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    var msg = selectedItem.Payway + selectedItem.Bank ? "" : "-" + selectedItem.Bank + " 转入" + selectedItem.Amount +
                              " " + selectedItem.Currency + "标记为失败成功，资金已自动退还用户账户";
                    Notification.notice("操作成功", msg);
                    datatable.ajax.reload();
                    selectedItem = undefined;
                    $('#failTxDialog').modal('hide');
                }
                else
                    Notification.notice("操作失败", "失败原因:" + result.Message);
            });
            return false;
        });
        window.ParsleyValidator.setLocale('zh_cn');
    }
};
var handleDotpayToFiCompleteListTable = function () {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
             { "data": "Item.Email" },
             { "data": "Item.SequenceNo" },
             { "data": "Item.Payway" },
             { "data": "Item.Destination" },
             { "data": "Item.Amount" },
             { "data": "Item.FiTransactionNo" },
             { "data": "Item.Manager" },
             { "data": "Item.Memo" },
             { "data": "Item.CreateAt" },
             { "data": "Item.CompleteAt" }
            ],
            "columnDefs": [{
                "targets": 2,
                "render": function (data, type, full, meta) {
                    return full.Item.Payway + (!!!full.Item.Bank ? "" : "-" + full.Item.BankName)
                }
            }, {
                "targets": 4,
                "render": function (data, type, full, meta) {
                    return full.Item.Amount + full.Item.Currency + "(实名:" + (full.Item.RealName || "无") + ")";
                }
            }, {
                "targets": 5,
                "render": function (data, type, full, meta) {
                    return (!!data) ? ("锁定-" + data) : "待处理";
                }
            }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/dotpaytofi/tx/complete/list",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val(),
                        "sequenceNo": $('#search_seq').val(),
                        "transferNo": $('#search_transferno').val()
                    });
                }
            }
        });
        $("div.dataTables_Toolbar").html('<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="用户邮箱" />' +
                               '<input type="search" id="search_seq" placeholder="流水号" />' +
                               '<input type="search" id="search_transferno" placeholder="交易号" /></label></div>');

        $('#data-table tbody').on('click', 'tr', function () {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;
        });

        $("#search_email,#search_seq,#search_transferno").keyup(function () {
            datatable.ajax.reload();
        });
        window.ParsleyValidator.setLocale('zh_cn');
    }
};
var handleDotpayToFiFailListTable = function () {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
             { "data": "Item.Email" },
             { "data": "Item.SequenceNo" },
             { "data": "Item.Payway" },
             { "data": "Item.Destination" },
             { "data": "Item.Amount" },
             { "data": "Item.Manager" },
             { "data": "Item.Reason" },
             { "data": "Item.Memo" },
             { "data": "Item.CreateAt" },
             { "data": "Item.CompleteAt" }
            ],
            "columnDefs": [{
                "targets": 2,
                "render": function (data, type, full, meta) {
                    return full.Item.Payway + (!!!full.Item.Bank ? "" : "-" + full.Item.BankName)
                }
            }, {
                "targets": 4,
                "render": function (data, type, full, meta) {
                    return full.Item.Amount + full.Item.Currency + "(实名:" + (full.Item.RealName || "无") + ")";
                }
            }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/dotpaytofi/tx/fail/list",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val(),
                        "sequenceNo": $('#search_seq').val()
                    });
                }
            }
        });
        $("div.dataTables_Toolbar").html('<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="用户邮箱" />' +
                               '<input type="search" id="search_seq" placeholder="流水号" /></label></div>');

        $('#data-table tbody').on('click', 'tr', function () {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;
        });

        $("#search_email,#search_seq").keyup(function () {
            datatable.ajax.reload();
        });
        window.ParsleyValidator.setLocale('zh_cn');
    }
};

var DotpayToFiList = function () {
    "use strict";
    return {
        //main function
        initPendingList: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleDotpayToFiPendingListTable();
            });
        }, initCompleteList: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleDotpayToFiCompleteListTable();
            });
        }, initFailList: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleDotpayToFiFailListTable();
            });
        }
    };
}();