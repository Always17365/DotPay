
function IndexCtrl($scope, $modal) {

    $scope.modifyPassword = function () {
        var modalInstance = $modal.open({
            templateUrl: 'Home/ModalModifyPassword',
            controller: ModifyPasswordModalCtrl,
            backdrop: 'static'
        });
    };
    $scope.logout = function () {
        console.log("用户ID=" + $scope.userId + "退出登录!");
    }
}

function ModifyPasswordModalCtrl($scope, $modalInstance) {
    $scope.save = function () {
        console.log("save");
    };


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};


function LeftNavCtrl($scope, $http, $sce, $window) {

    $scope.frameSrc = $sce.trustAsResourceUrl("../Admin/Index/Welcome");
    $http.post('../base/GetGroupList').success(function (data, status, headers) {
        //data = (data.replace(/"/g, ""));
        //var jsonObj = (data.replace(/\*/g, '"'));
        //var json = $scope.$eval('([' + jsonObj + '])');
        var json = data;
        /*****************************************************************/

        var newdata = new Array();
        var num = 0
        for (var i = 0; i < json.length; i++) {
            if (typeof (json[i]) == 'undefined') {
                continue;
            }
            newdata[num] = new Object();
            newdata[num].title = json[i].title;
            newdata[num]['items'] = new Array();
            for (var b = i; b < json.length; b++) {
                if (typeof (json[b]) == "undefined") {
                    continue;
                }
                if (json[b].title == newdata[num].title) {
                    var x = newdata[num]['items'].length;
                    for (var z = 0; z < json[b].items.length; z++) {
                        if (x > 0) {
                            for (var zz = 0; zz < newdata[num].items.length; zz++) {
                                if (newdata[num].items[zz].name == json[b].items[z].name) {
                                    break;
                                }
                                newdata[num]['items'][x + z] = new Object();
                                newdata[num]['items'][x + z].name = json[b].items[z].name;
                                newdata[num]['items'][x + z].url = json[b].items[z].url;
                            }
                        } else {
                            newdata[num]['items'][z] = new Object();
                            newdata[num]['items'][z].name = json[b].items[z].name;
                            newdata[num]['items'][z].url = json[b].items[z].url;
                        }
                    }
                    delete json[b];
                }
            }
            num++;
        }
        /*****************************************************************************************/
        $scope.groups = newdata;
    });



    $scope.navTo = function (item) {
        $scope.frameSrc = $sce.trustAsResourceUrl(item.url);
    }
}