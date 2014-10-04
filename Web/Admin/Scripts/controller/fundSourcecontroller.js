
function FundSourceCtrl($scope, $http, $sce, $modal, $alert, $routeParams) {
    $scope.title = "";
    $scope.selectfundSource = undefined;
    $scope.currentPage = 1;
    $scope.action = $routeParams.action;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectfundSource = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { amount: $scope.amount_Search, transfer: $scope.transfer_Search, fundSourceState: $scope.action, page: page };

        $http.post('../fundSource/GetfundSourceCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../fundSource/GetfundSourceBySearch', searchCondition).success(function (data, status, headers) {
            $scope.fundSources = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (fundSource) {
        for (index in $scope.fundSources) {
            var _fundSource = $scope.fundSources[index];
            if (_fundSource.ID != fundSource.ID)
                _fundSource.Checked = false;
        }

        if (fundSource.Checked) $scope.selectfundSource = fundSource;
        else $scope.selectfundSource = undefined;
    }

    $scope.openAddModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addfundSourceModalContent.html',
            controller: AddFundSourceModalCtrl,
            backdrop: false,
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });
    }

    $scope.openAddRecharge = function () {
        var selectID = $scope.selectfundSource;
        if (selectID) {
            var modalInstance = $modal.open({
                templateUrl: 'rechargefundSourceModalContent.html',
                controller: rechargeFundSourceModalCtrl,
                backdrop: false,
                resolve: {
                    content: function () { return selectID;}
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        } else {
            $alert.Warn("未选择订单")
        }
    }

    $scope.delete = function () {
        if ($scope.selectfundSource == undefined) $alert.Warn("未选择任何账号");
        else {
            var url = '../fundSource/delete';
            $http.post(url, { fundSourceID: $scope.selectfundSource.ID }).success(function (data, status, headers) {
                $scope.search();
            });
        }
    }

    $scope.verify = function () {
        if ($scope.selectfundSource == undefined) $alert.Warn("未选择任何账号");
        else {
            var url = '../fundSource/Verify';
            $http.post(url, { fundSourceID: $scope.selectfundSource.ID }).success(function (data, status, headers) {
                $scope.search();
            });
        }
    }

    $scope.undoVerify = function () {
        if ($scope.selectfundSource == undefined) $alert.Warn("未选择任何账号");
        else {
            var url = '../fundSource/UndoVerify';
            $http.post(url, { fundSourceID: $scope.selectfundSource.ID }).success(function (data, status, headers) {
                $scope.search();
            });
        }
    }

    $scope.search();
}

function AddFundSourceModalCtrl($scope, $modalInstance, $http, $alert) {
    $scope.fundSourceTypes = [];
    $scope.bankAccount = [];

    $http.post('fundSource/GetfundSourceType').success(function (data, status, headers) {
        $scope.fundSourceTypes = data;
    });
    $http.post('fundSource/BankAccount').success(function (data, status, headers) {
        $scope.bankAccount = data;
    });

    $scope.add = function (fundSource, fundSourceType, bankAccountType) {
        if (this.fundSourceForm.$valid) {
            fundSource.bank = fundSourceType.Value;
            fundSource.capitalAccountID = bankAccountType.ID;
            fundSource.payway = 3;
            $http.post('../fundSource/Create', fundSource).success(function (data, status, headers) {
                $modalInstance.close(true);
                $scope.pageChange(1);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

function rechargeFundSourceModalCtrl($scope, $modalInstance, $http, $alert, content) {

    $scope.content = content;
    $scope.add = function (content) {
        if (this.fundRechargeForm.$valid) {
            $http.post('../fundSource/AccountPrepaid', content).success(function (data, status, headers) {
                $modalInstance.close(true);
                $scope.pageChange(1);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};


