
$(function () {
    "use strict";

    $(window).load(function () {
        // ----------------------------------------------------------------------------------------------------------------------->
        // SITE LOADER                     ||-----------
        // ----------------------------------------------------------------------------------------------------------------------->
        $('#loader').fadeOut();
        $('#preloader').delay(350).fadeOut('slow');
        $('body').delay(350).css({ 'overflow': 'visible' });

    })

    // ---------------------------------------------------------------------------------------------------------------------------->
    // GENERAL SCRIPTS FOR ALL PAGES    ||-----------
    // ---------------------------------------------------------------------------------------------------------------------------->

    $(document).ready(function () {
        openSite();

    });

    function openSite() {
        header();
        scroll();
        winResize();
        //scrollCallbackEle();
        shortcodeElements();

    };

    // ---------------------------------------------------------------------------------------------------------------------------->
    // RESIZE FUNCTIONS   ||-----------
    // ---------------------------------------------------------------------------------------------------------------------------->
    function winResize() {
        $(window).resize(function () {

        })
    };
});


// ---------------------------------------------------------------------------------------------------------------------------->
// SCROLL FUNCTIONS   ||-----------
// ---------------------------------------------------------------------------------------------------------------------------->

function scroll() {

    // //Click Event to Scroll to Top
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.scroll-top').fadeIn();
        } else {
            $('.scroll-top').fadeOut();
        }
    });
    $('.scroll-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });

    // Scroll Down Elements
    $('.scroll-down[href^="#"], .scroll-to-target[href^="#"]').on('click', function (e) {
        e.preventDefault();

        var target = this.hash;
        var $target = $(target);

        $('html, body').stop().animate({
            'scrollTop': $target.offset().top
        }, 900, 'swing', function () {
            window.location.hash = target;
        });
    });

};


// ---------------------------------------------------------------------------------------------------------------------------->
// HEADER FUNCTIONS   ||-----------
// ---------------------------------------------------------------------------------------------------------------------------->

function header() {

    // Sticky Header Elements
    $(window).scroll(function () {
        if ($(this).scrollTop() > 1) {
            $('.header').addClass("sticky");
            $('.inner-inro').css('z-index', '-1');
        }
        else {
            $('.header').removeClass("sticky");
            $('.inner-inro').css('z-index', 'auto');
        }
    });
    heightElement();
    function heightElement() {

        // windowWidth = $(window).width();
        var windowHeight = $(window).height();
        if ($(window).width() > 767) {
            // $('.inner-inro').css('height', windowHeight - 200);
        }
        else {
            // $('.inner-inro').css({ 'height': '400px', 'padding-top': '64px' });
        }
    }

    $(window).resize(function () {
        heightElement();
    });
};


// ---------------------------------------------------------------------------------------------------------------------------->
// SCROLL CALLBACK FUNCTION  ||-----------
// ---------------------------------------------------------------------------------------------------------------------------->
//function scrollCallbackEle() {
//    //scroll Callback Element
//    $('.load-ele-fade').viewportChecker({
//        classToAdd: 'visible animated fadeIn',
//        offset: 100,
//        callbackFunction: function (elem, action) {
//        }
//    });
//    //scroll Animate Element
//    wow = new WOW({
//        boxClass: 'wow',
//        animateClass: 'animated',
//        offset: 0,
//        mobile: false,
//        live: true
//    })
//    wow.init();
//};

// ---------------------------------------------------------------------------------------------------------------------------->
// SHORTCODE ELEMENTS  ||-----------
// ---------------------------------------------------------------------------------------------------------------------------->


function shortcodeElements() {


    //Parallax Function element
    $('.parallax').each(function () {
        var $el = $(this);
        $(window).scroll(function () {
            parallax($el);
        });
        parallax($el);
    });


    function parallax($el) {
        var diff_s = $(window).scrollTop();
        var parallax_height = $('.parallax').height();
        var yPos_p = (diff_s * 0.5);
        var yPos_m = -(diff_s * 0.5);
        var diff_h = diff_s / parallax_height;

        if ($('.parallax').hasClass('parallax-section1')) {
            $el.css('top', yPos_p);
        }
        if ($('.parallax').hasClass('parallax-section2')) {
            $el.css('top', yPos_m);
        }
        if ($('.parallax').hasClass('parallax-static')) {
            $el.css('top', (diff_s * 1));
        }
        if ($('.parallax').hasClass('parallax-opacity')) {
            $el.css('opacity', (1 - diff_h * 1));
        }

        if ($('.parallax').hasClass('parallax-background1')) {
            $el.css("background-position", 'left' + " " + yPos_p + "px");
        }
        if ($('.parallax').hasClass('parallax-background2')) {
            $el.css("background-position", 'left' + " " + -yPos_p + "px");

        }
    };

    //Parallax plugin Js Function element
    //$.stellar({
    //    horizontalScrolling: false,
    //    verticalOffset: 500
    //});
};


