
function MarketTransactionRecordsCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    var title = $routeParams.basecurrency + '_' + $routeParams.targetcurrency;
    switch (title)
    {
        case "BTC_LTC":
            $scope.title = "BTC_LTC交易记录";
            break;
        case "BTC_IFC":
            $scope.title = "BTC_IFC交易记录";
            break;
        case "BTC_NXT":
            $scope.title = "BTC_NXT交易记录";
            break;
        case "BTC_DOGE":
            $scope.title = "BTC_DOGE交易记录";
            break;
        case "CNY_BTC":
            $scope.title = "CNY_BTC交易记录";
            break;
        case "CNY_LTC":
            $scope.title = "CNY_LTC交易记录";
            break;
        case "CNY_IFC":
            $scope.title = "CNY_IFC交易记录";
            break;
        case "CNY_NXT":
            $scope.title = "CNY_NXT交易记录";
            break;
        case "CNY_DOGE":
            $scope.title = "CNY_DOGE交易记录";
            break;
        case "LTC_DOGE":
            $scope.title = "LTC_DOGE交易记录";
            break;
        case "LTC_IFC":
            $scope.title = "LTC_IFC交易记录";
            break;
        case "LTC_NXT":
            $scope.title = "LTC_NXT交易记录";
            break;
    }	
    $scope.currentPage = 1;
    $scope.basecurrency = $routeParams.basecurrency;
    $scope.targetcurrency = $routeParams.targetcurrency;

    $scope.search = function () {
        $scope.pageChange($scope.currentPage);
        $scope.selectMarketTransactionRecords = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { basecurrency: $scope.basecurrency, targetcurrency: $scope.targetcurrency, page: page };

        $http.post('../markettransactionrecords/GetMarketTransactionRecordsCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../markettransactionrecords/GetMarketTransactionRecordsBySearch', searchCondition).success(function (data, status, headers) {
            $scope.marketTransactionRecordss = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };


    $scope.search();
}
