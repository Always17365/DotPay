var module = angular.module('mgApp', ['ui.bootstrap', 'ngRoute', 'AlertBox']);

module.directive('ncgRequestVerificationToken', ['$http', function ($http) {
    return function (scope, element, attrs) {
        $http.defaults.headers.common['RequestVerificationToken'] = attrs.ncgRequestVerificationToken || "no request verification token";
    };
}]);

module.directive('ueditor', [function () {
    return {
        restrict: 'A'
        , scope:false
        , link: function (scope, element, attrs) {
            var editor = new UE.ui.Editor({
                initialContent: scope.content,
            });
            editor.render(element[0]);

            editor.ready(function () {
                editor.addListener('contentChange', function () {
                    scope.content = editor.getContent();
                    scope.$root.$$phase || scope.$apply();
                });
            });

            scope.$watch('contentdb', function (newValue) {
                editor.setContent(newValue);
            });
        }
};
}])


module.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
    .when('/index', { templateUrl: 'Content/templete/welcome.html' })

    .when('/user', { templateUrl: '/User/Index', controller: 'UserCtrl' })
    .when('/userlock/:action', { templateUrl: '/User/Lock', controller: 'UserCtrl' })
    .when('/manager', { templateUrl: '/User/Manager', controller: 'ManagerCtrl' })
    .when('/customerservice', { templateUrl: '/User/CustomerService', controller: 'CustomerServiceCtrl' })
    .when('/depositcodemakelog/:action/:nickname', { templateUrl: '/User/depositcodemakelog', controller: 'DepositCodeMakeLogCtrl' })
    .when('/useraccountinfo/:currencyType', { templateUrl: '/User/UserInfo', controller: 'GetUserAccountInfoCtrl' })

    .when('/smsinterface', { templateUrl: '/SmsInterface/Index', controller: 'SmsInterfaceCtrl' })
    .when('/currency', { templateUrl: '/Currency/Index', controller: 'CurrencyCtrl' })
    .when('/list/:id', { templateUrl: 'views/route/detail.html', controller: 'RouteDetailCtl' })
    .when('/deposit', { templateUrl: '/CNYDeposit/Deposit/depositCNY', controller: 'CNYDepositCtrl' })
    .when('/capitalaccount', { templateUrl: '/CapitalAccount/index', controller: 'CapitalAccountCtrl' })
    .when('/depositwaitverify/:action', { templateUrl: '/FundSource/depositindex', controller: 'FundSourceCtrl' })
    .when('/vipsetting', { templateUrl: '/vipsetting/index', controller: 'VipSettingCtrl' })
    .when('/VirtualCoinDecimalPlaces', { templateUrl: '/market/marketsetting', controller: 'VirtualCoinDecimalPlacesCtrl' })
 
    .when('/bankwaitverify/:action', { templateUrl: '/FundSource/index', controller: 'FundSourceCtrl' })
    .when('/bankverify/:action', { templateUrl: '/FundSource/examine', controller: 'FundSourceCtrl' })
    .when('/bankundoverify/:action', { templateUrl: '/FundSource/undoexamine', controller: 'FundSourceCtrl' })
    .when('/bankrecharge/:action', { templateUrl: '/FundSource/recharge', controller: 'FundSourceCtrl' })
    .when('/bankcomplete/:action', { templateUrl: '/FundSource/complete', controller: 'FundSourceCtrl' })

    .when('/bankinfo', { templateUrl: '/BankOutlets/bankinfo', controller: 'BankOutletsCtrl' })
    .when('/withdraw_wv/:action', { templateUrl: '/cnywithdraw/Withdraw/examine', controller: 'WithdrawCtrl' })
    .when('/withdraw_e/:action', { templateUrl: '/cnywithdraw/Withdraw/waitsubmit', controller: 'WithdrawCtrl' })
    .when('/withdraw_p/:action', { templateUrl: '/cnywithdraw/Withdraw/processing', controller: 'WithdrawCtrl' })
    .when('/withdraw_c/:action', { templateUrl: '/cnywithdraw/Withdraw/complete', controller: 'WithdrawCtrl' })
    .when('/withdraw_f/:action', { templateUrl: '/cnywithdraw/Withdraw/UndoExamine', controller: 'WithdrawCtrl' })
        
    .when('/notice', { templateUrl: '/notice/notice', controller: 'NoticeCtrl' })
    .when('/noticelist', { templateUrl: '/notice/noticelist', controller: 'NoticeCtrl' })
    .when('/noticechange/:action', { templateUrl: '/notice/noticechange', controller: 'NoticeCtrl' })
    .when('/article', { templateUrl: '/article/article', controller: 'ArticleCtrl' })
    .when('/articlelist', { templateUrl: '/article/articlelist', controller: 'ArticleCtrl' })
    .when('/articlechange/:action', { templateUrl: '/article/articlechange', controller: 'ArticleCtrl' })

     .when('depositauthorizationuselog', { templateUrl: '/Logs/depositauthorizationuselog', controller: 'LoginCtrl' })
    .when('/userlocklog/:action', { templateUrl: '/Logs/userlocklog', controller: 'LogsCtrl' })
    .when('/userloginlog', { templateUrl: '/Logs/userloginlog', controller: 'LoginCtrl' })
    .when('/userassignrolelog/:action', { templateUrl: '/Logs/userassignrolelog', controller: 'LogsCtrl' })
    .when('/vipsettinglog/:action', { templateUrl: '/Logs/vipsettinglog', controller: 'LogsCtrl' })
    .when('/currencylog/:action', { templateUrl: '/Logs/currencylog', controller: 'LogsCtrl' })
    .when('/btcdepositprocesslog/:action', { templateUrl: '/Logs/btcdepositprocesslog', controller: 'LogsCtrl' })
    .when('/btcwithdrawprocesslog/:action', { templateUrl: '/Logs/btcwithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/capitalaccountopreatelog/:action', { templateUrl: '/Logs/capitalaccountopreatelog', controller: 'LogsCtrl' })
    .when('/cnydepositprocesslog/:action', { templateUrl: '/Logs/cnydepositprocesslog', controller: 'LogsCtrl' })
    .when('/cnywithdrawprocesslog/:action', { templateUrl: '/Logs/cnywithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/depositauthorizelog/:action', { templateUrl: '/Logs/depositauthorizelog', controller: 'LogsCtrl' })
    .when('/dogedepositprocesslog/:action', { templateUrl: '/Logs/dogedepositprocesslog', controller: 'LogsCtrl' })
    .when('/dogewithdrawprocesslog/:action', { templateUrl: '/Logs/dogewithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/fundsourcelog/:action', { templateUrl: '/Logs/fundsourcelog', controller: 'LogsCtrl' })
    .when('/ifcdepositprocesslog/:action', { templateUrl: '/Logs/ifcdepositprocesslog', controller: 'LogsCtrl' })
    .when('/ifcwithdrawprocesslog/:action', { templateUrl: '/Logs/ifcwithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/ltcdepositprocesslog/:action', { templateUrl: '/Logs/ltcdepositprocesslog', controller: 'LogsCtrl' })
    .when('/ltcwithdrawprocesslog/:action', { templateUrl: '/Logs/ltcwithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/marketsettinglog/:action', { templateUrl: '/Logs/marketsettinglog', controller: 'LogsCtrl' })
    .when('/nxtdepositprocesslog/:action', { templateUrl: '/Logs/nxtdepositprocesslog', controller: 'LogsCtrl' })
    .when('/nxtwithdrawprocesslog/:action', { templateUrl: '/Logs/nxtwithdrawprocesslog', controller: 'LogsCtrl' })
    .when('/fbcdepositprocesslog/:action', { templateUrl: '/Logs/fbcdepositprocesslog', controller: 'LogsCtrl' })
    .when('/fbcwithdrawprocesslog/:action', { templateUrl: '/Logs/fbcwithdrawprocesslog', controller: 'LogsCtrl' })

    .when('/virtualcurrencydeposit/:action/:state', { templateUrl: '/virtualcurrencydeposit/deposit/depositvirtualdeposit', controller: 'VirtualCurrencyDepositCtrl' })
    .when('/receivepaymenttransaction/:currencyType', { templateUrl: '/virtualcurrencydeposit/deposit/receivepaymenttransaction', controller: 'ReceivePayMentTransactionCtrl' })

    .when('/virtualcurrencywithdraw/:currencyType/:state', { templateUrl: '/VirtualCurrencyWithdraw/withdraw/index', controller: 'StayAuditCtrl' })
    .when('/verifyvirtualcurrencywithdraw/:currencyType/:state', { templateUrl: '/VirtualCurrencyWithdraw/withdraw/verifyindex', controller: 'StayAuditCtrl' })
    .when('/completevirtualcurrencywithdraw/:currencyType/:state', { templateUrl: '/VirtualCurrencyWithdraw/withdraw/completeindex', controller: 'StayAuditCtrl' })


    .when('/depthmarket/:basecurrency/:targetcurrency', { templateUrl: '/depthmarket/index', controller: 'DeptMarketCtrl' })
    .when('/markettransactionrecords/:basecurrency/:targetcurrency', { templateUrl: '/markettransactionrecords/index', controller: 'MarketTransactionRecordsCtrl' })

    .when('/usedepositcode/:action/:state/:range', { templateUrl: '/depositcode/use', controller: 'DepositCodeCtrl' })
    .when('/notuseddepositcode/:action/:state/:range', { templateUrl: '/depositcode/notused', controller: 'DepositCodeCtrl' })
    .when('/destroydepositcode/:action/:state/:range', { templateUrl: '/depositcode/destroy', controller: 'DepositCodeCtrl' })
    .when('/depositcode/:action/:state/:range', { templateUrl: '/depositcode/show', controller: 'DepositCodeCtrl' })

    .when('/virtualcoinstatis/:action', { templateUrl: '/virtualcoinstatis/index', controller: 'VirtualCoinStatisCtrl' })   


    .when('/market', { templateUrl: '/Market/Index', controller: 'MarketCtrl' })
    .otherwise({ redirectTo: '/index' });
}]);
