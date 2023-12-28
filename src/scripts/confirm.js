var okToGo = false;
$(function () {
    bindPage();
});

function bindPage() {
    $('.section-toggle').on('click', toggleSection);
    $('.section-toggle a').on('click', goToSection);
    $(document).on('click', '.details', getPaxInfo);
    $(document).on('click', '.hide-guest', hidePaxInfo);
    $(document).on('click', '.show-all', showAll);
    $(document).on('click', '.hide-all', hideAll);
    $(window).on('beforeunload', checkPage);
    $('a[href!="logout.aspx"]').on('click', setOkToGo);
    $('form').on('submit', setOkToGo);
}

function checkPage() {
    if (!okToGo) 
        return "You have not submitted your registration. Are you sure you want to leave this page before submitting?";
}

function setOkToGo() {
    okToGo = true;
}

function toggleSection(e) {
    e.preventDefault();
    var target = $(this).data('target');
    if ($(target).hasClass('hidden')) {
        $(target).removeClass('hidden').addClass('show');
        $(this).find('.fa').removeClass('fa-chevron-circle-right').addClass('fa-chevron-circle-down');
    } else {
        $(target).removeClass('show').addClass('hidden');
        $(this).find('.fa').removeClass('fa-chevron-circle-down').addClass('fa-chevron-circle-right');
    }
}

function showAll() {
    $('.section').removeClass('hidden').addClass('show');
    $('.fa').removeClass('fa-chevron-circle-right').addClass('fa-chevron-circle-down');
}

function hideAll() {
    $('.section').removeClass('show').addClass('hidden');
    $('.fa').removeClass('fa-chevron-circle-down').addClass('fa-chevron-circle-right');
}

function goToSection(e) {
    setOkToGo();
    e.stopPropagation();
    e.preventDefault();
    var target = $(this).attr('href');
    $.ajax({url: 'navigation.ashx',
        type: "GET",
        cache: false,
        data: { "target": target },
        dataType: "json",
        success: function() {
            window.location = target;
        }
    });
}

function getPaxInfo() {
    $('#pnlGuestDetails').hide();
    var paxId = $(this).data('paxid');
    $.post("getPAXInfo.aspx", { "paxid": paxId }, function(data) {
        $('#txtGLegalName').html(data.LegalName);
        $('#txtGNickName').closest('.col-xs-12').hide();
        if (data.Nickname != "") {
            $('#txtGNickName').html(data.Nickname).closest('.col-xs-12').show();
        }
        $('#txtGBadgeFirstName').closest('.col-xs-12').hide();
        if (data.BadgeFirst != "" || data.BadgeLast != "") {
            $('#txtGBadgeFirstName').closest('.col-xs-12').show();
            $('#txtGBadgeFirstName').html(data.BadgeFirst);
            $('#txtGBadgeLastName').html(data.BadgeLast);
        }
        $('#txtGDOB').closest('.col-xs-12').hide();
        if (data.DOB != "") {
            $('#txtGDOB').html(data.DOB).closest('.col-xs-12').show();
        }
        $('#txtGGender').html(data.Gender);
        $('#txtGProofOfCitizenship').html(data.CitizenshipType);

        $('#txtGDietaryPreferences').closest('.col-xs-12').hide();
        $('#txtGPhysicalAssistanceRequests').closest('.col-xs-12').hide();
        $('#txtGPhysicalAssistanceRequests').closest('.col-md-6').hide();
        if (data.DietaryPreferences != "") {
            $('#txtGDietaryPreferences').html(data.DietaryPreferences.replace(/\,/g, '')).closest('.col-xs-12').show().closest('.col-md-6').show();
            $('#txtGDietaryPreferences').html(data.DietaryPreferences.replace(':', '')).closest('.col-xs-12').show().closest('.col-md-6').show();
        }
        if (data.PhysAssistanceReqs != "") {
            $('#txtGPhysicalAssistanceRequests').html(data.PhysAssistanceReqs.replace(/\,/g, '')).closest('.col-xs-12').show().closest('.col-md-6').show();
            $('#txtGPhysicalAssistanceRequests').html(data.DietaryPreferences.replace(':', '')).closest('.col-xs-12').show().closest('.col-md-6').show();
        }
        $('#divGPassportDetails').hide();
        if (data.PassportVisible) {
            $('#txtGPFirstName').html(data.PassportFirst);
            $('#txtGPMiddleName').html(data.PassportMiddle);
            $('#txtGPLastName').html(data.PassportLast);
            $('#txtGPNumber').html(data.PassportNumber);
            $('#txtGPExpiration').html(data.PassportExpiration);
            $('#txtGPIssue').html(data.PassportIssueDate);
            $('#txtGCountryofIssue').html(data.PassportIssueCountry);
            $('#txtGPIssuingAuthority').html(data.PassportIssueAuth);
            $('#txtGNationality').html(data.PassportNationality);
            $('#divGPassportDetails').show();
        }
        $('#pnlGuestDetails').slideDown();
    }, "json");
}

function hidePaxInfo(e) {
    e.preventDefault();
    $('#pnlGuestDetails').slideUp();
}