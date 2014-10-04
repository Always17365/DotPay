function CNYDepositCtrl($scope, $http) {
    $scope.title = "充值管理";
    $scope.selectDeposit = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectDeposit = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchcondition = { amountsearch: $scope.amount_search, email: $scope.email_search, page: page };
        $http.post('../CNYDeposit/Deposit/GetDepositCountBySearch', searchcondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../CNYDeposit/Deposit/GetDepositBySearch', searchcondition).success(function (data, status, headers) {
            $scope.deposits = data;
        });
    }
    $scope.checkedChange = function (deposit) {
        for (index in $scope.deposits) {
            var cDeposit = $scope.deposits[index];
            if (cDeposit.ID != deposit.ID)
                cDeposit.Checked = false;
        }

        if (deposit.Checked) $scope.selectDeposit = deposit;
        else $scope.selectDeposit = undefined;
    }
    $scope.Undo = function () {
        var deposit = $scope.selectDeposit;
        var searchcondition = { depositID: deposit.ID, userID: deposit.UserID };
        $http.post('../CNYDeposit/Deposit/UndoCompleteCNYDeposit', searchcondition).success(function (data, status, headers) {
            $scope.search();
        })
    }

    $scope.search();
}
