
function CurrencyCtrl($scope, $http, $sce, $modal, $alert) {
    $scope.title = "币种列表";
    $scope.selectCurrency = undefined;
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectCurrency = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { code: $scope.code_Search, name: $scope.name_Search, page: page };

        $http.post('../Currency/GetCurrencyCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../Currency/GetCurrenciesBySearch', searchCondition).success(function (data, status, headers) {
            $scope.currencies = data;
        });
    }

    $scope.successAction = function () {
        $alert('Success alert', "success");
    };

    $scope.checkedChange = function (currency) {
        for (index in $scope.currencies) {
            var _currency = $scope.currencies[index];
            if (_currency.ID != currency.ID)
                _currency.Checked = false;
        }

        if (currency.Checked) $scope.selectCurrency = currency;
        else $scope.selectCurrency = undefined;
    }

    $scope.openAddModal = function () {
        var modalInstance = $modal.open({
            templateUrl: 'addCurrencyModalContent.html',
            controller: AddCurrencyModalCtrl,
            backdrop: false,
        });
        modalInstance.result.then(function (result) {
            if (result == true) {
                $scope.pageChange($scope.currentPage);
            }
        });
    }

    $scope.enable = function () {
        var currency = $scope.selectCurrency
        if (currency) {
            $http.post('../Currency/Enable', { currencyID: currency.ID }).success(function (data, status, headers) {
                $scope.search();
            });
        }
        else $alert.Warn("未选择有效数据")
    }

    $scope.disable = function () {
        var currency = $scope.selectCurrency
        if (currency) {
            $http.post('../Currency/Disable', { currencyID: currency.ID }).success(function (data, status, headers) {
                 $scope.search();
            });
        }
        else $alert.Warn("未选择有效数据")
    }

    $scope.changeDeposit = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeDepositModalContent.html',
                controller: ModifyCurrencyDepositFeeRate,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }

    $scope.changeWithdraw = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeWithdrawModalContent.html',
                controller: ModifyCurrencyWithdrawFeeRate,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }

    $scope.changeOnceLimit = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeOnceLimitModalContent.html',
                controller: ModifyCurrencyWithdrawOnceLimit,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }

    $scope.changeDayLimit = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeDayLimitModalContent.html',
                controller: ModifyCurrencyWithdrawDayLimit,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }

    $scope.changeVerifyLine = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeVerifyLineModalContent.html',
                controller: ModifyCurrencyWithdrawVerifyLine,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }
    $scope.changeOnceMin = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeWithdrawOnceMinModalContent.html',
                controller: ModifyCurrencyWithdrawOnceMin,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }
    $scope.changeNeedConfirm = function (currency) {
        if (currency) {
            var modalInstance = $modal.open({
                templateUrl: 'changeNeedConfirmModalContent.html',
                controller: changeNeedConfirmModalContent,
                backdrop: false,
                resolve: {
                    currency: function () {
                        return currency;
                    }
                }
            });
            modalInstance.result.then(function (result) {
                if (result == true) {
                    $scope.pageChange($scope.currentPage);
                }
            });
        }
        else $alert.Warn("未选择任有效信息")
    }
    
    $scope.search();
}

function AddCurrencyModalCtrl($scope, $modalInstance, $http, $alert) {
    $scope.currentTypes = [];

    $http.get('Currency/GetCurrencyType').success(function (data, status, headers) {
        $scope.currentTypes = data;
    });

    $scope.add = function (currency, currencyType) {
        if (this.currencyForm.$valid) {
            currency.Code = currencyType.Key;
            currency.CurrencyID = currencyType.Value;
            $http.post('../Currency/Create', currency).success(function (data, status, headers) {
                if (data.Code == -1) {
                    $alert.Danger("币种" + currency.Code + "已经存在了");
                }
                else if (data.Code == -2) {
                    $alert.Danger("名称" + currency.Name + "已经存在了");
                }
                else
                    $modalInstance.close(true);
            });
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

function ModifyCurrencyDepositFeeRate($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyDepositFeeRate", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });

    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function ModifyCurrencyWithdrawFeeRate($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyWithdrawFeeRate", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function ModifyCurrencyWithdrawOnceLimit($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.currency.onceLimit = currency.WithdrawOnceLimit;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyWithdrawOnceLimit", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function ModifyCurrencyWithdrawDayLimit($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.currency.dayLimit = currency.WithdrawDayLimit;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyWithdrawDayLimit", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function ModifyCurrencyWithdrawVerifyLine($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.currency.verifyLine = currency.WithdrawVerifyLine;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyWithdrawVerifyLine", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function ModifyCurrencyWithdrawOnceMin($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.currency.WithdrawOnceMin = currency.WithdrawOnceMin;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyWithdrawOnceMin", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
function changeNeedConfirmModalContent($scope, $modalInstance, $http, $alert, currency) {
    $scope.currency = currency;
    $scope.currency.currencyID = currency.ID;
    $scope.currency.num = currency.NeedConfirm;
    $scope.change = function (currency) {
        $http.post("../Currency/ModifyCurrencyNeedConfirm", currency).success(function (data, status, headers) {
            $modalInstance.close(true);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};