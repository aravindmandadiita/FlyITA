/// <reference path="jasmine/jasmine.js" />
/// <reference path="frameworks/jquery-1.11.1.min.js" />
/// <reference path="frameworks/moment.min.js" />
/// <reference path="frameworks/bootstrap.js" />
/// <reference path="frameworks/bootstrap-datetimepicker.min.js" />
/// <reference path="jasmine/fixtures.js" />
/// <reference path="main.js" />

describe("main feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("main");
    });

    self.afterEach(function () {
        fixtures.cleanUp();
    });

    it("Should init", function () {
        var main = new Main();
        main.timeout = new TimeoutTimer();
        spyOn(main.timeout, "init");
        spyOn(main.timeout, "RefreshSession");
        spyOn(main, "forceNavigation");
        spyOn(main, "formatPhone");
        expect(main.disableForwardNavigation).toBeTruthy();
        main.disableForwardNavigation = false;
        main.init();
        var event = $.Event("click");
        $('#disabled-link').trigger(event);
        $('#extend-session').trigger('click');
        $('#txtBusinessPhone').trigger('blur');
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect(main.forceNavigation).toHaveBeenCalled();
        expect(main.timeout.init).toHaveBeenCalled();
        expect(main.timeout.RefreshSession).toHaveBeenCalled();
        expect(main.formatPhone.calls.count()).toBe(2);
    });

    it("Should force navigation", function() {
        var main = new Main();
        spyOn(history, "forward");
        main.forceNavigation();
        expect(history.forward).toHaveBeenCalled();
    });

    it("Should format phone number", function() {
        var main = new Main();
        $('#txtBusinessPhone').val('1234567890');
        main.formatPhone.call($('#txtBusinessPhone'), $.Event('blur'));
        expect($('#txtBusinessPhone').val()).toBe('123-456-7890');
    });

    it("Should format phone number when input length is nothing", function () {
        var main = new Main();
        $('#txtBusinessPhone').val(' ');
        main.formatPhone.call($('#txtBusinessPhone'), $.Event('blur'));
        expect($('#txtBusinessPhone').val()).toBe('');
        expect($('#txtBusinessPhone').closest('.form-group').hasClass('has-error')).toBeFalsy();
    });

    it("Should do nothing to the phone number when input value is correct", function () {
        var main = new Main();
        $('#txtBusinessPhone').val('123-456-7890');
        main.formatPhone.call($('#txtBusinessPhone'), $.Event('blur'));
        expect($('#txtBusinessPhone').val()).toBe('123-456-7890');
        expect($('#txtBusinessPhone').closest('.form-group').hasClass('has-error')).toBeFalsy();
    });

    it("Should apply error when phone number is not empty and is invalid", function () {
        var main = new Main();
        $('#txtBusinessPhone').val('123-456-78');
        main.formatPhone.call($('#txtBusinessPhone'), $.Event('blur'));
        expect($('#txtBusinessPhone').val()).toBe('123-456-78');
        expect($('#txtBusinessPhone').closest('.form-group').hasClass('has-error')).toBeTruthy();
    });

    it("Should show datepicker", function () {
        fixtures.loadFixture("hotelext");
        $('#checkin-date').datetimepicker({ pickTime: false });
        var picker = $('#txtCheckInDate').parent().data("DateTimePicker");
        spyOn(picker, "show");
        showDatePicker.call($('#txtCheckInDate'));
        expect(picker.show).toHaveBeenCalled();
    });

});

describe("Timeout Timer feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("main");
        jasmine.clock().install();
    });

    self.afterEach(function () {
        fixtures.cleanUp();
        jasmine.clock().uninstall();
    });

    it("Should init", function () {
        var timer = new TimeoutTimer();
        expect(timer.Timeout).toBeNull();
        expect(timer.RemainingSeconds).toBe(0);
        timer.init();
        expect(timer.Timeout).not.toBeNull();
        expect(timer.RemainingSeconds).toBe(timer.TimeoutSeconds);
        expect($("#timeout_timer").html()).toBe(timer.TimeoutSeconds.toString());
    });

    it("Should show warning when modal not prevented", function() {
        var timer = new TimeoutTimer();
        expect(timer.Timer).toBeNull();
        timer.ShowWarning();
        expect(timer.Timer).not.toBeNull();
    });

    it("Should not show warning when modal prevented", function () {
        var timer = new TimeoutTimer();
        timer.PreventModal = true;
        expect(timer.Timer).toBeNull();
        timer.ShowWarning();
        expect(timer.Timer).toBeNull();
    });

    it("Should update timer", function() {
        var timer = new TimeoutTimer();
        spyOn(timer, "DoRedirect");
        timer.RemainingSeconds = 2;
        timer.UpdateTimer();
        expect($("#timeout_timer").html()).toBe('1');
        expect(timer.RemainingSeconds).toBe(1);
        timer.UpdateTimer();
        expect(timer.DoRedirect).toHaveBeenCalled();
    });

    it("Should Refresh Session", function() {
        var timer = new TimeoutTimer();
        spyOn(timer, "ClearTimer");
        spyOn($, "ajax");
        timer.RefreshSession();
        expect(timer.ClearTimer).toHaveBeenCalled();
        expect($.ajax).toHaveBeenCalled();
    });

    it("Should Clear Timer", function() {
        var timer = new TimeoutTimer();
        timer.RemainingSeconds = 5;
        timer.Timer = setInterval(function () { return ""; }, 1000);
        timer.ClearTimer();
        expect(timer.Timer).toBeNull();
        expect(timer.RemainingSeconds).toBe(timer.TimeoutSeconds);
        expect($("#timeout_timer").html()).toBe(timer.TimeoutSeconds.toString());
    });

    it("Should Do Redirect", function() {
        var timer = new TimeoutTimer();
        timer.Window = new Object();
        timer.Window.location = new Object();
        timer.Window.location.href = 'stuff';
        timer.DoRedirect("things");
        expect(timer.Window.location.href).toBe("things");
    });
});