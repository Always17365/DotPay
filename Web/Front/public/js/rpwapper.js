/// <reference path="ripple/ripple-debug.js" /> 
var remote;
$(function () {
    var remoteOptions = {
        trusted: true,
        local_signing: true,
        local_fee: true,
        fee_cushion: 1.5,
        servers: [{
            host: 's1.ripple.com',
            port: 443,
            secure: true
        }, {
            host: 's_east.ripple.com',
            port: 443,
            secure: true
        }, {
            host: 's_west.ripple.com',
            port: 443,
            secure: true
        }
        ]
    }

    remote = new ripple.Remote(remoteOptions);
    reconnect(remote);
    remote.isConnected
    function reconnect(r) {
        r.connect(function () {
            GetBalance();
            console.log("测试成功")
        });
        r.once("disconnect", function () {
            console.log("测试断开成功")
            r = null;
            r = new ripple.Remote(remoteOptions);
            reconnect(r);
        });
    }
})

function GetBalances() {
    if (remote.isConnected()) {
        var req = remote.requestAccountLines("rUSweLuRhP8xWd11FsnEjbDo8hJPcvnuLm", function (error, lines) {
            console.log(error);
            console.log(lines);
        });
        req.request();
    }
    else {
        console.log("not connected to ripple");
    }
}

