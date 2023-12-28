/// <reference path="jasmine/jasmine.js" />
/// <reference path="frameworks/jquery-1.11.1.min.js" />
/// <reference path="frameworks/bootstrap.js" />
/// <reference path="frameworks/moment.min.js" />
/// <reference path="frameworks/bootstrap-datetimepicker.min.js" />
/// <reference path="jasmine/fixtures.js" />
/// <reference path="jasmine/WebFormsMocks.js" />
/// <reference path="main.js" />
/// <reference path="guestinformation.js" />

describe("guest information feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("guestinfo");
    });

    self.afterEach(function () {
        fixtures.cleanUp();
    });

    it("Should bind page", function () {
        spyOn(window, "toggleYes");
        spyOn(window, "toggleNo");
        expect($('#addGuestSection').is(":visible")).toBeFalsy();
        bindPage();
        expect($('#rbGuestYes').is(":checked")).toBeTruthy();
        expect(window.toggleYes).toHaveBeenCalled();
        $('#rbGuestNo').trigger('click');
        expect(window.toggleNo).toHaveBeenCalled();
    });

    it("Should toggle Yes", function() {
        $('.form-group.required').find('input, select').removeAttr('required');
        $('#rbGuestYes').prop('checked', true);
        toggleYes();
        expect($('#addGuestSection').is(":visible")).toBeTruthy();
        expect($('#btnSaveAndContinue').val()).toBe('Add Guest');
        expect($('#txtDateOfBirth').attr('required')).toBe('required');
    });

    it("Should toggle No", function () {
        $('#addGuestSection').show();
        $('#btnSaveAndContinue').val('Add Guest');
        $('#rbGuestNo').prop('checked', true);
        toggleNo();
        expect($('#addGuestSection').is(":visible")).toBeFalsy();
        expect($('#btnSaveAndContinue').val()).toBe('Save and Continue');
        expect($('#txtDateOfBirth').attr('required')).toBeUndefined();
    });

    it("Should webforms rebind", function() {
        var endrequest = jasmine.createSpy('add_endRequest');
        var prm = new Object();
        prm.add_endRequest = endrequest;
        webformsRebind(prm);
        expect(endrequest).toHaveBeenCalled();
    });
});