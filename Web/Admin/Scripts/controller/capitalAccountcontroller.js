
function CapitalAccountCtrl($scope, $http, $sce, $modal, $alert) {
    $scope.title = "";
    $scope.selectcapitalAccount = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectcapitalAccount = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { name: $scope.name_Search, page: page };

        $http.post('../capitalAccount/GetCapitalAccountCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../capitalAccount/GetCapitalAccountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.capitalAccounts = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (capitalAccount) {
        for (index in $scope.capitalAccounts) {
            var _capitalAccount = $scope.capitalAccounts[index];
            if (_capitalAccount.ID != capitalAccount.ID)
                _capitalAccount.Checked = false;
        }

        if (capitalAccount.Checked) $scope.selectcapitalAccount = capitalAccount;
        else $scope.selectcapitalAccount = undefined;
    }


    $scope.enable = function () {
        if ($scope.selectcapitalAccount == undefined) $alert.Warn("未选择任何账号");
        else if ($scope.selectcapitalAccount.IsEnable == true) $alert.Warn("账号已被启用");
        else {
            var url = '../CapitalAccount/enable';
            $http.post(url, { capitalAccountID: $scope.selectcapitalAccount.ID }).success(function (data, status, headers) {
                $scope.pageChange(1);
            });
        }        
    }

    $scope.disable = function () {
        if ($scope.selectcapitalAccount == undefined) $alert.Warn("未选择任何账号");
        else if ($scope.selectcapitalAccount.IsEnable == false) $alert.Warn("账号已被锁定");
        else {
            var url = '../CapitalAccount/disable';
            $http.post(url, { capitalAccountID: $scope.selectcapitalAccount.ID }).success(function (data, status, headers) {
                $scope.pageChange(1);
            });
        }
    }


    $scope.openAddModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addcapitalAccountModalContent.html',
            controller: AddCapitalAccountModalCtrl,
            backdrop: false,
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });
    }
    $scope.search();
}

function AddCapitalAccountModalCtrl($scope, $modalInstance, $http, $alert) {
    $scope.capitalAccountTypes = [];

    $http.get('capitalAccount/GetCapitalAccountType').success(function (data, status, headers) {
        $scope.capitalAccountTypes = data;
    });

    $scope.add = function (capitalAccount, capitalAccountType) {
        if (this.capitalAccountForm.$valid) {
            capitalAccount.Bank = capitalAccountType.Value;
            console.log(capitalAccount);
            $http.post('../capitalAccount/Create', capitalAccount).success(function (data, status, headers) {

                    $modalInstance.close(true);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};