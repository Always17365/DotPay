
function NoticeCtrl($scope, $window, $http, $sce, $modal, $alert, $routeParams) {
    $scope.selectNotice = undefined;
    $scope.contentdb = "";
    $scope.currentPage = 1;
    $scope.noticeID = $routeParams.action;

    $http.post('../Notice/GetNoticeCategory').success(function (data, status, headers) {
        $scope.lang = data;
    });	

    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectfundSource = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page };

        $http.post('../notice/GetNoticeCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../notice/GetNoticeBySearch', searchCondition).success(function (data, status, headers) {
            $scope.notices = data;
        });
    }

    $scope.checkedChange = function (Notice) {
        for (index in $scope.Notices) {
            var _Notice = $scope.Notices[index];
            if (_Notice.ID != Notice.ID)
                _Notice.Checked = false;
        }

        if (Notice.Checked) $scope.selectNotice = Notice;
        else $scope.selectNotice = undefined;
    }



    $scope.saveNotice = function (c_title, isTop, LangType) {
        var url = '../notice/CreateAnnouncement';
        $http.post(url, { title: c_title, isTop: isTop ? isTop : false, lang: LangType.Value, content: $scope.content }).success(function (data, status, headers) {
            $window.location.href = "/#/noticelist";
        });
    };
    $scope.changeNotice = function (noticeContents) {
        var url = '../notice/EditAnnouncement';
        $http.post(url, { announcementID: noticeContents.ID, title: noticeContents.Title, content: $scope.content, lang: noticeContents.LangType.Value, isTop: noticeContents.IsTop ? true : false }).success(function (data, status, headers) {
            $window.location.href = "/#/noticelist";
        });
    };
    if ($scope.noticeID != undefined) {
        $http.post('../notice/GetNotice', { id: $scope.noticeID }).success(function (data, status, headers) {
            $scope.noticeContents = data;
            $scope.contentdb = data.Content;
            $scope.noticeContents.LangType = $scope.lang[data.Lang - 1];
        });
    } else {
        $scope.search();
    }
}