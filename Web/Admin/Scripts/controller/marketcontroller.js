
function MarketCtrl($scope, $http, $sce, $modal, $alert) {
    $scope.title = "交易市场设置";
    $scope.selectMarket = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectMarket = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page };

        $http.post('../Market/CountMarketBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../Market/GetMarketBySearch', searchCondition).success(function (data, status, headers) {
            $scope.markets = data;
        });
    }

    $scope.checkedChange = function (market) {
        for (index in $scope.markets) {
            var _market = $scope.markets[index];
            if (_market.ID != market.ID)
                _market.Checked = false;
        }

        if (market.Checked) $scope.selectMarket = market;
        else $scope.selectMarket = undefined;
    }

    $scope.openSetModal = function () {
        var market = $scope.selectMarket
        if (market) {
            var modalInstance = $modal.open({
                templateUrl: 'ModifyMarketTradeSetModalContent.html',
                controller: SetMarketModalCtrl,
                backdrop: false,
                resolve: {
                    market: function () {
                        return market;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("请选择交易市场")

    }

    $scope.closeMarket = function (marketID) {
        $http.post('../Market/CloseMarket', { marketID: marketID }).success(function (data, status, headers) {
            if (data.Code == 1)
                $scope.search();
            else $alert.Warn("关闭市场出错");
        });
    }

    $scope.openMarket = function (marketID) {
        $http.post('../Market/OpenMarket', { marketID: marketID }).success(function (data, status, headers) {
            if (data.Code == 1)
                $scope.search();
            else $alert.Warn("开放市场出错");
        });
    }

    $scope.search();
}


function VirtualCoinDecimalPlacesCtrl($scope, $http, $sce, $modal, $alert) {
    $scope.title = "虚拟货币交易市场小数点格式化设置";
    $scope.selectMarket = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectMarket = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page };
        //$http.post('../Market/CountMarketBySearch', searchCondition).success(function (data, status, headers) {
        //    $scope.totalCount = data;
        //});
        $http.post('../Market/GetMarketBySearch', searchCondition).success(function (data, status, headers) {
            $scope.markets = data;
        });
    }

    $scope.checkedChange = function (market) {
        for (index in $scope.markets) {
            var _market = $scope.markets[index];
            if (_market.ID != market.ID)
                _market.Checked = false;
        }

        if (market.Checked) $scope.selectMarket = market;
        else $scope.selectMarket = undefined;
    }

    $scope.openSetModal = function () {
        var market = $scope.selectMarket
        if (market) {
            var modalInstance = $modal.open({
                templateUrl: 'MarketSettingModalContent.html',
                controller: MarketSettingModalCtrl,
                backdrop: false,
                resolve: {
                    market: function () {
                        return market;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                    $scope.pageChange($scope.currentPage);
            }, function (reason) {
                $scope.pageChange($scope.currentPage);
            });
        }
        else $alert.Warn("请选择交易市场")

    }
    $scope.search();
}
function MarketSettingModalCtrl($scope, $modalInstance, $http, $alert, market) {
    $scope.marketSetting = market;
    $scope.save = function (marketSetting) {
        $http.post("../market/changemarketsetting", marketSetting).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};