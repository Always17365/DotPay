function VirtualCurrencyDepositCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.title = "";
    $scope.selectDeposit = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;
    $scope.state = $routeParams.state;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectDeposit = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { userID: $scope.userID_Search, starttime: $scope.start_Search, endtime: $scope.end_Search, currency: $scope.action, state: $scope.state, page: page };

        $http.post('../virtualcurrencydeposit/Deposit/CountVirtualCurrencyDepositBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../virtualcurrencydeposit/Deposit/GetVirtualCurrencyDepositBySearch', searchCondition).success(function (data, status, headers) {
            $scope.deposits = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (Deposit) {
        for (index in $scope.Deposits) {
            var _Deposit = $scope.Deposits[index];
            if (_Deposit.ID != Deposit.ID)
                _Deposit.Checked = false;
        }

        if (Deposit.Checked) $scope.selectDeposit = Deposit;
        else $scope.selectDeposit = undefined;
    }

    $scope.search();
}

function ReceivePayMentTransactionCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.title = "";
    $scope.selectDeposit = undefined;
    $scope.currentPage = 1;
    $scope.currencyType = $routeParams.currencyType;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectDeposit = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { currencyType: $scope.currencyType, page: page };

        $http.post('../virtualcurrencydeposit/Deposit/CountReceivePayMentTransactionBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../virtualcurrencydeposit/Deposit/GetReceivePayMentTransactionBySearch', searchCondition).success(function (data, status, headers) {
            $scope.deposits = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (Deposit) {
        for (index in $scope.Deposits) {
            var _Deposit = $scope.Deposits[index];
            if (_Deposit.ID != Deposit.ID)
                _Deposit.Checked = false;
        }

        if (Deposit.Checked) $scope.selectDeposit = Deposit;
        else $scope.selectDeposit = undefined;
    }

    $scope.entry = function () {
        if ($scope.selectDeposit) {
            var modalInstance = $modal.open({
                templateUrl: 'receivePayMentTransaction.html',
                controller: ReceivePayMentTransactionModalCtrl,
                backdrop: false,
                resolve: {
                    selectDeposit: function () { return $scope.selectDeposit; },
                    currencyType: function () { return $scope.currencyType; }
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
function ReceivePayMentTransactionModalCtrl($scope, $modalInstance, $http, $alert, selectDeposit, currencyType) {
    $scope.title = "审核订单";
    $scope.selectDeposit = selectDeposit;

    $scope.save = function (txid, amount) {
        var url = '../virtualcurrencydeposit/Deposit/ConfirmReceivePaymentTransaction';
        $http.post(url, { receivePaymentTxId: selectDeposit.ID, txid: txid, amount: amount, currency: currencyType }).success(function (data, status, headers) {
            if (data.Code != 138 || data.Code==1)
                $modalInstance.close(true);
            else
                $alert.Warn("资金确认与录入信息有误!")
                $modalInstance.close(false);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};


function CompleteVirtualCoinWithdrawModalCtrl($scope, $modalInstance, $http, selectWithdraw, currencyType) {
    $scope.title = "审核订单";
    $scope.selectDeposit = selectWithdraw;

    $scope.save = function (txid) {
        var url = '../virtualcurrencywithdraw/Withdraw/CompleteVirtualCoinWithdraw';
        $http.post(url, { withdrawUniqueID: selectWithdraw.UniqueID, txid: txid, currency: currencyType }).success(function (data, status, headers) {
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

function StayAuditCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.selectWithdraw = undefined;
    $scope.currentPage = 1;
    $scope.currencyType = $routeParams.currencyType;
    $scope.state = $routeParams.state;
    switch ($scope.state)
    {
        case "3":
            $scope.title = "待审核";
            break;
        case "4":
            $scope.title = "已完成";
            break;
        case "5":
            $scope.title = "处理失败";
            break;
        case "6":
            $scope.title = "已撤销";
            break;

    }

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectWithdraw = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { userID: $scope.UserID_Search, state: $scope.state, currencyType: $scope.currencyType, page: page };

        $http.post('../virtualcurrencywithdraw/Withdraw/GetWithdrawCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../virtualcurrencywithdraw/Withdraw/GetWithdrawBySearch', searchCondition).success(function (data, status, headers) {
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
                controller: verifyModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; },
                    currencyType: function () { return $scope.currencyType; }
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
                controller: successModalCtrl,
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

    $scope.rescind = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'cancelVirtualCoinWithdraw.html',
                controller: CancelVirtualCoinWithdrawModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; },
                    currencyType: function () { return $scope.currencyType; }
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

    $scope.complete = function () {
        if ($scope.selectWithdraw) {
            var modalInstance = $modal.open({
                templateUrl: 'CompleteVirtualCoinWithdraw.html',
                controller: CompleteVirtualCoinWithdrawModalCtrl,
                backdrop: false,
                resolve: {
                    selectWithdraw: function () { return $scope.selectWithdraw; },
                    currencyType: function () { return $scope.currencyType; }
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


function verifyModalCtrl($scope, $modalInstance, $http, selectWithdraw, currencyType) {
    $scope.title = "审核订单";

    $scope.save = function (reason, hidden) {
        var url = '../virtualcurrencywithdraw/Withdraw/Verify';
        $http.post(url, { withdrawID: selectWithdraw.ID, memo: reason, currencyType: currencyType }).success(function (data, status, headers) {
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
function successModalCtrl($scope, $modalInstance, $http, selectWithdraw) {
    $scope.title = "确认订单";
    $scope.bankAccount = [];
    $scope.selectWithdraw = selectWithdraw;

    $http.post('fundSource/BankAccount').success(function (data, status, headers) {
        $scope.bankAccount = data;
    });
    $scope.save = function (transferNo, ID) {
        var url = '../virtualcurrencywithdraw/Withdraw/success';
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
function CancelVirtualCoinWithdrawModalCtrl($scope, $modalInstance, $http, selectWithdraw, currencyType) {
    $scope.title = currencyType + "撤销失败订单";
    $scope.selectWithdraw = selectWithdraw;
    $scope.action = currencyType;


    $scope.save = function (reason) {
        var url = '../virtualcurrencywithdraw/Withdraw/cancelvirtualcoinwithdraw';
        $http.post(url, { withdrawUniqueID: selectWithdraw.UniqueID, memo: reason, currency: currencyType }).success(function (data, status, headers) {
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