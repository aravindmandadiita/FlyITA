$(function () {
    var validation = new FormValidation();
    validation.init();
});

function FormValidation() {
    var self = this;

    self.form = function () {return $('form')[0];}

    self.init = function() {
        $('.submit-button').on('click', self.toggleStickyHeader);
        $(document).on('keyup change blur', '.required input, .required select', self.validateField);
        $(document).on('click', '.submit-button', self.handleSubmit);
        
    }

    //hides the sticky header on invalid fields when user clicks submit button
    self.toggleStickyHeader = function () {
        $('header.sticky').removeClass('sticky');
    }

    self.handleSubmit = function() {
        if (self.validateForm()) {
            $('.submit-button').button('loading');
        }
    }

    self.validateField = function() {
        $(this).closest('.form-group').removeClass('has-error');
        var value = $(this).val();
        if ($.trim(value) == "")
            $(this).closest('.form-group').addClass('has-error');
    }

    self.validateForm = function () {
        self.clearErrors();
        if (self.supportsValidation()) {
            if (self.validForm())
                return true;
            else {
                // errors
                self.applyErrors();
                return false;
            }
        } else {
            return true;
        }
    }
    
    self.applyErrors = function() {
        $('.required input, .required select').each(function (i, obj) {
            if (obj.checkValidity && !obj.checkValidity()) {
                $(obj).closest('.form-group').addClass('has-error');
            }
        });
    }

    self.clearErrors = function() {
        $('.has-error').removeClass('has-error');
    }

    self.supportsValidation = function () {
        return self.form().checkValidity != undefined;
    }

    self.validForm = function () {
        if (self.supportsValidation())
            return self.form().checkValidity();
        return true;
    }
}