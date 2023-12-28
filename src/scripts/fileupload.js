$(function () {
    var uploader = new Uploader();
    uploader.init();
});

function Uploader() {
    var self = this;

    self.pax = $('#ParticipantID').val();
    self.maxSize = 5242880;
    self.fileTypes = ["image/png", "image/jpeg", "image/jpg"];
    self.fileinput = function () { return $('#file-input')[0]; }

    self.init = function() {
        if (Modernizr.xhr2)
            self.bindXhr();
        else
            self.oldBrowser();
    }

    self.oldBrowser = function() {
        // ie9 and earlier
        $('#fallback').show();
        $('#modern-browsers').hide();
    }

    self.bindXhr = function() {
        $('#browse-button').on('click', self.triggerFileClick);
        $('#file-input').on('change', self.handleFileChange);

        if (Modernizr.draganddrop)
            self.bindDragAndDrop();
    }

    self.triggerFileClick = function() {
        $('#file-input').trigger('click');
        return true;
    }

    self.handleFileChange = function () {
        if (self.fileinput().files.length > 0)
            self.upload(self.fileinput().files[0]);
    }

    self.bindDragAndDrop = function() {
        $('#dragdrop').show();
        $('#dragdrop').on('dragover', self.dragover);
        $('#dragdrop').on('dragleave', self.dragleave);
        $('#dragdrop').on('drop', self.drop);
    }

    self.dragover = function(e) {
        e.preventDefault();
        $('#dragdrop').removeClass('error wrong-type too-big').addClass('over');
    }

    self.dragleave = function(e) {
        e.preventDefault();
        $('#dragdrop').removeClass('error wrong-type too-big over');
    }

    self.drop = function(e) {
        e.preventDefault();
        e.stopPropagation();
        $('#dragdrop').removeClass('over');
        if (e.originalEvent.dataTransfer) {
            if (e.originalEvent.dataTransfer.files.length > 0) {
                /*UPLOAD FILES HERE*/
                self.upload(e.originalEvent.dataTransfer.files[0]);
                $('#dragdrop').addClass('dropped');
            }
        }
    }

    self.loadImage = function() {
        $('.image-container img').attr('src', 'picture.aspx?imageid=' + self.pax);
        location.reload();
    }

    self.wrongType = function(type) {
        if (self.fileTypes.indexOf(type) == -1) {
            $('#dragdrop').removeClass('dropped').addClass('error wrong-type');
            return true;
        }
        return false;
    }

    self.tooBig = function(size) {
        if (size > self.maxSize) {
            $('#dragdrop').removeClass('dropped').addClass('error too-big');
            return true;
        }
        return false;
    }

    self.statechange = function() {
        if (this.readyState === 4) {
            $('#dragdrop').removeClass('dropped').addClass('success');
            self.loadImage();
            $('.progress').fadeOut('fast', function () {
                $('.progress-bar').css('width', '0%');
                $('.progress-bar').find('.sr-only').html('0% Complete'); // Fallback for unsupported browsers.
            });
        }
    }

    self.updateProgress = function(e) {
        if (e.lengthComputable) {
            var progress = (e.loaded / e.total) * 100;
            $('.progress-bar').css('width', progress + '%');
            $('.progress-bar').find('.sr-only').html(progress + '% Complete'); // Fallback for unsupported browsers.
        }
    }

    self.createXhr = function () {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'Upload.aspx', true);
        return xhr;
    }

    self.upload = function (file) {
        if (self.tooBig(file.size) || self.wrongType(file.type))
            return;
        var xhr = self.createXhr();
        xhr.setRequestHeader("Content-Type", file.type);

        $('.progress').show();

        // Listen to the upload progress.
        xhr.upload.onprogress = self.updateProgress;

        // notice that the event handler is on xhr and not xhr.upload
        xhr.addEventListener('readystatechange', self.statechange);

        xhr.send(file);
    }
}