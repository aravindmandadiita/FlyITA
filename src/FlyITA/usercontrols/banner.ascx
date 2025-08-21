<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="banner.ascx.cs" Inherits="FlyITA.usercontrols.banner" %>
<div id="banner-site">
        <section class="hero hidden-xs">
            <div class="page-banner"></div>
        </section>
        <div class="clearfix"></div>
</div>

<div id="banner-home">
        <section class="hero">
            <!-- Intro Scroll Down -->
            <div class="intro-scroll-down text-center hidden-xs">
                <a class="scroll-down" href="#itinerary">
                    <span class="fa fa-chevron-down"></span>
                </a>
                <div style="height:70px;" class="hidden-xs"></div>
            </div>
            <!--  End Intro Scroll Down -->

            <!-- Hero Section -->
            <div class="home-hero parallax parallax-section1">
                <div class="container caption-hero">
                    <h1>You're Traveling<br class="hidden-xs" /> in Good Company</h1>
                    <h4 class="h4slide">With over 50 years of Full Service Travel Management</h4>
                    <p><button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modalItinerary">View Itinerary and Flight Status</button></p>
                </div>
            </div>
            <!-- End Hero Section -->
        </section>
    <div id="scroll-to-point" class="hidden-xs"></div>

</div>
