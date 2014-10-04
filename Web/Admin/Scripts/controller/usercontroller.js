
function UserCtrl($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.title = "用户列表";
    $scope.selectUser = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectUser = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { userId: $scope.userID_Search, email: $scope.email_Search, page: page, isLocked: $scope.action != null ? true :false };

        $http.post('../User/GetUserCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../User/GetUsersBySearch', searchCondition).success(function (data, status, headers) {
            $scope.users = data;
        });
    }

    $scope.checkedChange = function (user) {
        for (index in $scope.users) {
            var cUser = $scope.users[index];
            if (cUser.ID != user.ID)
                cUser.Checked = false;
        }

        if (user.Checked) $scope.selectUser = user;
        else $scope.selectUser = undefined;
    }

    $scope.assignRole = function (role) {
        var user = $scope.selectUser
        if (user) {
            var modalInstance = $modal.open({
                templateUrl: 'assignRoleModalContent.html',
                controller: AssignRoleModalCtrl,
                backdrop: false,
                resolve: {
                    user: function () {
                        return user;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result = true) $location.path("/manager");
                else $alert.Warn("分配角色失败！请刷新后重试");

            });
        }
        else $alert.Warn("未选择任何用户")
    }

    $scope.lockUser = function () {
        if ($scope.selectUser == undefined) $alert.Warn("未选择任何用户");
        else if ($scope.selectUser.IsLocked == true) $alert.Warn("用户已被锁定");
        else {
            var modalInstance = openModifyLockModal($modal, $scope.selectUser, true);
            modalInstance.result.then(function (result) {
                if (result = true) $scope.refresh();
                else $alert.Warn("锁定失败！请刷新后重试");

            });
        }
    }

    $scope.unlockUser = function () {
        if ($scope.selectUser == undefined) $alert.Warn("未选择任何用户");
        else if ($scope.selectUser.IsLocked == false) $alert.Warn("用户未被锁定");
        else {
            var modalInstance = openModifyLockModal($modal, $scope.selectUser, false);
            modalInstance.result.then(function (result) {
                if (result = true) $scope.refresh();
                else $alert.Warn("解锁失败！请刷新后重试");

            });
        }
    }

    $scope.refresh = function () { $scope.pageChange($scope.currentPage); }

    var openModifyLockModal = function ($modal, user, lock) {
        var modalInstance = $modal.open({
            templateUrl: 'lockUserModalContent.html',
            controller: EnableDisableModalCtrl,
            backdrop: false,
            resolve: {
                user: function () {
                    return user;
                },
                lock: function () { return lock; }
            }
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });

        return modalInstance;
    }
    $scope.search();
}


function GetUserAccountInfoCtrl($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.title = "用户列表";
    $scope.selectUser = undefined;
    $scope.currentPage = 1;
    $scope.currencyType = $routeParams.currencyType;
    $scope.order = "balance";

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectUser = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { userId: $scope.userID_Search, email: $scope.email_Search, currencyType: $scope.currencyType, order: $scope.order, page: page };

        $http.post('../User/GetUsersCurrencyCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../User/GetUsersCurrencyBySearch', searchCondition).success(function (data, status, headers) {
            $scope.users = data;
        });
    }
    $scope.setOrder = function (order) {
        $scope.order = order;
        $scope.search($scope.currentPage);
    };
    
    $scope.search();
}

