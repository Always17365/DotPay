angular.module('AlertBox', [])
    .service('$alert', ['$timeout', function ($timeout) {

        this.delay = 5000;

        this.alerts = [];
        this.infos = [];
        this.warns = [];
        this.notices = [];
        this.dangers = [];

        this.setCloseDelay = function (delay) {
            this.delay = delay;
        }

        this.removeMessage = function (msgType, message, delay) {
            var tracked = this[msgType + 's'];
            if (angular.isDefined(delay)) {
                $timeout(
                    function () {
                        var index = tracked.indexOf(message);
                        if (index > -1) {
                            tracked.splice(index, 1);
                        }
                    },
                    this.delay
                );
            } else {
                var index = tracked.indexOf(message);
                if (index > -1) {
                    tracked.splice(index, 1);
                }
            }
        };

        this.Message = function (msgType, message) {
            var key = msgType + 's';
            this[key].push(message);
            this.removeMessage(msgType, message, this.delay);
        };

        this.Alert = function (message) { this.Message('alert', message); }
        this.Info = function (message) { this.Message('info', message); }
        this.Notice = function (message) { this.Message('notice', message); }
        this.Warn = function (message) { this.Message('warn', message); }
        this.Danger = function (message) { this.Message('danger', message); }

    }])
    .directive('alertBox', ['$alert', function ($alert) {
        return {
            restrict: 'AE',

            compile: function (element, attrs) {
                attrs.boxClass = attrs.boxClass || "alert";
                attrs.defaultClass = attrs.defaultClass || "alert-default";
                attrs.infoClass = attrs.infoClass || "alert-info";
                attrs.noticeClass = attrs.noticeClass || "alert-success";
                attrs.warnClass = attrs.warnClass || "alert-warning";
                attrs.dangerClass = attrs.dangerClass || "alert-danger";
            },

            scope: {
                boxClass: "@",
                defaultClass: "@",
                infoClass: "@",
                noticeClass: "@",
                warnClass: "@",
                dangerClass: "@"
            },

            controller: function ($scope) {
                $scope.alerts = $alert.alerts;
                $scope.warns = $alert.warns;
                $scope.dangers = $alert.dangers;
                $scope.infos = $alert.infos;
                $scope.notices = $alert.notices;
            },
            template: '<div ng-repeat="alert in alerts track by $index" ng-class="[boxClass, defaultClass]">{{alert}}</div> \
                       <div ng-repeat="warn in warns track by $index" ng-class="[boxClass, warnClass]">{{warn}}</div> \
                       <div ng-repeat="info in infos track by $index" ng-class="[boxClass, infoClass]">{{info}}</div>\
                       <div ng-repeat="notice in notices track by $index" ng-class="[boxClass, noticeClass]">{{notice}}</div>\
                       <div ng-repeat="danger in dangers track by $index" ng-class="[boxClass, dangerClass]">{{danger}}</div>'
        };

    }]);
