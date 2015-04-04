/*
Template Name: Color Dotpay.Admin - Responsive Dotpay.Admin Dashboard Template build with Twitter Bootstrap 3.3.2
Version: 1.6.0
Author: Sean Ngu
Website: http://www.seantheme.com/color-admin-v1.6/admin/
*/

var handleDatepicker = function() {
    $('#datepicker-default').datepicker({
        todayHighlight: true
    });
    $('#datepicker-inline').datepicker({
        todayHighlight: true
    });
    $('.input-daterange').datepicker({
        todayHighlight: true
    });
    $('#datepicker-disabled-past').datepicker({
        todayHighlight: true
    });
    $('#datepicker-autoClose').datepicker({
        todayHighlight: true,
        autoclose: true
    });
};

var handleIonRangeSlider = function() {
    $('#default_rangeSlider').ionRangeSlider({
        min: 0,
        max: 5000,
        type: 'double',
        prefix: "$",
        maxPostfix: "+",
        prettify: false,
        hasGrid: true
    });
    $('#customRange_rangeSlider').ionRangeSlider({
        min: 1000,
        max: 100000,
        from: 30000,
        to: 90000,
        type: 'double',
        step: 500,
        postfix: " €",
        hasGrid: true
    });
    $('#customValue_rangeSlider').ionRangeSlider({
        values: [
            'January', 'February', 'March',
            'April', 'May', 'June',
            'July', 'August', 'September',
            'October', 'November', 'December'
        ],
        type: 'single',
        hasGrid: true
    });
};

var handleFormMaskedInput = function() {
    "use strict";
    $("#masked-input-date").mask("99/99/9999");
    $("#masked-input-phone").mask("(999) 999-9999");
    $("#masked-input-tid").mask("99-9999999");
    $("#masked-input-ssn").mask("999-99-9999");
    $("#masked-input-pno").mask("aaa-9999-a");
    $("#masked-input-pkey").mask("a*-999-a999");
};

var handleFormColorPicker = function () {
    "use strict";
    $('#colorpicker').colorpicker({format: 'hex'});
    $('#colorpicker-prepend').colorpicker({format: 'hex'});
    $('#colorpicker-rgba').colorpicker();
};

var handleFormTimePicker = function () {
    "use strict";
    $('#timepicker').timepicker();
};

var handleFormPasswordIndicator = function() {
    "use strict";
    $('#password-indicator-default').passwordStrength();
    $('#password-indicator-visible').passwordStrength({targetDiv: '#passwordStrengthDiv2'});
};

var handleJqueryAutocomplete = function() {
    var availableTags = [
        'ActionScript',
        'AppleScript',
        'Asp',
        'BASIC',
        'C',
        'C++',
        'Clojure',
        'COBOL',
        'ColdFusion',
        'Erlang',
        'Fortran',
        'Groovy',
        'Haskell',
        'Java',
        'JavaScript',
        'Lisp',
        'Perl',
        'PHP',
        'Python',
        'Ruby',
        'Scala',
        'Scheme'
    ];
    $('#jquery-autocomplete').autocomplete({
        source: availableTags
    });
};

var handleBootstrapCombobox = function() {
    $('.combobox').combobox();
};

var handleTagsInput = function() {
    $('.bootstrap-tagsinput input').focus(function() {
        $(this).closest('.bootstrap-tagsinput').addClass('bootstrap-tagsinput-focus');
    });
    $('.bootstrap-tagsinput input').focusout(function() {
        $(this).closest('.bootstrap-tagsinput').removeClass('bootstrap-tagsinput-focus');
    });
};

var handleSelectpicker = function() {
    $('.selectpicker').selectpicker('render');
};

var handleJqueryTagIt = function() {
    $('#jquery-tagIt-default').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-inverse').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-white').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-primary').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-info').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-success').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-warning').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
    $('#jquery-tagIt-danger').tagit({
        availableTags: ["c++", "java", "php", "javascript", "ruby", "python", "c"]
    });
};

var FormPlugins = function () {
	"use strict";
    return {
        //main function
        init: function () {
        
            $.getScript('assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js').done(function() {
                handleDatepicker();
            });
            
            $.getScript('assets/plugins/ionRangeSlider/js/ion-rangeSlider/ion.rangeSlider.min.js').done(function() {
			    handleIonRangeSlider();
            });
            
            $.getScript('assets/plugins/bootstrap-colorpicker/js/bootstrap-colorpicker.min.js').done(function() {
			    handleFormColorPicker();
			});
			
            $.getScript('assets/plugins/masked-input/masked-input.min.js').done(function() {
			    handleFormMaskedInput();
			});
			
            $.getScript('assets/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js').done(function() {
			    handleFormTimePicker();
            });
            
            $.getScript('assets/plugins/password-indicator/js/password-indicator.js').done(function() {
			    handleFormPasswordIndicator();
            });
            
            $.getScript('assets/plugins/bootstrap-combobox/js/bootstrap-combobox.js').done(function() {
			    handleBootstrapCombobox();
            });
            
            $.getScript('assets/plugins/bootstrap-select/bootstrap-select.min.js').done(function() {
			    handleSelectpicker();
            });
            
            $.getScript('assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput.min.js').done(function() {
			    handleTagsInput();
            });
            
            $.getScript('assets/plugins/bootstrap-tagsinput/bootstrap-tagsinput-typeahead.js').done(function() {
			    handleJqueryAutocomplete();
            });
            
            $.getScript('assets/plugins/jquery-tag-it/js/tag-it.min.js').done(function() {
			    handleJqueryTagIt();
            });
        }
    };
}();