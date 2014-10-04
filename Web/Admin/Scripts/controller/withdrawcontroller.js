
function WithdrawCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.title = "";
    $scope.selectWithdraw = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectWithdraw = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { amount: $scope.amount_Search, transfer: $scope.transfer_Search, withdrawState: $scope.action, page: page };

        $http.post('../cnywithdraw/Withdraw/GetWithdrawCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../cnywithdraw/Withdraw/GetWithdrawBySearch', searchCondition).success(function (data, status, headers) {
            $scope.withdraws = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (Withdraw) {
        for (index in $scope.withdraws) {
            var _Withdraw = $scope.withdraws[index];
            if (_Withdraw.ID != Withdraw.ID)
                _Withdraw.Checked = false;
        }

        if (Withdraw.Checked) $scope.selectWithdraw = Withdraw;
        else $scope.selectWithdraw = undefined;
    }

    $scope.verify = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'verifyModalContent.html',
                controller: verifyCNYModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }
    $scope.WaitSubmit = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'waitSubmitModalContent.html',
                controller: waitSubmitModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }



    $scope.success = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'successModalContent.html',
                controller: successCNYModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }

    

    $scope.transferfail = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'transferfailModalContent.html',
                controller: transferfailModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }

    $scope.undoApplication = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'undoExamineModalContent.html',
                controller: undoExamineModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }
    $scope.changerUserBank = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'changerUserBankModalContent.html',
                controller: changerUserBankModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }

    $scope.search();
}



function verifyCNYModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "审核订单";

    $scope.save = function (reason) {
        var url = '../cnywithdraw/Withdraw/Verify';
        $http.post(url, { withdrawID: selectWithdraw.ID, memo: reason }).success(function (data, status, headers) {
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
function waitSubmitModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "提交订单";

    $scope.save = function (reason) {
        var url = '../cnywithdraw/Withdraw/waitSubmit';
        $http.post(url, { withdrawID: selectWithdraw.ID, memo: reason }).success(function (data, status, headers) {
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
function successCNYModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "确认订单";
    $scope.bankAccount = [];
    $scope.selectWithdraw = selectWithdraw;

    $http.post('fundSource/BankAccount').success(function (data, status, headers) {
        $scope.bankAccount = data;
    });
    $scope.save = function (transferNo,ID) {
        var url = '../cnywithdraw/Withdraw/success';
        $http.post(url, { withdrawID: selectWithdraw.ID, transferAccountID: ID, transferNo: transferNo }).success(function (data, status, headers) {
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
function transferfailModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "驳回订单";
    $scope.save = function (reason) {
        var url = '../cnywithdraw/Withdraw/transferfail';
        $http.post(url, { withdrawID: selectWithdraw.ID, memo: reason }).success(function (data, status, headers) {
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
function undoExamineModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "驳回申请";
    $scope.save = function (reason) {
        var url = '../cnywithdraw/Withdraw/undoApplication';
        $http.post(url, { withdrawID: selectWithdraw.ID, memo: reason }).success(function (data, status, headers) {
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
function changerUserBankModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "修改银行账户";
    $scope.province = [];
    $scope.capitalAccountTypes = [];
    $scope.selectWithdraw = selectWithdraw;

    //$http.post('../cnywithdraw/Withdraw/GetProvince').success(function (data, status, headers) {
    //    $scope.provinces = data;
    //});
    //$scope.getCity = function (ID) {
    //    $http.post('../cnywithdraw/withdraw/getcity', { fatherid: ID }).success(function (data, status, headers) {
    //        $scope.citys = data;
    //    });
    //};
    //$scope.getBank = function (ID) {
    //    $http.post('../cnywithdraw/withdraw/getBank', { cityid: ID }).success(function (data, status, headers) {
    //        $scope.banks = data;
    //    });
    //};
    $http.get('capitalAccount/GetCapitalAccountType').success(function (data, status, headers) {
        $scope.capitalAccountTypes = data;
    });


    $scope.add = function (changerUserBank) {
        var url = '../cnywithdraw/Withdraw/changerUserBank';
        $http.post(url, { withdrawID: selectWithdraw.ID, bank: changerUserBank.capitalAccountType.Value, bankAccount: changerUserBank.bankAccount }).success(function (data, status, headers) {
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

