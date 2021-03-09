﻿<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Dashboard - Seeking Equipment" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Seeking.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Default" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking</h1>

    <h3>Find Services</h3>

    <ul class="search">
        <li>
            <a href="/account/seeking/tractor">
                <img src="/Resources/Images/Tractor.svg" />
                <span>Tractors</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/lorry">
                <img src="/Resources/Images/Lorry.svg" />
                <span>Lorries</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/processing">
                <img src="/Resources/Images/Processing.svg" />
                <span>Processing</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/bus">
                <img src="/Resources/Images/Bus.svg" />
                <span>Buses</span>
            </a>
        </li>
    </ul>

    <ul class="search">
        <li>
            <a href="/account/seeking/other">
                <img src="/Resources/Images/Equipment.svg" />
                <span>Equipment</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/irrigation">
                <img src="/Resources/Images/Irrigation.svg" class="tall" />
                <span>Irrigation</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/labour">
                <img src="/Resources/Images/Labour.svg" class="tall" />
                <span>Labour</span>
            </a>
        </li>
        <li>
            <a href="/account/seeking/land">
                <img src="/Resources/Images/Land.svg" class="tall" />
                <span>Land</span>
            </a>
        </li>
    </ul>

    <br />

    <div class="cols-rev">

        <div>

            <h3>Recent Notifications <a href="/account/notifications/seeking">View All</a></h3>

            <web:PagedRepeater runat="server" ID="Notifications" OnItemDataBound="BindNotification" EmptyMessage="You have no unread notifications">
                <HeaderTemplate>
                    <div class="notifications-summary-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <strong><asp:Literal runat="server" ID="Title" /> <em><asp:Literal runat="server" ID="TimeAgo" /></em></strong>
                            <span><asp:Literal runat="server" ID="Message" /></span>
                            <small><asp:Literal runat="server" ID="Date" /> &bull; <asp:Literal runat="server" ID="Listing" /></small>
                            <strong runat="server" ID="Action" class="button" />
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

        </div>

        <div>
    
            <h3>My Bookings <a href="/account/bookings/seeking">View All</a></h3>

            <asp:Panel runat="server" ID="Summary" CssClass="summary">
                <div>
                    <small><asp:Literal runat="server" Id="LeftSummaryTitle" /></small>
                    <span><asp:Literal runat="server" Id="LeftSummaryAmount" /></span>
                </div>
                <div>
                    <small>All Time</small>
                    <span><asp:Literal runat="server" Id="AllTimeSummary" /></span>
                </div>
            </asp:Panel>            

            <web:PagedRepeater runat="server" ID="Bookings" OnItemDataBound="BindBooking" EmptyMessage="You have not made any bookings yet">
                <HeaderTemplate>
                    <div class="bookings-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <span runat="server" ID="Photo" />
                            <span>
                                <small><asp:Literal runat="server" ID="Date" /></small>
                                <span><asp:Literal runat="server" ID="Title" /></span>
                                <strong><asp:Literal runat="server" ID="Price" /></strong>
                            </span>
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>
            

        </div>

    </div>

</asp:Content>
