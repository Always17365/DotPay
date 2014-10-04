
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



    //$scope.groups = [
    //  {
    //      title: '我的充值码',
    //      items: [{ name: '未使用的CNY充值码', url: '/notuseddepositcode/cny/notused/o' },        //512
    //              { name: '已使用的CNY充值码', url: '/usedepositcode/cny/use/o' },                //512
    //              { name: '已销毁的CNY充值码', url: '/destroydepositcode/cny/del/o' },            //512
    //              { name: '未使用的BTC充值码', url: '/notuseddepositcode/btc/notused/o' },        //512
    //              { name: '已使用的BTC充值码', url: '/usedepositcode/btc/use/o' },                //512
    //              { name: '已销毁的BTC充值码', url: '/destroydepositcode/btc/del/o' },            //512
    //              { name: '未使用的IFC充值码', url: '/notuseddepositcode/ifc/notused/o' },        //512
    //              { name: '已使用的IFC充值码', url: '/usedepositcode/ifc/use/o' },                //512
    //              { name: '已销毁的IFC充值码', url: '/destroydepositcode/ifc/del/o' },                   //512
    //              { name: '未使用的NXT充值码', url: '/notuseddepositcode/nxt/notused/o' },        //512
    //              { name: '已使用的NXT充值码', url: '/usedepositcode/nxt/use/o' },                //512
    //              { name: '已销毁的NXT充值码', url: '/destroydepositcode/nxt/del/o' },                   //512
    //              { name: '未使用的LTC充值码', url: '/notuseddepositcode/ltc/notused/o' },        //512
    //              { name: '已使用的LTC充值码', url: '/usedepositcode/ltc/use/o' },                //512
    //              { name: '已销毁的LTC充值码', url: '/destroydepositcode/ltc/del/o' },                   //512
    //              { name: '未使用的DOGE充值码', url: '/notuseddepositcode/doge/notused/o' },      //512
    //              { name: '已使用的DOGE充值码', url: '/usedepositcode/doge/use/o' },               //512
    //              { name: '已销毁的DOGE充值码', url: '/destroydepositcode/doge/del/o' },                   //512
    //              { name: '未使用的STR充值码', url: '/notuseddepositcode/str/notused/o' },      //512
    //              { name: '已使用的STR充值码', url: '/usedepositcode/str/use/o' },               //512
    //              { name: '已销毁的STR充值码', url: '/destroydepositcode/str/del/o' }                 //512
    //      ]
    //  },
    //  {
    //      title: '用户管理',
    //      items: [{ name: '用户列表', url: '/user' },                       //v-all  o-1
    //              { name: '已锁定用户', url: '/userlock/lock' },                //1
    //              { name: '管理员列表', url: '/manager' },                  //32
    //              { name: '客服员列表', url: '/customerservice' }           //32
    //      ]
    //  },
    //  {
    //         title: '用户虚拟币拥有概括',
    //         items: [{ name: 'BTC拥有概括', url: '/useraccountinfo/btc' },
    //                 { name: 'DOGE拥有概括', url: '/useraccountinfo/doge' },
    //                 { name: 'IFC拥有概括', url: '/useraccountinfo/ifc' },
    //                 { name: 'LTC拥有概括', url: '/useraccountinfo/ltc' },
    //                 { name: 'NXT拥有概括', url: '/useraccountinfo/nxt' },
    //                 { name: 'STR拥有概括', url: '/useraccountinfo/str' }
    //         ]
    //   }

    //  ,
    //  {
    //      title: '系统管理',
    //      items: [{ name: '币种列表', url: '/currency' },                  //1
    //              { name: '短信接口', url: '/smsinterface' },              //1
    //              { name: '交易市场设置', url: '/market' },                //1
    //              { name: '资金账号设置', url: '/capitalaccount' },        //1
    //              { name: '银行信息管理', url: '/bankinfo' },
    //              { name: 'VIP等级设置', url: '/vipsetting' },
    //              { name: '虚拟币格式化小数点位数', url: '/VirtualCoinDecimalPlaces' }]              //1
    //  },
    //  {
    //      title: '日志列表',                                                                                      //1
    //      items: [{ name: '管理员分配日志', url: '/userassignrolelog/userassignrolelog' },                        //1
    //              { name: '用户登陆日志', url: '/userloginlog' },                                                 //1
    //              { name: '用户锁定日志', url: '/userlocklog/userlocklog' },                                      //1
    //              { name: '用户等级设置日志', url: '/vipsettinglog/vipsettinglog' },                              //1
    //              { name: '币种操作日志', url: '/currencylog/currencylog' },                                      //1
    //              { name: 'BTC充值记录', url: '/btcdepositprocesslog/btcdepositprocesslog' },                     //1
    //              { name: 'BTC取款记录', url: '/btcwithdrawprocesslog/btcwithdrawprocesslog' },                   //1
    //              { name: 'CNY充值记录', url: '/cnydepositprocesslog/cnydepositprocesslog' },                     //1
    //              { name: 'CNY取款记录', url: '/cnywithdrawprocesslog/cnywithdrawprocesslog' },                   //1
    //              { name: 'DOGE存款日志', url: '/dogedepositprocesslog/dogedepositprocesslog' },                  //1
    //              { name: 'DOGE提现日志', url: '/dogewithdrawprocesslog/dogewithdrawprocesslog' },                //1
    //              { name: '资金来源日志', url: '/fundsourcelog/fundsourcelog' },                                  //1
    //              { name: 'IFC存款日志', url: '/ifcdepositprocesslog/ifcdepositprocesslog' },                     //1
    //              { name: 'IFC提现日志', url: '/ifcwithdrawprocesslog/ifcwithdrawprocesslog' },                   //1
    //              { name: 'LTC存款日志', url: '/ltcdepositprocesslog/ltcdepositprocesslog' },                     //1
    //              { name: 'LTC提现日志', url: '/ltcwithdrawprocesslog/ltcwithdrawprocesslog' },                   //1
    //              { name: 'NTX存款日志', url: '/nxtdepositprocesslog/nxtdepositprocesslog' },                     //1
    //              { name: 'NXT提现日志', url: '/nxtwithdrawprocesslog/nxtwithdrawprocesslog' },                   //1
    //              { name: '充值授权日志', url: '/depositauthorizelog/depositauthorizelog' },                      //1
    //              { name: '资金账户管理日志', url: '/capitalaccountopreatelog/capitalaccountopreatelog' },        //1
    //              { name: '市场设置日志', url: '/marketsettinglog/marketsettinglog' }                             //1
    //      ]                                                                                                       //1
    //  },
    //  {
    //      title: '充值管理',
    //      items: [{ name: '人民币充值记录', url: '/deposit' },                //8
    //              { name: '待处理银行到款', url: '/depositwaitverify/1' },  //9  FundSourceState.WaitVerify.ToString("D")
    //              { name: '账户充值', url: '/bankrecharge/2' }]                // FundSourceState.Verify.ToString("D")

    //  },
    //  {
    //      title: '银行到款管理',
    //      items: [{ name: '银行新到款', url: '/bankwaitverify/1' },     //2   //FundSourceState.WaitVerify.ToString("D")
    //              { name: '待审查银行到款', url: '/bankverify/1' },     //4   //FundSourceState.WaitVerify.ToString("D")
    //              { name: '已审查银行到款', url: '/bankundoverify/2' }, //4   //FundSourceState.Verify.ToString("D")
    //              { name: '已处理银行到款', url: '/bankcomplete/4' }]   //2 4 //FundSourceState.Complete.ToString("D")

    //  },
    //  {
    //      title: '人民币提现管理',
    //      items: [{ name: '待审查人民币提现', url: '/withdraw_wv/1' },  //64            //WithdrawState.WaitVerify.ToString("D")
    //              { name: '待提交人民币提现', url: '/withdraw_e/2' },    //128          //WithdrawState.WaitSubmit.ToString("D")
    //              { name: '处理中人民币提现', url: '/withdraw_p/3' },          //256    //WithdrawState.Processing.ToString("D")
    //              { name: '已处理人民币提现', url: '/withdraw_c/4' },    //128          //WithdrawState.Complete.ToString("D")
    //              { name: '被驳回提现', url: '/withdraw_f/5' }]    //64 128 256         //WithdrawState.Fail.ToString("D")

    //  },
    //    {
    //        title: '文章管理',
    //        items: [{ name: '发布公告', url: '/notice' },               //2048
    //                { name: '公告列表', url: '/noticelist' },           //2048
    //                { name: '发布文章', url: '/article' },              //2048
    //                { name: '文章列表', url: '/articlelist' }]          //2048

    //    },
    //    {
    //        title: '待完成虚拟币充值记录', 
    //        items: [{ name: 'BTC待完成虚拟币充值记录', url: '/virtualcurrencydeposit/btc/1' },          //1 16        //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'IFC待完成虚拟币充值记录', url: '/receivepaymenttransaction/ifc' },          //1 16        //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'NXT待完成虚拟币充值记录', url: '/virtualcurrencydeposit/nxt/1' },          //1 16        //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'LTC待完成虚拟币充值记录', url: '/virtualcurrencydeposit/ltc/1' },          //1 16        //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'DOGE待完成虚拟币充值记录', url: '/virtualcurrencydeposit/doge/1' },          //1 16        //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'STR待完成虚拟币充值记录', url: '/receivepaymenttransaction/STR' }]        //1 16        //WithdrawState.WaitVerify.ToString("D")

    //    },
    //    {
    //        title: '已完成虚拟币充值记录',
    //        items: [{ name: 'BTC已完成虚拟币充值记录', url: '/virtualcurrencydeposit/btc/3' },           //1 16      //WithdrawState.Processing.ToString("D")
    //                { name: 'IFC已完成虚拟币充值记录', url: '/virtualcurrencydeposit/ifc/3' },           //1 16      //WithdrawState.Processing.ToString("D")
    //                { name: 'NXT已完成虚拟币充值记录', url: '/virtualcurrencydeposit/nxt/3' },           //1 16      //WithdrawState.Processing.ToString("D")
    //                { name: 'LTC已完成虚拟币充值记录', url: '/virtualcurrencydeposit/ltc/3' },           //1 16      //WithdrawState.Processing.ToString("D")
    //                { name: 'DOGE已完成虚拟币充值记录', url: '/virtualcurrencydeposit/doge/3' },           //1 16      //WithdrawState.Processing.ToString("D")
    //                { name: 'STR已完成虚拟币充值记录', url: '/virtualcurrencydeposit/STR/3' }]         //1 16      //WithdrawState.Processing.ToString("D")
    //    },                                                                                                  
    //    {
    //        title: '待审核的虚拟币提现记录',
    //        items: [{ name: 'BTC待审核提现记录', url: '/verifyvirtualcurrencywithdraw/btc/1' },                //1 16             //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'IFC待审核提现记录', url: '/verifyvirtualcurrencywithdraw/ifc/1' },                //1 16             //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'NXT待审核提现记录', url: '/verifyvirtualcurrencywithdraw/nxt/1' },                //1 16             //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'LTC待审核提现记录', url: '/verifyvirtualcurrencywithdraw/ltc/1' },                //1 16             //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'DOGE待审核提现记录', url: '/verifyvirtualcurrencywithdraw/doge/1' },                //1 16             //WithdrawState.WaitVerify.ToString("D")
    //                { name: 'STR待审核提现记录', url: '/verifyvirtualcurrencywithdraw/str/1' }]             //1 16             //WithdrawState.WaitVerify.ToString("D")

    //    },
    //    {
    //        title: '处理中的虚拟币提现记录',             
    //        items: [{ name: 'BTC处理中记录', url: '/completevirtualcurrencywithdraw/btc/3' },                  //1 16              //WithdrawState.WaitSubmit.ToString("D")
    //                { name: 'IFC处理中记录', url: '/completevirtualcurrencywithdraw/ifc/3' },                  //1 16              //WithdrawState.WaitSubmit.ToString("D")
    //                { name: 'NXT处理中记录', url: '/completevirtualcurrencywithdraw/nxt/3' },                  //1 16              //WithdrawState.WaitSubmit.ToString("D")
    //                { name: 'LTC处理中记录', url: '/completevirtualcurrencywithdraw/ltc/3' },                  //1 16              //WithdrawState.WaitSubmit.ToString("D")
    //                { name: 'DOGE待确认提现记录', url: '/completevirtualcurrencywithdraw/doge/3' },                  //1 16              //WithdrawState.WaitSubmit.ToString("D")
    //                { name: 'STR待确认提现记录', url: '/completevirtualcurrencywithdraw/STR/3' }]               //1  16             //WithdrawState.WaitSubmit.ToString("D")

    //    },
    //    {
    //        title: '失败的虚拟币提现记录',                                                             //1 16
    //        items: [{ name: 'BTC提现失败记录', url: '/virtualcurrencywithdraw/btc/5' },                  //1 16
    //                { name: 'IFC提现失败记录', url: '/virtualcurrencywithdraw/ifc/5' },                  //1 16
    //                { name: 'NXT提现失败记录', url: '/virtualcurrencywithdraw/nxt/5' },                  //1 16
    //                { name: 'LTC提现失败记录', url: '/virtualcurrencywithdraw/ltc/5' },                  //1 16
    //                { name: 'DOGE提现失败记录', url: '/virtualcurrencywithdraw/doge/5' },                  //1 16
    //                { name: 'STR提现失败记录', url: '/virtualcurrencywithdraw/str/5' }]               //1  16

    //    },
    //    {
    //        title: '虚拟币提现撤销记录',                                                             //1 16
    //        items: [{ name: 'BTC撤销记录', url: '/virtualcurrencywithdraw/btc/6' },                  //1 16
    //                { name: 'IFC撤销记录', url: '/virtualcurrencywithdraw/ifc/6' },                  //1 16
    //                { name: 'NXT撤销记录', url: '/virtualcurrencywithdraw/nxt/6' },                  //1 16
    //                { name: 'LTC撤销记录', url: '/virtualcurrencywithdraw/ltc/6' },                  //1 16
    //                { name: 'DOGE撤销记录', url: '/virtualcurrencywithdraw/doge/6' },                  //1 16
    //                { name: 'STR撤销记录', url: '/virtualcurrencywithdraw/str/6' }]               //1  16

    //    },
    //    {
    //        title: '完成的虚拟币提现记录', 
    //        items: [{ name: 'BTC提现记录', url: '/virtualcurrencywithdraw/btc/4' },                          //1 16              //WithdrawState.Complete.ToString("D")
    //                { name: 'IFC提现记录', url: '/virtualcurrencywithdraw/ifc/4' },                          //1 16              //WithdrawState.Complete.ToString("D")
    //                { name: 'NXT提现记录', url: '/virtualcurrencywithdraw/nxt/4' },                          //1 16              //WithdrawState.Complete.ToString("D")
    //                { name: 'LTC提现记录', url: '/virtualcurrencywithdraw/ltc/4' },                          //1 16              //WithdrawState.Complete.ToString("D")
    //                { name: 'DOGE提现记录', url: '/virtualcurrencywithdraw/doge/4' },                          //1 16              //WithdrawState.Complete.ToString("D")
    //                { name: 'STR提现记录', url: '/virtualcurrencywithdraw/str/4' }]                       //1 16              //WithdrawState.Complete.ToString("D")

    //    },
    //    {
    //        title: '未使用的充值码',                                                              //1 16
    //        items: [{ name: 'CNY未使用充值码', url: '/depositcode/cny/notused/all' },             //1 16
    //                { name: 'BTC未使用充值码', url: '/depositcode/btc/notused/all' },             //1 16
    //                { name: 'IFC未使用充值码', url: '/depositcode/ifc/notused/all' },             //1 16
    //                { name: 'NXT未使用充值码', url: '/depositcode/nxt/notused/all' },             //1 16
    //                { name: 'LTC未使用充值码', url: '/depositcode/ltc/notused/all' },             //1 16
    //                { name: 'DOGE未使用充值码', url: '/depositcode/doge/notused/all' },             //1 16
    //                { name: 'STR未使用充值码', url: '/depositcode/STR/notused/all' }]           //1 16

    //    },
    //    {                                                                                       //1 16
    //        title: '已使用充值码',                                                              //1 16
    //        items: [{ name: 'CNY使用充值码', url: '/depositcode/btc/use/all' },                   //1 16
    //                { name: 'BTC使用充值码', url: '/depositcode/btc/use/all' },                   //1 16
    //                { name: 'IFC使用充值码', url: '/depositcode/ifc/use/all' },                   //1 16
    //                { name: 'NXT使用充值码', url: '/depositcode/nxt/use/all' },                   //1 16
    //                { name: 'LTC使用充值码', url: '/depositcode/ltc/use/all' },                   //1 16
    //                { name: 'DOG未使用充值码', url: '/depositcode/doge/use/all' },                   //1 16
    //                { name: 'STR未使用充值码', url: '/depositcode/STR/use/all' }]                //1 16

    //    },
    //    {
    //        title: '交易市场深度',                                                             //1 16
    //        items: [{ name: 'LTC_DOGE', url: '/depthmarket/ltc/doge/' },                         //1 16
    //                { name: 'LTC_IFC', url: '/depthmarket/ltc/ifc/' },                         //1 16
    //                { name: 'LTC_NXT', url: '/depthmarket/ltc/nxt/' },
    //                { name: 'BTC_LTC', url: '/depthmarket/btc/ltc/' },                         //1 16
    //                { name: 'BTC_IFC', url: '/depthmarket/btc/ifc/' },                         //1 16
    //                { name: 'BTC_NXT', url: '/depthmarket/btc/nxt/' },                         //1 16
    //                { name: 'BTC_DOGE', url: '/depthmarket/btc/doge/' },                       //1 16
    //                { name: 'BTC_STR', url: '/depthmarket/btc/str/' },                         //1 16
    //                { name: 'CNY_STR', url: '/depthmarket/cny/str/' },                         //1 16
    //                { name: 'CNY_BTC', url: '/depthmarket/cny/btc/' },                         //1 16
    //                { name: 'CNY_LTC', url: '/depthmarket/cny/ltc/' },                         //1 16
    //                { name: 'CNY_IFC', url: '/depthmarket/cny/ifc/' },                         //1 16
    //                { name: 'CNY_NXT', url: '/depthmarket/cny/nxt/' },                         //1 16
    //                { name: 'CNY_DOGE', url: '/depthmarket/cny/doge/' }]                       //1 16

    //    },
    //    {
    //        title: '交易市场成交记录',                                                         //1 16
    //        items: [{ name: 'LTC_DOGE', url: '/markettransactionrecords/ltc/doge/' },                         //1 16
    //                { name: 'LTC_IFC', url: '/markettransactionrecords/ltc/ifc/' },                         //1 16
    //                { name: 'LTC_NXT', url: '/markettransactionrecords/ltc/nxt/' },
    //                { name: 'BTC_LTC', url: '/markettransactionrecords/btc/ltc/' },            //1 16
    //                { name: 'BTC_IFC', url: '/markettransactionrecords/btc/ifc/' },            //1 16
    //                { name: 'BTC_NXT', url: '/markettransactionrecords/btc/nxt/' },            //1 16
    //                { name: 'BTC_DOGE', url: '/markettransactionrecords/btc/doge/' },
    //                { name: 'BTC_STR', url: '/markettransactionrecords/btc/str/' },            //1 16
    //                { name: 'CNY_STR', url: '/markettransactionrecords/cny/str/' },     //1 16
    //                { name: 'CNY_BTC', url: '/markettransactionrecords/cny/btc/' },       //1 16
    //                { name: 'CNY_LTC', url: '/markettransactionrecords/cny/ltc/' },            //1 16
    //                { name: 'CNY_IFC', url: '/markettransactionrecords/cny/ifc/' },            //1 16
    //                { name: 'CNY_NXT', url: '/markettransactionrecords/cny/nxt/' },            //1 16
    //                { name: 'CNY_DOGE', url: '/markettransactionrecords/cny/doge/' }]          //1 16


    //    },
    //     {
    //         title: '虚拟币统计概括',
    //         items: [{ name: 'BTC统计概括', url: '/virtualcoinstatis/btc' },
    //                 { name: 'DOGE统计概括', url: '/virtualcoinstatis/doge' },
    //                 { name: 'IFC统计概括', url: '/virtualcoinstatis/ifc' },
    //                 { name: 'LTC统计概括', url: '/virtualcoinstatis/ltc' },
    //                 { name: 'NXT统计概括', url: '/virtualcoinstatis/nxt' },
    //                 { name: 'STR统计概括', url: '/virtualcoinstatis/STR' }
    //         ]
    //     }
    //];

    $scope.navTo = function (item) {
        $scope.frameSrc = $sce.trustAsResourceUrl(item.url);
    }
}