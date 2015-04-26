var handleUserListTable = function () {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
                { "data": "Item.LoginName" },
                { "data": "Item.Email" },
                { "data": "Item.IsLocked" },
                { "data": "Item.VerifyRealName" },
                { "data": "Item.AtiveAt" },
                { "data": "Item.LastLoginAt" },
                { "data": "Item.LastLoginIp" }
            ],
            "columnDefs": [
                {
                    "targets": 0,
                    "render": function (data, type, full, meta) {
                        return (data || "未设定");
                    }
                }, {
                    "targets": 2, "render": function (data, type, full, meta) {
                        return (data === true ? "<span style='color:red'>已锁定</span>" : "No") + (full.Item.Reason ? " " + full.Item.Reason : "");
                    }
                }, {
                    "targets": 3, "render": function (data, type, full, meta) {
                        return (data === true ? "<a view-identity class='btn btn-sm btn-info'>查看实名信息</span>" : "未实名");
                    }
                },
                {
                    "targets": 4, "render": function (data, type, full, meta) {
                        return (data || "暂未激活");
                    }
                }
            ],
            "processing": false,
            "serverSide": true,
            "filter": false,
            "sort": false,
            "ajax": {
                "url": "/ajax/user/list",
                "data": function (d) {
                    return $.extend({}, d, {
                        "email": $('#search_email').val()
                    });
                }
            }
        });
        $("div.dataTables_Toolbar").html('<div class="col-xs-8 col-md-8">' +
            '<button class="btn btn-inverse" id="btnLockUser">锁定</button>' +
            '<button class="btn btn-inverse" id="btnUnlockUser">解除锁定</button></div>' +
            '<div class="dataTables_filter  text-right"><label>搜索 <input type="search" id="search_email" placeholder="输入Email进行搜索..." /></label></div>');

        $('#data-table tbody').on('click', 'tr', function () {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;

            console.log("a2")
        });
        $('#data-table tbody').on('click', 'a[view-identity]', function (e) {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;
            console.log("a1")
            e.preventDefault();
        });

        $("#search_email").keyup(function () {
            datatable.ajax.reload();
        });

        //锁定用户
        $("#btnLockUser").click(function () {
            Notification.notice("暂未实现", "");
            return;
            if (selectedItem) {
                $("#lockUserDialog").modal('show');
                $("#formLockUser #lockLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何用户", "请选择要锁定的用户");
            }
        });
        $("#formLockUser").submit(function () {
            $.post("/ajax/user/lock", $(this).serialize() + "&userId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("操作成功", selectedItem.LoginName + "已被成功锁定");
                    datatable.ajax.reload();
                    $('#lockUserDialog').modal('hide');
                } else
                    Notification.notice("操作失败", "失败原因:" + result.Message);
            });
            return false;
        });
        //解除锁定用户
        $("#btnUnlockUser").click(function () {

            Notification.notice("暂未实现", "");
            return;
            if (selectedItem) {
                $("#unlockUserDialog").modal('show');
                $("#formUnlockUser #unlockLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何用户", "请选择要解除锁定的用户");
            }
        });
        $("#formUnlockUser").submit(function () {
            $.post("/ajax/user/unlock", "userId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("操作成功", selectedItem.LoginName + "已成功解除锁定");
                    datatable.ajax.reload();
                    $('#unlockUserDialog').modal('hide');
                } else
                    Notification.notice("操作失败", "失败原因:" + result.Message);
            });
            return false;
        });

    };
}

var UserList = function () {
    "use strict";
    return {
        //main function
        init: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleUserListTable();
            });
        }
    };
}();