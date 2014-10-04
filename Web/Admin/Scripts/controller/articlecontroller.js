
function ArticleCtrl($scope,$window, $http, $sce, $modal, $alert, $routeParams) {
    $scope.selectArticle = undefined;
    $scope.contentdb = "";
    $scope.currentPage = 1;
    $scope.articleID = $routeParams.action;
    $http.post('../article/GetArticleCategory').success(function (data, status, headers) {
        $scope.category = data.articleCategory;
        $scope.lang = data.lang;
    });
    
    $scope.search = function () {
        $scope.pageChange(1);
        $scope.selectfundSource = undefined;
    }

    $scope.pageChange = function (page) {
        $scope.currentPage = page;
        var searchCondition = { page: page };

        $http.post('../article/GetArticleCountBySearch', searchCondition).success(function (data, status, headers) {
            $scope.totalCount = data;
        });
        $http.post('../article/GetArticleBySearch', searchCondition).success(function (data, status, headers) {
            $scope.articles = data;
        });
    }

    $scope.checkedChange = function (Article) {
        for (index in $scope.Articles) {
            var _Article = $scope.Articles[index];
            if (_Article.ID != Article.ID)
                _Article.Checked = false;
        }

        if (Article.Checked) $scope.selectArticle = Article;
        else $scope.selectArticle = undefined;
    }



    $scope.saveArticle = function (c_title, isTop, categoryType, LangType) {
        var url = '../article/CreateArticle';
        $http.post(url, { category: categoryType.Value, title: c_title, isTop: isTop ? true : false, content: $scope.content, lang: LangType.Value }).success(function (data, status, headers) {
            $window.location.href = "/#/articlelist";
        });
    };
    $scope.changeArticle = function (articleContents, categoryType, LangType) {
        var url = '../article/EditArticle';
        $http.post(url, { category: categoryType.Value, articleID: articleContents.ID, title: articleContents.Title, content: $scope.content, isTop: articleContents.IsTop, lang: LangType.Value }).success(function (data, status, headers) {
            $window.location.href = "/#/articlelist";
        });
    };
    if ($scope.articleID != undefined) {
        $http.post('../article/GetArticle', { id: $scope.articleID }).success(function (data, status, headers) {
            $scope.articleContents = data;
            $scope.contentdb = data.Content;
            $scope.categoryType = $scope.category[data.Category-1];
            $scope.LangType = $scope.lang[data.Lang-1];
        });
    } else {
        $scope.search();
    }
}