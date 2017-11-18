var app = function() {

    var init = function() {

        menu();
        togglePanel();
        closePanel();
    };

 

    var togglePanel = function() {
        $('.actions > .fa-chevron-down').click(function() {
            $(this).parent().parent().next().slideToggle('fast');
            $(this).toggleClass('fa-chevron-down fa-chevron-up');
        });

    };

    var toggleMenuLeft = function() {
        $('#toggle-left').bind('click', function(e) {
            $('body').removeClass('off-canvas-open');    
            var bodyEl = $('#container');
            ($(window).width() > 768) ? $(bodyEl).toggleClass('sidebar-mini'): $(bodyEl).toggleClass('sidebar-opened');
        });
    };

    var toggleMenuRight = function() {
        $('#toggle-right').click(function(){
            $('.off-canvas').toggleClass('off-canvas-open');
        });
    };

   

    var closePanel = function() {
        $('.actions > .fa-times').click(function() {
            $(this).parent().parent().parent().fadeOut();
        });

    }

    var menu = function() {
        var subMenu = $('.sidebar .nav');
        $(subMenu).navgoco({
            caretHtml: false,
            accordion: true,
            slide: {
                duration: 500,
                easing: 'swing'
            }
        });

    };
  


    


    //return functions
    return {
        init: init

    };
}();

$(document).ready(function() {
    app.init();

});
