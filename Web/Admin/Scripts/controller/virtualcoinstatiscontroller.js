
function VirtualCoinStatisCtrl($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectVirtualCoinStatis = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;
    $scope.title = $routeParams.action;

    $scope.search = function () {
        $http.post('../VirtualCoinStatis/GetResults', { currencyType: $scope.action }).success(function (data, status, headers) {
            $scope.virtualCoinStatiss = data;
        });
    }

    $scope.search();
}