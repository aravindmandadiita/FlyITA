var main;
$(function () {
    main = new Main();
    main.init();
});

function Main() {
    var self = this;

    self.disableForwardNavigation = $('#optDisableForwardNav').val() == 'true';

    // 20150714 :: Conditionally set scrollto from webreg (fixes page scrolling issue)
    self.scrollToWindowTop = $('#optScrollToTopOfWindow').val() == 'true';
    self.timeout = new TimeoutTimer();
    self.scrolling = false;

    self.init = function() {
        self.timeout.init();
        if (!self.disableForwardNavigation)
            self.forceNavigation();

        $(document).on('click', '#extend-session', self.timeout.RefreshSession);
        $('a.disabled').on('click', function (e) { e.preventDefault(); });
        $('input[type=tel], input.telephone').on('blur change', self.formatPhone);
        $('input[type=tel], input.telephone').each(function (i, obj) { self.formatPhone.call($(obj)); });
        self.overrideInvalidInputNavigation();

        // 20150714 :: Added ability to conditionally set scrollto from webreg (fixes page scrolling issue)
        if (self.scrollToWindowTop) {
            window.scrollTo(0, 0);
        }
    }

    self.forceNavigation = function() {
        try {
            history.forward();
        } catch (e) {
            // ignore
        }
    }

    //meant to adjust placement for HTML form validation so that it doesn't hide behind sticky nav, but only works on chrome
    self.overrideInvalidInputNavigation = function () {
        $('input, select, textarea').on('invalid', function (e) {
            var element = $('input:invalid, select:invalid, textarea:invalid')[0];
            var targetOffset = $(element).offset().top - 130;
            $('body').height();
            if (!self.isInViewPort(element) && $('body').scrollTop() != targetOffset && !self.scrolling) {
                self.scrolling = true;
                $('body').animate({
                    scrollTop: targetOffset
                }, 500, function () {
                    self.scrolling = false;
                    $('input[type="submit"]').trigger('click');
                });
            }
        })
    }

    self.formatPhone = function () {
        $(this).val($.trim($(this).val()));
        $(this).closest('.form-group').removeClass('has-error');
        var number = $(this).val();
        var length = 12;
        var regex = new RegExp(/\d{3}[-]\d{3}[-]\d{4}/g);
        if (number.length == 0 || (number.length == length && regex.test(number)))
            return;
        number = number.replace(/\D/g, '');
        var replacement = number.substr(0, 3);
        if (number.length > 3)
            replacement += '-' + number.substr(3, 3);
        if (number.length > 6)
            replacement += '-' + number.substr(6, 4);
        $(this).val(replacement);

        if (replacement.length != length || !regex.test(replacement))
            $(this).closest('.form-group').addClass('has-error');
        return;
    }

    self.isInViewPort = function (el) {

        //special bonus for those using jQuery
        if (typeof jQuery === "function" && el instanceof jQuery) {
            el = el[0];
        }

        var rect = el.getBoundingClientRect();

        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
            rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
        );
    }
}

function TimeoutTimer() {
    var self = this;

    self.TimeoutSeconds = Number($('#optTimeoutSeconds').val());
    self.WarningSeconds = Number($('#optTimeoutWarningSeconds').val());
    self.PreventModal = $('#PreventTimeoutModal').length > 0;
    self.Timer = null;
    self.Window = window;
    self.Timeout = null;
    self.RemainingSeconds = 0;

    self.init = function() {
        self.RemainingSeconds = self.TimeoutSeconds;
        $("#timeout_timer").html(self.RemainingSeconds);
        self.Timeout = setTimeout(self.ShowWarning, self.WarningSeconds * 1000);
    }

    self.ShowWarning = function () {
        if (!self.PreventModal) {
            $('#timeout_warning').modal({ keyboard: false, backdrop: 'static', show: true });
            self.Timer = setInterval(self.UpdateTimer, 1000);
        }
    }

    self.UpdateTimer = function() {
        self.RemainingSeconds--;
        $("#timeout_timer").html(self.RemainingSeconds);
        if (self.RemainingSeconds < 1) {
            self.DoRedirect("logout.aspx?msg=timeout");
        }
    }

    self.RefreshSession = function() {
        $.ajax("/home.aspx");
        self.ClearTimer();
        $('#timeout_warning').modal('hide');
    }

    self.ClearTimer = function() {
        self.RemainingSeconds = self.TimeoutSeconds;
        $("#timeout_timer").html(self.RemainingSeconds);
        clearInterval(self.Timer);
        self.Timer = null;
    }

    self.DoRedirect = function(path) {
        self.Window.location.href = path;
    }
}

function showDatePicker() {
    $(this).parent().data("DateTimePicker").show();
}
