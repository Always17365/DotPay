
function LogsCtrl($scope, $http, $sce, $modal, $alert, $location, $routeParams) {
    $scope.selectLogs = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectLogs = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { action: $scope.action, page: page, email: $scope.Email_Search };

        $http.post('../Logs/GetLogsCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../Logs/GetLogsBySearch', searchCondition).success(function (data, status, headers) {
            $scope.logss = data;
        });
    }

    $scope.checkedChange = function (logs) {
        for (index in $scope.logss) {
            var cLogs = $scope.logss[index];
            if (cLogs.ID != logs.ID)
                cLogs.Checked = false;
        }

        if (logs.Checked) $scope.selectLogs = logs;
        else $scope.selectLogs = undefined;
    }


    $scope.search();
}

function LoginCtrl($scope, $http, $sce, $modal, $alert, $location) {
    $scope.selectLogs = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectLogs = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page, email: $scope.Email_Search };

        $http.post('../Logs/GetUserLoginCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../Logs/GetUserLoginBySearch', searchCondition).success(function (data, status, headers) {
            $scope.logins = data;
        });
    }

    $scope.checkedChange = function (logs) {
        for (index in $scope.logss) {
            var cLogs = $scope.logss[index];
            if (cLogs.ID != logs.ID)
                cLogs.Checked = false;
        }

        if (logs.Checked) $scope.selectLogs = logs;
        else $scope.selectLogs = undefined;
    }


    $scope.search();
}