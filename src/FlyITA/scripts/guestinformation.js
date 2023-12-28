$(function () {
    bindPage();



});


function pageLoad() {
        
        initFliers();
        $('.frequent-flier .remove-flier').on('click', handleRemove);
        $('.frequent-flier .add-flier').on('click', handleAdd);
       
        $('#divDateOfBirth').datetimepicker({ pickTime: false, useCurrent: false });
       
        $('#passport-expiration').datetimepicker({ pickTime: false, useCurrent: false });
        $('#passport-issued').datetimepicker({ pickTime: false, useCurrent: false });

        $('#passport-expiration input').on('click', showDatePicker);
        $('#passport-issued input').on('click', showDatePicker);
          

}

function bindPage() {
    $('#guest-dob').datetimepicker({ pickTime: false, useCurrent: false });
    $('#rbGuestYes').on('click change', toggleYes);
    $('#rbGuestNo').on('click change', toggleNo);
    $('#rbGuestYes').prop('checked', true).closest('.btn').addClass('active');
    $('#rbGuestYes').trigger('change');


}

function toggleYes() {
    if ($('#rbGuestYes').is(':checked')) {
        $('#addGuestSection').show();
        $('#btnSaveAndContinue').val('Add Guest');
        $('.form-group.required').each(function (i, formobj) {
            $(formobj).find('input[type="text"], select').attr('required', 'required');
        });
    }
}

function toggleNo() {
    if ($('#rbGuestNo').is(':checked')) {
        $('#addGuestSection').hide();
        $('#btnSaveAndContinue').val('Save and Continue');
        $('.form-group.required').each(function (i, formobj) {
            $(formobj).find('input, select').removeAttr('required');
            $('#addGuestSection').find('input,select').val('');
        });
    }
}
function initFliers() {
    $('.person').each(function (x, person) {
        $(person).find('.frequent-flier .frequent-flier-number').each(function (i, obj) {
             //if ($(obj).find('select.form-control').val() == "" && $(obj).find('input.form-control').val() == "")
            $(obj).hide();
        });
        $(person).find('.frequent-flier .frequent-flier-number:first').show();
        handleDisableAdd(person);
        handleDisableRemove(person);
    });
}

function handleAdd(e) {
    e.preventDefault();
    var person = $(this).closest('.person');
    $(person).find('.remove-flier').removeAttr('disabled');
    $(person).find('.frequent-flier .frequent-flier-number:visible').last().next().slideDown();
    handleDisableAdd(person);
}

function handleRemove(e) {
    e.preventDefault();
    var person = $(this).closest('.person');
    $(person).find('.add-flier').removeAttr('disabled');
    if ($(person).find('.frequent-flier .frequent-flier-number:visible').length > 1) {
        $(person).find('.frequent-flier .frequent-flier-number:visible').last().find('.form-control').val('');
        $(person).find('.frequent-flier .frequent-flier-number:visible').last().slideUp(function () { handleDisableRemove(person); });
    }
}

function handleDisableAdd(person) {
    if ($(person).find('.frequent-flier .frequent-flier-number:visible').length >= $(person).find('.frequent-flier .frequent-flier-number').length)
        $(person).find('.frequent-flier .add-flier').attr('disabled', 'disabled');
}

function handleDisableRemove(person) {
    if ($(person).find('.frequent-flier .frequent-flier-number:visible').length <= 1)
        $(person).find('.frequent-flier .remove-flier').attr('disabled', 'disabled');
}
function webformsRebind(prm) {
    prm.add_endRequest(bindPage);
}
webformsRebind(Sys.WebForms.PageRequestManager.getInstance());