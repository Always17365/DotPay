var Notification = function () {
    "use strict";
    return {
        //main function
        init: function () {
            $.getScript('/assets/plugins/gritter/js/jquery.gritter.js');
        },
        notice: function(title, text) {
            $.gritter.add({
                title: title,
                text: text
            });
        }
    };
}();