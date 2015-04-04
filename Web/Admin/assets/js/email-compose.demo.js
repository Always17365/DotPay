/*
Template Name: Color Dotpay.Admin - Responsive Dotpay.Admin Dashboard Template build with Twitter Bootstrap 3.3.2
Version: 1.6.0
Author: Sean Ngu
Website: http://www.seantheme.com/color-admin-v1.6/admin/
*/

var handleEmailToInput = function() {
    $('#email-to').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
};

var handleEmailContent = function() {
    $('#wysihtml5').wysihtml5();
};

var EmailCompose = function () {
	"use strict";
    return {
        //main function
        init: function () {
            $.getScript('assets/plugins/jquery-tag-it/js/tag-it.min.js').done(function() {
                handleEmailToInput();
            });
            
            $.getScript('assets/plugins/bootstrap-wysihtml5/lib/js/wysihtml5-0.3.0.js').done(function() {
                $.getScript('assets/plugins/bootstrap-wysihtml5/src/bootstrap-wysihtml5.js').done(function() {
                    handleEmailContent();
                });
            });
        }
    };
}();