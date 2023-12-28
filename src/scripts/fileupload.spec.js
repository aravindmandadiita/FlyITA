/// <reference path="jasmine/jasmine.js" />
/// <reference path="frameworks/jquery-1.11.1.min.js" />
/// <reference path="frameworks/modernizr-2.8.2.js" />
/// <reference path="frameworks/bootstrap.js" />
/// <reference path="jasmine/fixtures.js" />
/// <reference path="fileupload.js" />

describe("file upload feature", function () {
    var self = this;

    self.beforeEach(function () {
        $.fx.off = true;
        fixtures.CreateFixtures();
        fixtures.loadFixture("fileuploadpage");
    });

    self.afterEach(function () {
        fixtures.cleanUp();
    });

    it("Should bind page with xhr support", function () {
        var uploader = new Uploader();
        Modernizr.xhr2 = true;
        spyOn(uploader, 'bindXhr');
        uploader.init();
        expect(uploader.bindXhr).toHaveBeenCalled();
    });

    it("Should bind page without xhr support", function () {
        var uploader = new Uploader();
        Modernizr.xhr2 = false;
        spyOn(uploader, 'oldBrowser');
        uploader.init();
        expect(uploader.oldBrowser).toHaveBeenCalled();
    });

    it("Should fall back for old browsers", function() {
        expect($('#fallback').is(":visible")).toBeFalsy();
        expect($('#modern-browsers').is(":visible")).toBeTruthy();
        var uploader = new Uploader();
        uploader.oldBrowser();
        expect($('#fallback').is(":visible")).toBeTruthy();
        expect($('#modern-browsers').is(":visible")).toBeFalsy();
    });

    it("Should bind xhr", function() {
        var uploader = new Uploader();
        Modernizr.draganddrop = true;
        spyOn(uploader, "bindDragAndDrop");
        spyOn(uploader, "triggerFileClick");
        spyOn(uploader, "handleFileChange");
        uploader.bindXhr();
        $('#browse-button').trigger('click');
        $('#file-input').trigger('change');
        expect(uploader.bindDragAndDrop).toHaveBeenCalled();
        expect(uploader.triggerFileClick).toHaveBeenCalled();
        expect(uploader.handleFileChange).toHaveBeenCalled();
    });

    it("Should get file input", function() {
        var uploader = new Uploader();
        var input = uploader.fileinput();
        expect(input).toEqual($('#file-input')[0]);
    });

    it("Should trigger click", function() {
        var uploader = new Uploader();
        var result = uploader.triggerFileClick();
        expect(result).toBeTruthy();
    });

    it("Should not upload files when no files selected", function() {
        var uploader = new Uploader();
        spyOn(uploader, "upload");
        Modernizr.draganddrop = false;
        uploader.bindXhr();
        $('#file-input').trigger('change');
        expect(uploader.upload).not.toHaveBeenCalled();
    });

    it("Should bind drag and drop", function() {
        var uploader = new Uploader();
        expect($('#dragdrop').is(':visible')).toBeFalsy();
        spyOn(uploader, "dragover");
        spyOn(uploader, "dragleave");
        spyOn(uploader, "drop");
        uploader.bindDragAndDrop();
        expect($('#dragdrop').is(':visible')).toBeTruthy();
        $('#dragdrop').trigger("dragover");
        expect(uploader.dragover).toHaveBeenCalled();
        $('#dragdrop').trigger("dragleave");
        expect(uploader.dragleave).toHaveBeenCalled();
        $('#dragdrop').trigger("drop");
        expect(uploader.drop).toHaveBeenCalled();
    });

    it("Should handle drag over", function() {
        var uploader = new Uploader();
        $('#dragdrop').addClass('error wrong-type too-big');
        var event = $.Event("dragover");
        uploader.dragover(event);
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect($('#dragdrop').hasClass('error wrong-type too-big')).toBeFalsy();
        expect($('#dragdrop').hasClass('over')).toBeTruthy();
    });

    it("Should handle drag leave", function () {
        var uploader = new Uploader();
        $('#dragdrop').addClass('error wrong-type too-big over');
        var event = $.Event("dragleave");
        uploader.dragleave(event);
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect($('#dragdrop').hasClass('error wrong-type too-big over')).toBeFalsy();
    });

    it("Should handle droppin the bass", function () {
        var uploader = new Uploader();
        spyOn(uploader, "upload");
        $('#dragdrop').addClass('over');
        var event = $.Event("drop");
        event.originalEvent = $.Event("drop");
        event.originalEvent.dataTransfer = new function() { this.files = [ 0, 1 ]};
        event.originalEvent.dataTransfer.files = [ 1 ];
        uploader.drop(event);
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect(event.isPropagationStopped()).toBeTruthy();
        expect($('#dragdrop').hasClass('over')).toBeFalsy();
        expect($('#dragdrop').hasClass('dropped')).toBeTruthy();
        expect(uploader.upload).toHaveBeenCalled();
    });

    it("Should handle droppin the bass with no files", function () {
        var uploader = new Uploader();
        spyOn(uploader, "upload");
        $('#dragdrop').addClass('over');
        var event = $.Event("drop");
        event.originalEvent = $.Event("drop");
        event.originalEvent.dataTransfer = new function () { this.files = [0]; };
        event.originalEvent.dataTransfer.files = [];
        uploader.drop(event);
        expect(event.isDefaultPrevented()).toBeTruthy();
        expect(event.isPropagationStopped()).toBeTruthy();
        expect($('#dragdrop').hasClass('over')).toBeFalsy();
        expect($('#dragdrop').hasClass('dropped')).toBeFalsy();
        expect(uploader.upload).not.toHaveBeenCalled();
    });

    it("Should handle file change", function() {
        var uploader = new Uploader();
        var input = new Object();
        input.files = [0, 1];
        spyOn(uploader, "fileinput").and.returnValue(input);
        spyOn(uploader, "upload");
        uploader.handleFileChange();
        expect(uploader.upload).toHaveBeenCalled();
    });

    it("Should load image", function() {
        var uploader = new Uploader();
        uploader.loadImage();
        expect($('.image-container img').attr('src')).toBe('picture.aspx?imageid=123');
    });

    it("Should determine wrong file type", function() {
        var uploader = new Uploader();
        $('#dragdrop').addClass('dropped');
        var result = uploader.wrongType("text/css");
        expect($('#dragdrop').hasClass('dropped')).toBeFalsy();
        expect($('#dragdrop').hasClass('error wrong-type')).toBeTruthy();
        expect(result).toBeTruthy();
    });

    it("Should determine correct file type", function () {
        var uploader = new Uploader();
        $('#dragdrop').addClass('dropped');
        var result = uploader.wrongType("image/jpeg");
        expect($('#dragdrop').hasClass('dropped')).toBeTruthy();
        expect($('#dragdrop').hasClass('error wrong-type')).toBeFalsy();
        expect(result).toBeFalsy();
    });

    it("Should determine too large file size", function () {
        var uploader = new Uploader();
        uploader.maxSize = 1000;
        $('#dragdrop').addClass('dropped');
        var result = uploader.tooBig(1500);
        expect($('#dragdrop').hasClass('dropped')).toBeFalsy();
        expect($('#dragdrop').hasClass('error too-big')).toBeTruthy();
        expect(result).toBeTruthy();
    });

    it("Should determine ok file size", function () {
        var uploader = new Uploader();
        $('#dragdrop').addClass('dropped');
        var result = uploader.tooBig(1500);
        expect($('#dragdrop').hasClass('dropped')).toBeTruthy();
        expect($('#dragdrop').hasClass('error too-big')).toBeFalsy();
        expect(result).toBeFalsy();
    });

    it("Should reset page on complete upload", function() {
        $('#dragdrop').addClass('dropped');
        var uploader = new Uploader();
        spyOn(uploader, 'loadImage');
        $('.progress').show();
        $('.progress-bar').css('width', '90%');
        $('.progress-bar').find('.sr-only').html('90% Complete');
        var obj = new Object();
        obj.readyState = 4;
        $(obj).on('readystatechange', uploader.statechange);
        $(obj).trigger('readystatechange');
        expect(uploader.loadImage).toHaveBeenCalled();
        expect($('.progress').is(":visible")).toBeFalsy();
        expect($('.progress-bar').css('width')).toBe('0%');
        expect($('.progress-bar').find('.sr-only').html()).toBe('0% Complete');
    });

    it("Should compute and display upload progress", function() {
        var e = $.Event('progress');
        e.lengthComputable = true;
        e.loaded = 500;
        e.total = 1000;
        var uploader = new Uploader();
        uploader.updateProgress(e);
        expect($('.progress-bar').css('width')).toBe('50%');
        expect($('.progress-bar').find('.sr-only').html()).toBe('50% Complete');
    });

    it("Should create xhr", function() {
        var uploader = new Uploader();
        var xhr = uploader.createXhr();
        expect(xhr).toEqual(jasmine.any(XMLHttpRequest));
    });

    it("Should not upload file when incorrect file type", function() {
        var uploader = new Uploader();
        uploader.maxSize = 1499;
        var xhr = new XMLHttpRequest();
        spyOn(xhr, "send");
        spyOn(uploader, "createXhr").and.returnValue(xhr);
        var file = new Object();
        file.type = "text/css";
        file.size = 1500;
        uploader.upload(file);
        expect(xhr.send).not.toHaveBeenCalled();
    });

    it("Should upload file when correct file type", function () {
        var uploader = new Uploader();
        var xhr = uploader.createXhr();
        spyOn(xhr, "send");
        spyOn(uploader, "createXhr").and.returnValue(xhr);
        var file = new Object();
        file.type = "image/png";
        file.size = 1500;
        uploader.upload(file);
        expect(xhr.send).toHaveBeenCalled();
        expect($('.progress').is(':visible')).toBeTruthy();
    });

});