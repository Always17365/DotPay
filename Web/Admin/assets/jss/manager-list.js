var handleManagerListTable = function () {
    "use strict";
    var selectedItem;
    if ($('#data-table').length !== 0) {
        var datatable = $('#data-table').DataTable({
            "language": { "sProcessing": "处理中...", "sLengthMenu": "显示 _MENU_ 项结果", "sZeroRecords": "没有匹配结果", "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项", "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项", "sInfoFiltered": "(由 _MAX_ 项结果过滤)", "sInfoPostFix": "", "sSearch": "搜索:", "sUrl": "", "sEmptyTable": "表中数据为空", "sLoadingRecords": "载入中...", "sInfoThousands": ",", "oPaginate": { "sFirst": "首页", "sPrevious": "上页", "sNext": "下页", "sLast": "末页" } },
            "dom": '<"dataTables_Toolbar">frtip',
            "columns": [
             { "data": "Item.LoginName" },
             { "data": "Item.IsLocked" },
             { "data": "Item.LastLoginIp" },
             { "data": "Item.LastLoginAt" },
             { "data": "Item.RoleNames" }
            ],
            "columnDefs": [{
                "targets": 1,
                "data": "Item.Reason",
                "render": function (data, type, full, meta) {
                    return (data===true?"<span style='color:red'>已锁定</span>":"No") + (full.Item.Reason ? " " + full.Item.Reason : "");
                }
            }],
            "processing": true,
            "serverSide": true,
            "sort": false,
            "ajax": "ajax/manager/list",
        });
        $("div.dataTables_Toolbar").html('<button class="btn btn-default" id="btnAddManager"  data-toggle="modal" data-target="#createManagerDialog">添加管理员</button>' +
                              '<button class="btn btn-default" id="btnLockManager">锁定</button>' +
                              '<button class="btn btn-default" id="btnUnlockManager">解除锁定</button>' +
                              '<button class="btn btn-default" id="btnAssignRoles">分配管理角色</button>' +
                              '<button class="btn btn-default" id="btnViewTFKey">查看秘钥</button>' +
                              '<button class="btn btn-default" id="btnResetLoginPassword">重置登陆密码</button>' +
                              '<button class="btn btn-default" id="btnResetTFKey">重置秘钥</button>');

        $('#data-table tbody').on('click', 'tr', function () {
            $(this).siblings().removeClass("selected");
            if ($(this).toggleClass('selected').hasClass('selected')) {
                selectedItem = datatable.row(this).data().Item;
            } else
                selectedItem = null;
        });

        //添加管理员
        $("#btnAddManager").click(function () {
            $("#formCreateManager")[0].reset();
        });
        $("#formCreateManager").submit(function () {
            $.post("/ajax/manager/create", $(this).serialize(), function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("保存成功", "");
                    datatable.ajax.reload();
                    $('#createManagerDialog').modal('hide');
                }
                else
                    Notification.notice("保存失败", result.Message);
            });
            return false;
        });
        //分配管理角色
        $("#btnAssignRoles").click(function () {
            if (selectedItem) {
                $("#assignManagerRolesDialog").modal('show');
                $("#fromAssignRoles #manangeLoginName").val(selectedItem.LoginName);
                $("#fromAssignRoles input:checkbox[name='roles[]']").each(function (n, cb) {
                    if (selectedItem.Roles && selectedItem.Roles.length > 0) {
                        for (var index = 0; index < selectedItem.Roles.length; index++) {
                            if ($(cb).val() == selectedItem.Roles[index]) {
                                $(cb).attr("checked", true);
                                continue;
                            }
                        }
                    }
                    else $(cb).attr("checked", false);
                })
            } else {
                Notification.notice("未选择任何管理员", "请选择一个要分配角色的管理员");
            }
        });
        $("#fromAssignRoles").submit(function () {
            $.post("/ajax/manager/assign", $(this).serialize() + "&managerId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("角色分配成功", selectedItem.LoginName + "已重新分配角色");
                    datatable.ajax.reload();
                    $('#assignManagerRolesDialog').modal('hide');
                }
                else
                    Notification.notice("保存失败", result.Message);
            });
            return false;
        });
        //锁定管理员
        $("#btnLockManager").click(function () {
            if (selectedItem) {
                $("#lockManagerDialog").modal('show');
                $("#formLockManager #lockLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何管理员", "请选择要锁定的管理员");
            }
        });
        $("#formLockManager").submit(function () {
            $.post("/ajax/manager/lock", $(this).serialize() + "&managerId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("锁定成功", selectedItem.LoginName + "已被锁定");
                    datatable.ajax.reload();
                    $('#lockManagerDialog').modal('hide');
                }
                else
                    Notification.notice("锁定失败", result.Message);
            });
            return false;
        });
        //解除锁定管理员
        $("#btnUnlockManager").click(function () {
            if (selectedItem) {
                $("#unlockManagerDialog").modal('show');
                $("#formUnlockManager #unlockLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何管理员", "请选择要解除锁定的管理员");
            }
        });
        $("#formUnlockManager").submit(function () {
            $.post("/ajax/manager/unlock", "managerId=" + selectedItem.Id, function (result, status) {
                if (result.Code === 1) {
                    Notification.notice("解除锁定成功", selectedItem.LoginName + "已解除锁定");
                    datatable.ajax.reload();
                    $('#unlockManagerDialog').modal('hide');
                }
                else
                    Notification.notice("解除锁定失败", result.Message);
            });
            return false;
        });

        //查看tf-key
        $("#btnViewTFKey").click(function () {
            if (selectedItem) {
                $("#viewTFKeyDialog").modal('show');
                $("#viewTFKeyDialog #tfLoginName").val(selectedItem.LoginName);
                $("#viewTFKeyDialog #tfKey").val("正在加载...");
                $.post("/ajax/manager/tfkey", { managerId: selectedItem.Id }, function (result, status) {
                    if (result.Code == 1) {
                        $("#viewTFKeyDialog #tfKey").val(result.Message);
                    } else {
                        $("#viewTFKeyDialog #tfKey").val("加载出错，请稍后重试");
                    }
                })
            } else {
                Notification.notice("未选择任何管理员", "请选择要解除锁定的管理员");
            }
        });
        //重置登陆密码
        $("#btnResetLoginPassword").click(function () {
            if (selectedItem) {
                $("#formResetLoginPassword")[0].reset();
                $("#resetLoginPwdDialog").modal('show');
                $("#resetLoginPwdDialog #resetLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何管理员", "请选择管理员");
            }
        });

        $("#formResetLoginPassword").submit(function () {
            $.post("/ajax/manager/resetloginpwd", $(this).serialize() + "&managerId=" + selectedItem.Id, function(result, status) {
                if (result.Code == 1) { 
                    Notification.notice("重置成功", selectedItem.LoginName + "的登陆密码重置成功了");
                    $("#resetLoginPwdDialog").modal('hide');
                } else {
                    Notification.notice("重置失败", result.Message);
                }
            });
            return false;
        });

        //重置Twofactor-Key
        $("#btnResetTFKey").click(function () {
            if (selectedItem) {
                $("#resetTfKeyDialog").modal('show');
                $("#resetTfKeyDialog #resetTFLoginName").val(selectedItem.LoginName);
            } else {
                Notification.notice("未选择任何管理员", "请选择管理员");
            }
        });

        $("#formResetTfKey").submit(function () {
            $.post("/ajax/manager/resettfkey", $(this).serialize() + "&managerId=" + selectedItem.Id, function(result, status) {
                if (result.Code == 1) {
                    Notification.notice("重置成功", selectedItem.LoginName+"的谷歌秘钥重置成功了");
                    $("#resetTfKeyDialog").modal('hide');
                } else {
                    Notification.notice("重置失败", result.Message);
                }
            });
            return false;
        });
        window.ParsleyValidator.setLocale('zh_cn');
    }
};

var ManagerList = function () {
    "use strict";
    return {
        //main function
        init: function () {
            $.getScript('/assets/plugins/DataTables/js/jquery.dataTables.min.js').done(function () {
                handleManagerListTable();
            });
        }
    };
}();