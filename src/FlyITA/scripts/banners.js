$(function () {
    if ($('#banner-home').is(':visible'))
        loadHomeBannerImages();
    if ($('#banner-site').is(':visible'))
        loadInternalBannerImages();
});

function loadHomeBannerImages() {
    $('#image1').attr('src', 'images/gallery/01.jpg');
    $('#image2').attr('src', 'images/gallery/02.jpg');
    $('#image3').attr('src', 'images/gallery/03.jpg');
    $('#image4').attr('src', 'images/gallery/04.jpg');
}

function loadInternalBannerImages() {
    $('#internal-banner').attr('src', 'images/banner-bkg.jpg');
}