function GetPendingTransferTransaction($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectTransferTransaction = undefined;
    $scope.currentPage = 1;
    $scope.payway = $routeParams.payway;
    $scope.title = $routeParams.payway;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);

    }
    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { payway: $scope.payway, page: page };
        $http.post('../TransferTransaction/GetPendingTransferTransaction', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data.count;
            $scope.transferTransactions = data.result;
        });
    }
    $scope.checkedChange = function (transferTransaction) {
        for (index in $scope.transferTransactions) {
            var _transferTransaction = $scope.transferTransactions[index];
            if (_transferTransaction.ID != transferTransaction.ID)
                _transferTransaction.Checked = false;
        }

        if (transferTransaction.Checked) {
            $scope.selectTransferTransaction = transferTransaction;
            $scope.selectTransferTransaction.payway = $scope.payway;
        }
        else $scope.selectTransferTransaction = undefined;
    }
    $scope.Processing = function () { 
        if ($scope.selectTransferTransaction) {
            $http.post('../TransferTransaction/MarkThirdPartyPaymentTransferProcessing', { tppTransferId: $scope.selectTransferTransaction.ID, payway: $scope.selectTransferTransaction.payway }).success(function (data, status, headers) {
                if (data.Code == 1) {
                    $scope.pageChange($scope.currentPage);
                    $alert.Warn("处理成功");
                } else {
                    $alert.Warn("处理失败");
                }
            });
        }
        else $alert.Warn("未选择任何用户")
    }
    $scope.Success = function () {
        var transferTransaction = $scope.selectTransferTransaction
        if (transferTransaction) {
            var modalInstance = $modal.open({
                templateUrl: 'successTransferTransaction.html',
                controller: successTransferTransactionCtrl,
                backdrop: true,
                resolve: {
                    transferTransaction: function () {
                        return transferTransaction;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任何用户")
    }
    $scope.Fail = function () {
        var transferTransaction = $scope.selectTransferTransaction
        if (transferTransaction) {
            var modalInstance = $modal.open({
                templateUrl: 'failTransferTransaction.html',
                controller: failTransferTransactionCtrl,
                backdrop: true,
                resolve: {
                    transferTransaction: function () {
                        return transferTransaction;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任何用户")
    }
    $scope.search();
}

function GetSuccessTransferTransaction($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectTransferTransaction = undefined;
    $scope.currentPage = 1;
    $scope.payway = $routeParams.payway;
    $scope.title = $routeParams.payway;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);

    }
    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { account: $scope.Account, amount: $scope.Amount, txid: $scope.TxId, starttime: $scope.start_Search, endtime: $scope.end_Search, payway: $scope.payway, page: page };
        $http.post('../TransferTransaction/GetSuccessTransferTransaction', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data.count;
            $scope.transferTransactions = data.result;
        });
    }
    $scope.checkedChange = function (transferTransaction) {
        for (index in $scope.transferTransactions) {
            var _transferTransaction = $scope.transferTransactions[index];
            if (_transferTransaction.ID != transferTransaction.ID)
                _transferTransaction.Checked = false;
        }

        if (transferTransaction.Checked) {
            $scope.selectTransferTransaction = transferTransaction;
            $scope.selectTransferTransaction.payway = $scope.payway;
        }
        else $scope.selectTransferTransaction = undefined;
    }
    $scope.search();
}
function GetFailTransferTransaction($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectTransferTransaction = undefined;
    $scope.currentPage = 1;
    $scope.payway = $routeParams.payway;
    $scope.title = $routeParams.payway;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);

    }
    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { account: $scope.Account, amount: $scope.Amount, txid: $scope.TxId, starttime: $scope.start_Search, endtime: $scope.end_Search, payway: $scope.payway, page: page };
        $http.post('../TransferTransaction/GetFailTransferTransaction', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data.count;
            $scope.transferTransactions = data.result;
        });
    }
    $scope.checkedChange = function (transferTransaction) {
        for (index in $scope.transferTransactions) {
            var _transferTransaction = $scope.transferTransactions[index];
            if (_transferTransaction.ID != transferTransaction.ID)
                _transferTransaction.Checked = false;
        }

        if (transferTransaction.Checked) {
            $scope.selectTransferTransaction = transferTransaction;
            $scope.selectTransferTransaction.payway = $scope.payway;
        }
        else $scope.selectTransferTransaction = undefined;
    } 
    $scope.search();
}

function successTransferTransactionCtrl($scope, $modalInstance, $http, $alert, transferTransaction) {

    $scope.set = function (successTransferTransaction) {
        if (transferTransaction.Amount == successTransferTransaction.Amount) {
            $http.post('../TransferTransaction/ThirdPartyPaymentTransferComplete', { Amount:successTransferTransaction.Amount,transferId: transferTransaction.ID, transferNo: successTransferTransaction.TransferNo, payway: transferTransaction.payway }).success(function (data, status, headers) {
                if (data.Code == 1) $modalInstance.close(true);

                else $modalInstance.close(false);
            });
        } else {
            $alert.Warn("输入金额有误")
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function failTransferTransactionCtrl($scope, $modalInstance, $http, $alert, transferTransaction) {

    $scope.set = function (text) {
        if (text !="") {
            $http.post('../TransferTransaction/ThirdPartyPaymentTransferFail', { transferId: transferTransaction.ID, reason: text, payway: transferTransaction.payway }).success(function (data, status, headers) {
                if (data.Code == 1) $modalInstance.close(true);
                else $modalInstance.close(false);
            });
        } else {
            $alert.Warn("输入内容不得为空")
        }
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};