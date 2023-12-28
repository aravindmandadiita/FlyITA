/// <reference path="jasmine/jasmine.js" />
/// <reference path="frameworks/jquery-1.11.1.min.js" />
/// <reference path="jasmine/fixtures.js" />
/// <reference path="confirm.js" />

describe("confirm feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("confirm");
    });

    self.afterEach(function () {
        fixtures.cleanUp();
    });

    it("Should bind page", function () {
        spyOn(window, "toggleSection");
        spyOn(window, "getPaxInfo");
        spyOn(window, "hidePaxInfo");
        spyOn(window, "showAll");
        spyOn(window, "hideAll");
        bindPage();
        $('.section-toggle').trigger('click');
        $('.details').trigger('click');
        $('.hide-guest').trigger('click');
        $('.hide-all').trigger('click');
        $('.show-all').trigger('click');
        expect(window.toggleSection).toHaveBeenCalled();
        expect(window.getPaxInfo).toHaveBeenCalled();
        expect(window.hidePaxInfo).toHaveBeenCalled();
        expect(window.showAll).toHaveBeenCalled();
        expect(window.hideAll).toHaveBeenCalled();
    });

    it("Should toggle section visibility", function() {
        expect($('#section').hasClass('hidden')).toBeTruthy();
        expect($('.fa').hasClass('fa-chevron-circle-right')).toBeTruthy();
        bindPage();
        $('.section-toggle').trigger('click');
        expect($('#section').hasClass('hidden')).toBeFalsy();
        expect($('#section').hasClass('show')).toBeTruthy();
        expect($('.fa').hasClass('fa-chevron-circle-down')).toBeTruthy();
        $('.section-toggle').trigger('click');
        expect($('#section').hasClass('show')).toBeFalsy();
        expect($('#section').hasClass('hidden')).toBeTruthy();
        expect($('.fa').hasClass('fa-chevron-circle-right')).toBeTruthy();
    });

    it("should hide all sections", function() {
        $('.fa').removeClass('fa-chevron-circle-right').addClass('fa-chevron-circle-down');
        $('.section').addClass('show').removeClass('hidden');
        hideAll();
        expect($('.fa-chevron-circle-down').length).toBe(0);
        expect($('.fa-chevron-circle-right').length).toBe(1);
        expect($('.section.hidden').length).toBe(1);
        expect($('.section.show').length).toBe(0);
    });

    it("should show all sections", function () {
        $('.fa').removeClass('fa-chevron-circle-down').addClass('fa-chevron-circle-right');
        $('.section').addClass('hidden').removeClass('show');
        showAll();
        expect($('.fa-chevron-circle-down').length).toBe(1);
        expect($('.fa-chevron-circle-right').length).toBe(0);
        expect($('.section.hidden').length).toBe(0);
        expect($('.section.show').length).toBe(1);
    });

    it("should go to section", function () {
        var event = $.Event("click");
        var link = $.parseHTML('<a href="ProfileInformation.aspx">Edit</a>');
        spyOn(window, "setOkToGo");
        spyOn($, 'ajax').and.callFake(function(params) {
            params.success();
            expect(params.data.target).toBe("ProfileInformation.aspx");
        });
        goToSection.call(link, event);
        expect(event.isPropagationStopped()).toBeTruthy();
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect(window.setOkToGo).toHaveBeenCalled();
    });

    it("Should load guest information", function() {
        var guestjson = fixtures.loadJson("guestjson");
        spyOn($, "post").and.callFake(function(url, params, callback) {
            callback(guestjson);
        });
        getPaxInfo.call($('.details'));
        expect($.post).toHaveBeenCalled();
        expect($('#pnlGuestDetails').is(":visible")).toBeTruthy();
        expect($('#txtGLegalName').html()).toBe(guestjson.LegalName);
        expect($('#txtGNickName').html()).toBe(guestjson.Nickname);
        expect($('#txtGBadgeFirstName').html()).toBe(guestjson.BadgeFirst);
        expect($('#txtGBadgeLastName').html()).toBe(guestjson.BadgeLast);
        expect($('#txtGDOB').html()).toBe(guestjson.DOB);
        expect($('#txtGGender').html()).toBe(guestjson.Gender);
        expect($('#txtGProofOfCitizenship').html()).toBe(guestjson.CitizenshipType);
        expect($('#txtGDietaryPreferences').html()).toBe(guestjson.DietaryPreferences);
        expect($('#txtGPhysicalAssistanceRequests').html()).toBe(guestjson.PhysAssistanceReqs);
        expect($('#divGPassportDetails').is(":visible")).toBeTruthy();
        expect($('#txtGPFirstName').html()).toBe(guestjson.PassportFirst);
        expect($('#txtGPMiddleName').html()).toBe(guestjson.PassportMiddle);
        expect($('#txtGPLastName').html()).toBe(guestjson.PassportLast);
        expect($('#txtGPNumber').html()).toBe(guestjson.PassportNumber);
        expect($('#txtGPExpiration').html()).toBe(guestjson.PassportExpiration);
        expect($('#txtGPIssue').html()).toBe(guestjson.PassportIssueDate);
        expect($('#txtGCountryofIssue').html()).toBe(guestjson.PassportIssueCountry);
        expect($('#txtGPIssuingAuthority').html()).toBe(guestjson.PassportIssueAuth);
        expect($('#txtGNationality').html()).toBe(guestjson.PassportNationality);
    });

    it("Should hide guest info", function() {
        $('#pnlGuestDetails').show();
        var event = $.Event('click');
        hidePaxInfo(event);
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect($('#pnlGuestDetails').is(":visible")).toBeFalsy();
    });

    it("should check page when not ok to go", function() {
        okToGo = false;
        var result = checkPage();
        expect(result).toBe("You have not submitted your registration. Are you sure you want to leave this page before submitting?");
    });

    it("should check page when ok to go", function () {
        okToGo = true;
        var result = checkPage();
        expect(result).toBeUndefined();
    });

    it("should set ok to go", function() {
        okToGo = false;
        setOkToGo();
        expect(okToGo).toBeTruthy();
    });
});