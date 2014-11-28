
function GetTransferTransactionCtrl($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectTransferTransaction = undefined;
    $scope.currentPage = 1;
    $scope.state = $routeParams.state;
    $scope.payway = $routeParams.payway;
    $scope.title = $routeParams.payway;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);
       
    }
    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { state: $scope.state, payway: $scope.payway, page: page };
        $http.post('../TransferTransaction/GetPendingTransferTransaction', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data.count;
            $scope.transferTransactions = data.result;
        });
    }
    $scope.checkedChange = function (transferTransaction) {
        for (index in $scope.transferTransaction) {
            var _transferTransaction = $scope.bankinfos[index];
            if (_transferTransaction.ID != transferTransaction.ID)
                _transferTransaction.Checked = false;
        }

        if (bankinfo.Checked) $scope.selectTransferTransaction = transferTransaction;
        else $scope.selectTransferTransaction = undefined;
    }
    $scope.success = function () {
        var transferTransaction = $scope.selectTransferTransaction
        if (transferTransaction) {
            var modalInstance = $modal.open({
                templateUrl: 'seeUserDepositAmount.html',
                controller: seeUserDepositAmountCtrl,
                backdrop: true,
                resolve: {
                    transferTransaction: function () {
                        return transferTransaction;
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
    $scope.search();
}
function seeUserDepositAmountCtrl($scope, $modalInstance, $http, user) {
    $scope.user = user;
    $http.post('../user/seeUserDepositAmoun', { userID: $scope.user.UserID }).success(function (data, status, headers) {
        $scope.servers = data;
    });
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};