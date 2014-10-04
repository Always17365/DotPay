
function BankOutletsCtrl($scope, $http, $sce, $modal, $alert, $routeParams)
{
    $scope.title = "";
    $scope.selectbankinfo = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;


    $http.get('capitalAccount/GetCapitalAccountType').success(function (data, status, headers) {
        $scope.bankAccountTypes = data;
    });
    $http.post('../cnywithdraw/Withdraw/GetProvince').success(function (data, status, headers) {
        $scope.provinces = data;
    });
    $scope.getCity = function (ID) {
        $http.post('../cnywithdraw/withdraw/getcity', { fatherid: ID }).success(function (data, status, headers) {
            $scope.citys = data;
        });
    };

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectbankinfo = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { bankType: $scope.bankType ? $scope.bankType.Value : $scope.bankType, province: $scope.province? $scope.province.ID : $scope.province, city: $scope.city ? $scope.city.ID : $scope.city, page: page };

        $http.post('../bankoutlets/GetBankInfoCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../bankoutlets/GetBankInfoBySearch', searchCondition).success(function (data, status, headers) {
            $scope.bankinfos = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (bankinfo) {
        for (index in $scope.bankinfos) {
            var _bankinfo = $scope.bankinfos[index];
            if (_bankinfo.ID != bankinfo.ID)
                _bankinfo.Checked = false;
        }

        if (bankinfo.Checked) $scope.selectbankinfo = bankinfo;
        else $scope.selectbankinfo = undefined;
    }
    $scope.add = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addbankinfoModalContent.html',
            controller: AddBankInfoModalCtrl,
            backdrop: false,
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });
    }
    $scope.delete = function () {
        if ($scope.selectbankinfo) {
            $http.post('../bankoutlets/removebankoutlets', { bankOutletsId: $scope.selectbankinfo.ID}).success(function (data, status, headers) {
                $scope.pageChange($scope.currentPage);
            });
        }
        else $alert.Warn("未选择任何信息")
    }
    $scope.search();
}

function AddBankInfoModalCtrl($scope, $modalInstance, $http, $alert) {
    $scope.fundSourceTypes = [];
    $scope.bankAccount = [];
    $http.get('capitalAccount/GetCapitalAccountType').success(function (data, status, headers) {
        $scope.bankAccountTypes = data;
    });
    $http.post('../cnywithdraw/Withdraw/GetProvince').success(function (data, status, headers) {
        $scope.provinces = data;
    });
    $scope.getCity = function (ID) {
        $http.post('../cnywithdraw/withdraw/getcity', { fatherid: ID }).success(function (data, status, headers) {
            $scope.citys = data;
        });
    };
    $scope.save = function (province, city, bankType, bankName) {
        if (this.addBankInfoModalForm.$valid) {
            $http.post('../bankoutlets/CreateBankOutlets', { bank: bankType.Value, provinceID: province.ID, cityID: city.ID, bankName: bankName }).success(function (data, status, headers) {
                $modalInstance.close(true);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

