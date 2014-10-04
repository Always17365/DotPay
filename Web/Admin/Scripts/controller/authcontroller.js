var module = angular.module('authApp', ['AlertBox']);

function AuthCtrl($scope, $http, $alert, $window) {
    $scope.loginSuccess = false;
    $scope.openGA = false;
    $scope.openSMS = false;

    $scope.login = function (passport) {
        $http.post('../Auth/Login', { email: passport.account, password: passport.password })
             .success(function (data, status, headers) {
                 if (data.Code == -1) { $alert.Danger("用户名为非法的Email地址"); return; }
                 if (data.Code == -2) { $alert.Danger("密码不能为空"); return; }
                 if (data.Code == -3) { $alert.Danger("用户名或密码错误"); return; }
                 if (data.Code == 1) { $alert.Danger("未开启双重身份认证，不允许登录"); return; }
                 if ((data.Code & 2) == 2) { $scope.loginSuccess = true; $scope.openGA = true; }
                 if ((data.Code & 4) == 4) { $scope.loginSuccess = true; $scope.openSMS = true; }
             });
    }

    $scope.verifyTwoFactor = function (twofactor) {
        $http.post('../Auth/VerifyTwoFactor', { ga: twofactor.ga, sms: twofactor.sms })
             .success(function (data, status, headers) {
                 if ((data.Code & 1) == 1) $alert.Danger("谷歌身份验证码错误");
                 if ((data.Code & 2) == 2) $alert.Danger("短信验证码错误");
                 if ((data.Code & 8) == 8) $window.location.href = "/Index"
             });
    }
}
