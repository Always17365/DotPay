
function DeptMarketCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    var title = $routeParams.basecurrency + '_' + $routeParams.targetcurrency;
    switch (title)
    {
        case "BTC_LTC":
            $scope.title = "BTC_LTC交易市场深度";
            break;
        case "BTC_IFC":
            $scope.title = "BTC_IFC交易市场深度";
            break;
        case "BTC_NXT":
            $scope.title = "BTC_NXT交易市场深度";
            break;
        case "BTC_DOGE":
            $scope.title = "BTC_DOGE交易市场深度";
            break;
        case "CNY_BTC":
            $scope.title = "CNY_BTC交易市场深度";
            break;
        case "CNY_LTC":
            $scope.title = "CNY_LTC交易市场深度";
            break;
        case "CNY_IFC":
            $scope.title = "CNY_IFC交易市场深度";
            break;
        case "CNY_NXT":
            $scope.title = "CNY_NXT交易市场深度";
            break;
        case "CNY_DOGE":
            $scope.title = "CNY_DOGE交易市场深度";
            break;
        case "LTC_DOGE":
            $scope.title = "LTC_DOGE交易市场深度";
            break;
        case "LTC_IFC":
            $scope.title = "LTC_IFC交易市场深度";
            break;
        case "LTC_NXT":
            $scope.title = "LTC_NXT交易市场深度";
            break;
    }	
    $scope.currentPage = 1;
    $scope.basecurrency = $routeParams.basecurrency;
    $scope.targetcurrency = $routeParams.targetcurrency;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);
        $scope.selectDepthMarket = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { basecurrency: $scope.basecurrency, targetcurrency: $scope.targetcurrency, page: page };

        $http.post('../depthmarket/GetDepthMarketCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../depthmarket/GetDepthMarketBySearch', searchCondition).success(function (data, status, headers) {
            $scope.depthMarkets = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };


    $scope.search();
}
