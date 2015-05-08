var handleTransferListTable = function () {
    "use strict";
    if ($('#recordTable').length !== 0) {
        var datatable = $('#recordTable').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
             { "data": "SequenceNo" },
             { "data": "Payway" },
             { "data": "Amount" },
             { "data": "Status" },
             { "data": "Memo" },
             { "data": "CreateAt" }
            ],
            "columnDefs": [{
                "targets": 1,
                "render": function (data, type, full, meta) {
                    return (full.Bank == "?" ? data : full.Bank);
                }
            }, {
                "targets": 4,
                "render": function (data, type, full, meta) {
                    return data.substr(0, 10);
                }
            }],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/records/transfer/list",
                "type": "POST",
                "data": function (d) {
                    return $.extend({}, d, {
                        "startDate": $('#dateStart').val(),
                        "endDate": $('#dateEnd').val()
                    });
                }
            }
        });

        $("#btnSearch").click(function () {
            var start = $('#dateStart').val();
            var end = $('#dateEnd').val();
            if (start && end)
                datatable.ajax.reload();
        });

    }
};

var TransferRecord = function () {
    "use strict";
    return {
        //main function
        init: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.js').done(function () {
                handleTransferListTable();
            });
        }
    };
}();