<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="Agrishare.Web.Pages.Homepage" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">

    <div class="background">
        <div></div>
    </div>
    
    <div class="slideshow" ng-controller="SlideshowController">
        <div ng-class="{'visible': index==0 }">
            <h2><asp:Literal runat="server" ID="ListingCount" /> Live Listings<br />
                <asp:Literal runat="server" ID="UserCount" /> Active Users</h2>
            <p>Join our active community today</p>
            <p><a href="/account/get-started">Get Started</a></p>
        </div>
        <div ng-class="{'visible': index==1 }">
            <h2>Linking farmers in Africa to agricultural resources</h2>
            <p>AgriShare is an app to hire or rent out agricultural resources between farmers and equipment manufacturers securely and with ease.</p>
            <p><a href="/account/get-started">Get Started</a></p>
        </div>
        <div ng-class="{'visible': index==2 }">
            <h2>Share Agricultural resources</h2>
            <p>Find farming equipment in your area and hire your equipment out to other farmers.</p>
            <p><a href="/account/get-started">Get Started</a></p>
        </div>
    </div>

    <div class="touts">
        <div>
            <img src="/Resources/Images/Tout-Seeking.svg" />
            <h2>Seeking</h2>
            <p>Are you looking for some agricultural resources to hire? Look through the listings to find something in your area and check if it is available.</p>
            <p><a href="/account/get-started" class="button">Get Started</a></p>
        </div>        
        <div>
            <img src="/Resources/Images/Tout-Offering.svg" />
            <h2>Offering</h2>
            <p>Would you like to hire out your farming resources to other farmers for extra money? Register on the app or the site, and add your equipment to hire it out.</p>
            <p><a href="/account/get-started" class="button">Get Started</a></p>
        </div>
    </div>

    <div class="apps">
        <div>
            <img src="/Resources/Images/Screenshot.png" />
        </div>
        <div>
            <h2>Download the app</h2>
            <p>Use the AgriShare app to advertise the farming equipment that you would like to hire out to other farmers. Get the easy to use AgriShare app to find out which farming equipment is available for hire or to buy. </p>
            <p>
                <a href="https://play.google.com/store/apps/details?id=app.agrishare"><img src="/Resources/Images/Google-Play.png" alt="Download on Google Play" /></a>
            </p>
        </div>
    </div>

    <div class="social">
        <div>
            <a class="twitter-timeline" href="https://twitter.com/agri_share?ref_src=twsrc%5Etfw">Tweets by agri_share</a> <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>
        </div>
        <div>
            <iframe src="https://www.facebook.com/plugins/page.php?href=https%3A%2F%2Fwww.facebook.com%2FAgriShareZim%2F&tabs=timeline&width=480&height=4000&small_header=false&adapt_container_width=true&hide_cover=false&show_facepile=true&appId=146461798748713" width="480" height="4000" data-adapt-container-width="true" style="border:none;overflow:hidden; width: 100% !important;" scrolling="no" frameborder="0" allowTransparency="true" allow="encrypted-media"></iframe>
        </div>
    </div>

    <div class="testimony">
        <p><span>Phinneas Chikombo was able to harvest his crop on time thanks to AgriShare</span></p>
    </div>

</asp:Content>
