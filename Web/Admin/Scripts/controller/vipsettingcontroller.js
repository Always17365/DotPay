
function VipSettingCtrl($scope, $window, $http, $sce, $modal, $alert) {
    $scope.selectVipSetting = undefined;
    $scope.contentdb = "";
    $scope.currentPage = 1;

    $scope.search = function () {
        $scope.pageChange(1);
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page };

        $http.post('../VipSetting/GetVipSettingBySearch', searchCondition).success(function (data, status, headers) {
            $scope.vipsettings = data;
        });
    }

    $scope.checkedChange = function (VipSetting) {
        for (index in $scope.VipSettings) {
            var _VipSetting = $scope.VipSettings[index];
            if (_VipSetting.ID != VipSetting.ID)
                _VipSetting.Checked = false;
        }

        if (VipSetting.Checked) $scope.selectVipSetting = VipSetting;
        else $scope.selectVipSetting = undefined;
    }


    $scope.setVipLevel = function (action) {      
        if ($scope.selectVipSetting) {
            var modalInstance = $modal.open({
                templateUrl: 'vipSettingModalContent.html',
                controller: vipSettingModalContent,
                backdrop: false,
                resolve: {           
                    selectVipSetting: function () { return $scope.selectVipSetting; },
                    action: function () { return action; }                
                }
            });
            modalInstance.result.then(function (result) {
                $scope.pageChange($scope.currentPage);
            }, function (reason) {
                $scope.pageChange($scope.currentPage);
            });
        }
        else $alert.Warn("请选择有效数据")

    }
    
        $scope.search();
    
}

function vipSettingModalContent($scope, $modalInstance, $http, $alert, selectVipSetting, action) {
   $scope.selectVipSetting = selectVipSetting;
   switch (action)
    {
        case 'Discount':
            $scope.title = "设置折扣";
            break;
        case 'ScoreLine':
            $scope.title = "设置积分线";
            break;
        case 'VoteCount':
            $scope.title = "设置票数";
            break;

    }
    $scope.set = function (value) {
        var url = '../vipsetting/SetVipLevel' + action;
        $http.post(url, { vipLevelId: selectVipSetting.ID, value: value }).success(function (data, status, headers) {
            if (data.Code == 1)
                $modalInstance.close(true);
            else{
                $alert.Warn("操作失败!")
                $modalInstance.close(false);
            }
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};