/// <reference path="jasmine/jasmine.js" />
/// <reference path="frameworks/jquery-1.11.1.min.js" />
/// <reference path="frameworks/bootstrap.min.js" />
/// <reference path="jasmine/fixtures.js" />
/// <reference path="formvalidation.js" />

describe("form validation feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("forms");
    });

    self.afterEach(function () {
        fixtures.cleanUp();
    });

    it("Should init", function () {
        var validator = new FormValidation();
        spyOn(validator, "validateField");
        spyOn(validator, "handleSubmit");
        validator.init();
        $('.submit-button').trigger('click');
        expect(validator.handleSubmit).toHaveBeenCalled();
        $('.required select').trigger('change');
        expect(validator.validateField.calls.count()).toEqual(1);
        $('.required input').trigger('blur');
        expect(validator.validateField.calls.count()).toEqual(2);
        $('.required input').trigger('keyup');
        expect(validator.validateField.calls.count()).toEqual(3);
    });

    it("Should handle submit", function() {
        var validator = new FormValidation();
        spyOn(validator, "validateForm").and.returnValue(true);
        expect($('.submit-button').val()).toBe('Save and Continue');
        validator.handleSubmit();
        expect($('.submit-button').val()).toBe('Please Wait...');
    });

    it("Should validate field when invalid", function() {
        var validator = new FormValidation();
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeFalsy();
        validator.validateField.call($('#txtLegalFirstName'));
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeTruthy();
    });

    it("Should validate field when valid", function () {
        var validator = new FormValidation();
        $('#txtLegalFirstName').val("Bruce");
        $('#txtLegalFirstName').closest('.form-group').addClass('has-error');
        validator.validateField.call($('#txtLegalFirstName'));
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeFalsy();
    });

    it("Should apply errors", function () {
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeFalsy();
        expect($('#ddlGender').closest('.form-group').hasClass('has-error')).toBeFalsy();
        $('#txtLegalFirstName').val("Bruce");
        var validator = new FormValidation();
        validator.applyErrors();
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeFalsy();
        expect($('#ddlGender').closest('.form-group').hasClass('has-error')).toBeTruthy();
    });

    it("Should clear errors", function() {
        $('#ddlGender').closest('.form-group').addClass('has-error');
        $('#txtLegalFirstName').closest('.form-group').addClass('has-error');
        var validator = new FormValidation();
        validator.clearErrors();
        expect($('#txtLegalFirstName').closest('.form-group').hasClass('has-error')).toBeFalsy();
        expect($('#ddlGender').closest('.form-group').hasClass('has-error')).toBeFalsy();
    });

    it("Should should determine support for HTML5 form validation", function() {
        var validator = new FormValidation();
        var form = $('form')[0];
        var result = validator.supportsValidation();
        expect(result).toBeTruthy();
        form.checkValidity = undefined;
        spyOn(validator, 'form').and.returnValue(form);
        result = validator.supportsValidation();
        expect(result).toBeFalsy();
    });

    it("Should return form valid if no html5 support", function () {
        var validator = new FormValidation();
        spyOn(validator, 'supportsValidation').and.returnValue(false);
        var result = validator.validForm();
        expect(result).toBeTruthy();
    });

    it("Should return form valid state if html5 support exists", function () {
        var validator = new FormValidation();
        var result = validator.validForm();
        expect(result).toBeFalsy();
    });

    it("Should validate Form with no html5 validation", function () {
        var validator = new FormValidation();
        spyOn(validator, 'clearErrors');
        spyOn(validator, 'supportsValidation').and.returnValue(false);
        var result = validator.validateForm();
        expect(validator.clearErrors).toHaveBeenCalled();
        expect(result).toBeTruthy();
    });

    it("Should validate Form", function () {
        var validator = new FormValidation();
        spyOn(validator, 'clearErrors');
        spyOn(validator, 'supportsValidation').and.returnValue(true);
        spyOn(validator, 'validForm').and.returnValue(true);
        var result = validator.validateForm();
        expect(validator.clearErrors).toHaveBeenCalled();
        expect(validator.supportsValidation).toHaveBeenCalled();
        expect(validator.validForm).toHaveBeenCalled();
        expect(result).toBeTruthy();
    });

    it("Should validate Form when not valid", function () {
        var validator = new FormValidation();
        spyOn(validator, 'clearErrors');
        spyOn(validator, 'applyErrors');
        spyOn(validator, 'supportsValidation').and.returnValue(true);
        spyOn(validator, 'validForm').and.returnValue(false);
        var result = validator.validateForm();
        expect(validator.clearErrors).toHaveBeenCalled();
        expect(validator.applyErrors).toHaveBeenCalled();
        expect(validator.supportsValidation).toHaveBeenCalled();
        expect(validator.validForm).toHaveBeenCalled();
        expect(result).toBeFalsy();
    });

});