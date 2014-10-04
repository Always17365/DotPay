
function SmsInterfaceCtrl($scope, $http, $sce, $modal, $alert, $location) {
    $scope.title = "互亿无线短信接口设置";


    $scope.loadSetting = function (page) {
        $http.post('../SmsInterface/GetSmsInterface').success(function (data, status, headers) {
            $scope.account = data.Account;
            $scope.password = data.Password;
        });
    }


    $scope.modify = function (account, password) {
        $http.post('../SmsInterface/CreateSmsInterface', { account: account, password: password }).success(function (data, status, headers) {
            if (data.Code == 1) $alert.Notice("保存成功！");
            else if (data.Code == -1) $alert.Notice("账号不能为空！");
            else if (data.Code == -2) $alert.Notice("密码不能为空！");
            else if (data.Code == -3) $alert.Notice("无权进行此操作！");
        });
    }

    $scope.loadBalance = function (account, password) {
        $http.post('../SmsInterface/CreateSmsInterface', { account: account, password: password }).success(function (data, status, headers) {
            if (data.Code == 1) $alert.Notice("保存成功！");
            else if (data.Code == -1) $alert.Notice("账号不能为空！");
            else if (data.Code == -2) $alert.Notice("密码不能为空！");
            else if (data.Code == -3) $alert.Notice("无权进行此操作！");
        });
    }
    $scope.loadSetting();
}
