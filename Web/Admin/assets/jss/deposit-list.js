var handlePendingDepositListTable = function (isAlipay) {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
                { "data": "Item.Email" },
                { "data": "Item.SequenceNo" },
                { "data": "Item.Amount" },
                { "data": "Item.PaywayName" },
                { "data": "Item.Memo" },
                { "data": "Item.CreateAt" }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": isAlipay === true ? "/ajax/deposit/tx/alipay/list/pending" : "/ajax/deposit/tx/list/pending",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val(),
                        "sequenceNo": $('#search_seq').val()
                    });
                }
            }
        }); 
       
        var tools = (isAlipay === true ? '<div class="col-xs-2 col-md-2">' +
            '<button class="btn btn-inverse" id="btnConfirm">' +
            '确认充值</button></div>' : "") +
            '<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="输入用户名进行搜索..." /><input type="search" id="search_seq" placeholder="输入完整流水号搜索" /></label></div>';

        $("div.dataTables_Toolbar").html(tools);

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

        //确认充值
        $("#btnConfirm").click(function () {
            if (selectedItem) {
                $("#formConfirmDeposit")[0].reset();
                $("#confirmDepositDialog").modal('show');
                $("#formConfirmDeposit #seqno").val(selectedItem.SequenceNo);
            } else {
                Notification.notice("未选择任何待处理充值", "请选择一个要确认的充值记录");
            }

        });
        $("#formConfirmDeposit").submit(function () {
            var amount = $("#formConfirmDeposit [name='amount']").val()

            if (amount == selectedItem.Amount) {
                $.post("/ajax/deposit/confirm", $(this).serialize() + "&depositId=" + selectedItem.Id, function (result, status) {
                    if (result.Code === 1) {
                        Notification.notice("操作成功", "单号:" + selectedItem.SequenceNo + "<br> 金额:" + selectedItem.Amount + "<br>  成功确认!");
                        datatable.ajax.reload();
                        $('#confirmDepositDialog').modal('hide');
                    } else
                        Notification.notice("操作失败", "失败原因:" + result.Message);
                });
            } else {
                Notification.notice("操作失败", "充值金额不匹配，请仔细核对");
            }
            return false;
        });
    };
}
var handleCompleteDepositListTable = function () {
    "use strict";
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
                { "data": "Item.Email" },
                { "data": "Item.SequenceNo" },
                { "data": "Item.Amount" },
                { "data": "Item.PaywayName" },
                { "data": "Item.TransactionNo" },
                { "data": "Item.Memo" },
                { "data": "Item.CreateAt" }
            ],
            "columnDefs": [
                {
                    "targets": 4,
                    "render": function (data, type, full, meta) {
                        return (full.Item.ManagerName ? full.Item.ManagerName + ":" : "")+(data || "");
                    }
                }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/deposit/tx/list/complete",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val(),
                        "sequenceNo": $('#search_seq').val()
                    });
                }
            }
        });
        var tools = '<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="输入用户名进行搜索..." /><input type="search" id="search_seq" placeholder="输入完整流水号搜索" /></label></div>';

        $("div.dataTables_Toolbar").html(tools);

        $("#search_email,#search_seq").keyup(function () {
            datatable.ajax.reload();
        });
    }
}
var handleFailDepositListTable = function () {
    "use strict";
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
                { "data": "Item.Email" },
                { "data": "Item.SequenceNo" },
                { "data": "Item.Amount" },
                { "data": "Item.PaywayName" },
                { "data": "Item.Reason" },
                { "data": "Item.Memo" },
                { "data": "Item.CreateAt" }
            ],
            "columnDefs": [
                {
                    "targets": 4,
                    "render": function (data, type, full, meta) {
                        return (full.Item.ManagerName ? full.Item.ManagerName + ":" : "")+(data || "");
                    }
                }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/deposit/tx/list/fail",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val(),
                        "sequenceNo": $('#search_seq').val()
                    });
                }
            }
        });
        var tools = '<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="输入用户名进行搜索..." /><input type="search" id="search_seq" placeholder="输入完整流水号搜索" /></label></div>';

        $("div.dataTables_Toolbar").html(tools);

        $("#search_email,#search_seq").keyup(function () {
            datatable.ajax.reload();
        });

    }
}
var DepositList = function () {
    "use strict";
    return {
        //main function
        initAlipayPendingDepositListTable: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                var isAlipay = true; handlePendingDepositListTable(isAlipay);
            });
        },
        initPendingDepositListTable: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handlePendingDepositListTable();
            });
        },
        initCompleteDepositListTable: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleCompleteDepositListTable();
            });
        },
        initFailDepositListTable: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleFailDepositListTable();
            });
        }
    };
}();