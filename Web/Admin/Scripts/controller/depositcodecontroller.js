
function DepositCodeCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.selectDepositCode = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;
    $scope.state = $routeParams.state;    
    $scope.range = $routeParams.range;
    switch ($scope.action) {
        case 'cny':
            $scope.title = "CNY充值码";
            break;
        case 'btc':
            $scope.title = "BTC充值码";
            break;
        case 'doge':
            $scope.title = "DOGE充值码";
            break;
        case 'ifc':
            $scope.title = "IFC充值码";
            break;
        case 'ltc':
            $scope.title = "LTC充值码";
            break;
        case 'nxt':
            $scope.title = "NXT充值码";
            break;
        case 'str':
            $scope.title = "STR充值码";
            break;
        default:
            $scope.title = $scope.action + "充值码";

    }

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectDepositCode = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { FromAccountID: $scope.FromAccountID_Search, state: $scope.state, currencyType: $scope.action, page: page, range: $scope.range, starttime: $scope.start_Search, endtime: $scope.end_Search };
        $http.post('../user/seeUserDepositAmoun').success(function (data, status, headers) {
            $scope.moneys = data;
        });
        if ($scope.state == "use")
        {
            if ($scope.range == "o") {
                var url1 = '../depositCode/GetUseDepositCodeCountBySearch';
                var url2 = '../depositCode/GetUseDepositCodeBySearch';
            } else {
                var url1 = '../depositCode/GetAllUseDepositCodeCountBySearch';
                var url2 = '../depositCode/GetAllUseDepositCodeBySearch';
            }
        }
        else if ($scope.state == "notused")
        {
            if ($scope.range == "o") {
                var url1 = '../depositCode/GetDepositCodeCountBySearch';
                var url2 = '../depositCode/GetDepositCodeBySearch';
            } else {
                var url1 = '../depositCode/GetAllDepositCodeCountBySearch';
                var url2 = '../depositCode/GetAllDepositCodeBySearch';
            }
        }
        else if ($scope.state == "del")
        {
            var url1 = '../depositCode/GetDestroyDepositCodeCountBySearch';
            var url2 = '../depositCode/GetDestroyDepositCodeBySearch';
        }


        $http.post(url1, searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post(url2, searchCondition).success(function (data, status, headers) {
            $scope.depositCodes = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (DepositCode) {
        for (index in $scope.depositCodes) {
            var _DepositCode = $scope.depositCodes[index];
            if (_DepositCode.ID != DepositCode.ID)
                _DepositCode.Checked = false;
        }

        if (DepositCode.Checked) $scope.selectDepositCode = DepositCode;
        else $scope.selectDepositCode = undefined;
    }

    $scope.generate = function () {
        var modalInstance = $modal.open({
            templateUrl: 'GenerateDepositCode.html',
            controller: GenerateCtrl,
            backdrop: false,
            resolve: {
                action: function () { return $scope.action; }
            }
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });

    }
    $scope.makeFunds = function () {
        if ($scope.selectDepositCode) {
            var modalInstance = $modal.open({
                templateUrl: 'MakeFundsDepositCode.html',
                controller: MakeFundsDepositCodeCtrl,
                backdrop: false,
                resolve: {
                    action: function () { return $scope.action; },
                    selectDepositCode: function () { return $scope.selectDepositCode }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else $alert.Warn("请先选择数据!")

    }
    $scope.destory = function () {
        if ($scope.selectDepositCode) {
            var url = '../depositCode/DestoryDepositCode';
            $http.post(url, { depositCodeID: $scope.selectDepositCode.ID, CurrencyType: $scope.action }).success(function (data, status, headers) {
                $scope.pageChange($scope.currentPage);
            });
        }
    }

    $scope.search();
}

function GenerateCtrl($scope, $modalInstance, $alert, $http, action) {

    $scope.save = function (amount, password) {
        var url = '../depositCode/GenerateDepositCode';
        $http.post(url, { amount: amount, password: password, currencytype: action }).success(function (data, status, headers) {
            if (data.Code == 1)
                $modalInstance.close(true);
            else {
                $modalInstance.close(false);
                $alert.Warn("生成充值码失败,可能是额度不足或者密码错误!");
            }

        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function MakeFundsDepositCodeCtrl($scope, $modalInstance, $alert, $http, action, selectDepositCode) {

    $scope.capitalAccountTypes = [];
    $http.post('../depositCode/GetCapitalAccount').success(function (data, status, headers) {
        $scope.getCapitalAccounts = data;
    });

    $scope.makeFunds = function (CapitalAccountID, TransferTxNo, FundSourceAmount) {
        var url = '../depositCode/MarkCNYDepositCodeFundSource';
        $http.post(url, { cnyDepositCodeID: selectDepositCode.ID, capitalAccountID: CapitalAccountID.ID, transferTxNo: TransferTxNo, fundSourceAmount: FundSourceAmount }).success(function (data, status, headers) {
            if (data.Code == 1)
                $modalInstance.close(true);
            else {
                $modalInstance.close(false);
                $alert.Warn("标记失败!");
            }
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};