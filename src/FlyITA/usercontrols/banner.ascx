<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="banner.ascx.cs" Inherits="FlyITA.usercontrols.banner" %>
<div id="banner-site">
 <!-- Intro Section -->
        <section class="hero hidden-xs">
            <!-- Hero Slider Section -->
            <div class="page-banner"></div>
            <!-- End Hero Slider Section -->
        </section>
        <div class="clearfix"></div>
        <!-- End Intro Section -->
</div>

<div id="banner-home">
    <!-- Intro Section  -->
        <section class="hero">
            <!-- Intro Scroll Down -->
            <div class="intro-scroll-down text-center hidden-xs">
                <a class="scroll-down" href="#itinerary">
                    <span class="fa fa-chevron-down"></span>
                </a>
                <div style="height:70px;" class="hidden-xs"></div>
            </div>
            <!--  End Intro Scroll Down -->

            <!-- Hero Slider Section -->
            <div class="home-hero parallax parallax-section1">
            <div class="overlay-hero">
                <div class="container caption-hero">
                    <div class="inner-caption">
                        <h1>You're Traveling<br class="hidden-xs" /> in Good Company</h1>
                        <h4 class="h4slide">With over 50 years of Full Service Travel Management</h4>
                        <br />
                        <div>
                            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modalItinerary">View Itinerary and Flight Status</button>
                        </div>
                    </div>
                </div>
            </div>

            </div>
            <!-- End Hero Slider Section -->
        </section>
    <div id="scroll-to-point" class="hidden-xs"></div>
    <!-- End Intro Section -->
</div>
