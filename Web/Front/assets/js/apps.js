/* 01. Handle Home Content Height
------------------------------------------------ */
var handleHomeContentHeight = function () {
    $('#home').height($(window).height());
};


/* 02. Handle Header Navigation State
------------------------------------------------ */
var handleHeaderNavigationState = function () {
    $(window).on('scroll load', function () {
        if ($('#header').attr('data-state-change') != 'disabled') {
            var totalScroll = $(window).scrollTop();
            var headerHeight = $('#header').height();
            if (totalScroll >= headerHeight) {
                $('#header').addClass('navbar-small  navbar-fixed-top');
            } else {
                $('#header').removeClass('navbar-small  navbar-fixed-top');
            }
        }
    });
};


/* 03. Handle Commas to Number
------------------------------------------------ */
var handleAddCommasToNumber = function (value) {
    return value.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
};


/* 04. Handle Page Container Show
------------------------------------------------ */
var handlePageContainerShow = function () {
    $('#page-container').addClass('in');
};

/* 05. Handle Page Scroll Content Animation
------------------------------------------------ */
var handlePageScrollContentAnimation = function () {
    $('[data-scrollview="true"]').each(function () {
        var myElement = $(this);

        var elementWatcher = scrollMonitor.create(myElement, 60);

        elementWatcher.enterViewport(function () {
            $(myElement).find('[data-animation=true]').each(function () {
                var targetAnimation = $(this).attr('data-animation-type');
                var targetElement = $(this);
                if (!$(targetElement).hasClass('contentAnimated')) {
                    if (targetAnimation == 'number') {
                        var finalNumber = parseInt($(targetElement).attr('data-final-number'));
                        $({ animateNumber: 0 }).animate({ animateNumber: finalNumber }, {
                            duration: 1000,
                            easing: 'swing',
                            step: function () {
                                var displayNumber = handleAddCommasToNumber(Math.ceil(this.animateNumber));
                                $(targetElement).text(displayNumber).addClass('contentAnimated');
                            }
                        });
                    } else {
                        $(this).addClass(targetAnimation + ' contentAnimated');
                    }
                }
            });
        });
    });
};


/* 06. Handle Header Scroll To Action
------------------------------------------------ */
var handleHeaderScrollToAction = function () {
    $('[data-click=scroll-to-target]').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var target = $(this).attr('href');
        var headerHeight = 50;
        $('html, body').animate({
            scrollTop: $(target).offset().top - headerHeight
        }, 500);

        if ($(this).attr('data-toggle') == 'dropdown') {
            var targetLi = $(this).closest('li.dropdown');
            if ($(targetLi).hasClass('open')) {
                $(targetLi).removeClass('open');
            } else {
                $(targetLi).addClass('open');
            }
        }
    });
    $(document).click(function (e) {
        if (!e.isPropagationStopped()) {
            $('.dropdown.open').removeClass('open');
        }
    });
};


/* 07. Handle Tooltip Activation
------------------------------------------------ */
var handleTooltipActivation = function () {
    if ($('[data-toggle=tooltip]').length !== 0) {
        $('[data-toggle=tooltip]').tooltip('hide');
    }
};


/* 08. Handle Switch Language 
------------------------------------------------ */
var handleSwitchLanguage = function () {
    $("#lang_list a[theme-lang]").click(function () {
        var lang = $(this).attr("theme-lang");
        $.cookie("lanaguage", lang);

        var fontStyle = 'assets/css/lang/other.css'
        if (lang==="zh-cn"||lang==="zh-tw")
            var fontStyle = 'assets/css/lang/' + lang + '.css'; 

        $('#fontStyle').attr('href', fontStyle);
        window.location.reload();
    });

    fontstyle
}
/* Application Controller
------------------------------------------------ */
var App = function () {
    "use strict";

    return {
        //main function
        init: function () {
            handleHomeContentHeight();
            handleHeaderNavigationState();
            handlePageContainerShow();
            // handlePaceLoadingPlugins();
            handlePageScrollContentAnimation();
            handleHeaderScrollToAction();
            handleTooltipActivation();
            handleSwitchLanguage();
        }
    };
}();