function AssignRoleModalCtrl($scope, $modalInstance, $http, $alert, user) {
    $scope.user = user;
    $scope.managerRoles = [];
    $http.get('../User/GetManagerRoles').success(function (data, status, headers) {
        $scope.managerRoles = data;
    });

    $scope.save = function (role) {
        $http.post("../User/AssignUserRole", { userId: user.ID, role: role.Value })
                 .success(function (data) {
                     if (data.Code == 1) $modalInstance.close(true);
                     else if (data.Code == -3) $alert.Danger("无权进行此操作！");
                     else $modalInstance.close(false);
                 })
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

function EnableDisableModalCtrl($scope, $modalInstance, $http, user, lock) {
    $scope.user = user;
    $scope.lock = lock;

    if (lock) $scope.title = "锁定用户";
    else $scope.title = "解锁用户";

    $scope.save = function (reason) {
        var url = lock ? '../User/Lock' : '../User/Unlock';
        $http.post(url, { userId: $scope.user.ID, reason: reason }).success(function (data, status, headers) {
            if (data.Code == 1)
                $modalInstance.close(true);
            else
                $modalInstance.close(false);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

function ManagerCtrl($scope, $http, $sce, $alert) {
    $scope.title = "管理员列表";
    $scope.selectManager = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectManager = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { email: $scope.email_Search, page: page };

        $http.post('../User/GetManagerCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../User/GetManagersBySearch', searchCondition).success(function (data, status, headers) {
            $scope.managers = data;
        });
    }

    $scope.checkedChange = function (manager) {
        for (index in $scope.managers) {
            var cManager = $scope.managers[index];
            if (cManager.ID != manager.ID)
                cManager.Checked = false;
        }

        if (manager.Checked) $scope.selectManager = manager;
        else $scope.selectManager = undefined;
    }

    $scope.remove = function () {
        var manager = $scope.selectManager;
        if (manager) {
            $http.post("../User/RemoveManager", { UserID: manager.UserID, managerID: manager.ID }).success(function (data) {
                if (data.Code == 1) $scope.search();
                else if (data.Code == -3) $alert.Warn("无权进行此操作");

            })
        }
    }

    $scope.search();
}

function CustomerServiceCtrl($scope, $http, $sce, $modal, $alert) {
    $scope.title = "客服列表";
    $scope.selectServer = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectServer = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { email: $scope.email_Search, page: page };

        $http.post('../User/GetCustomerServiceCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../User/GetCustomerServicesBySearch', searchCondition).success(function (data, status, headers) {
            $scope.servers = data;
        });
    }

    $scope.checkedChange = function (server) {
        for (index in $scope.servers) {
            var cServer = $scope.servers[index];
            if (cServer.ID != server.ID)
                cServer.Checked = false;
        }

        if (server.Checked) $scope.selectServer = server;
        else $scope.selectServer = undefined;
    }


    $scope.see = function () {
        var user = $scope.selectServer
        if (user) {
            var modalInstance = $modal.open({
                templateUrl: 'seeUserDepositAmount.html',
                controller: seeUserDepositAmountCtrl,
                backdrop: true,
                resolve: {
                    user: function () {
                        return user;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    //$scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任何用户")
    }
    $scope.authorize = function () {
        var user = $scope.selectServer
        if (user) {
            var modalInstance = $modal.open({
                templateUrl: 'authorizeCustomerServiceUserDepositAmount.html',
                controller: AuthorizeCustomerServiceUserDepositAmountCtrl,
                backdrop: false,
                resolve: {
                    user: function () {
                        return user;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    //$scope.pageChange($scope.currentPage);
                    //$scope.checkedChange();
                }
            });
        }
        else $alert.Warn("未选择任何用户")
    }
    $scope.search();
}
function AuthorizeCustomerServiceUserDepositAmountCtrl($scope, $modalInstance, $http, user) {
    $scope.currencies = [];
    $scope.user = user;
    $http.post('../Currency/GetCurrenciesBySearch', { page: 1 }).success(function (data, status, headers) {
        $scope.currencies = data;
    });
    $scope.save = function (currencie,depositamount) {
        if (this.authorize.$valid) {
            $http.post('../user/AuthorizeCustomerServiceUserDepositAmount', { authTo: $scope.user.UserID, currencyType: currencie.Code, amount: depositamount }).success(function (data, status, headers) {
                if (data.Code == 1) $modalInstance.close(true);
                else if (data.Code == -3) $alert.Danger("剩余可用余额不足生成此充值码！");
                else $modalInstance.close(false);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function seeUserDepositAmountCtrl($scope, $modalInstance, $http, user) {
    $scope.user = user;
    $http.post('../user/seeUserDepositAmoun', { userID: $scope.user.UserID}).success(function (data, status, headers) {
            $scope.servers = data;
    });
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function DepositCodeMakeLogCtrl($scope, $http, $sce, $alert, $routeParams) {
    $scope.selectUserLog = undefined;
    $scope.currentPage = 1;
    $scope.nickname = $routeParams.nickname;
    $scope.userLogID = $routeParams.action;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);
        $scope.selectUserLog = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { userLogID: $scope.userLogID, starttime: $scope.start_Search, endtime: $scope.end_Search, page: page };

        $http.post('../User/GetUseLogCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../User/GetUseLogsBySearch', searchCondition).success(function (data, status, headers) {
            $scope.userLogs = data;
        });
    }


    $scope.search();
}
