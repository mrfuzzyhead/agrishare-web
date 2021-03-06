﻿<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Bus" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Bus.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Bus" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Bus</h1>

    <p>Please provide the following details for your bus service.</p>

    <div class="form-cols">

        <div>

            <h3>Listing Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentTitle" Text="Title *" />
                <asp:TextBox runat="server" ID="EquipmentTitle" MaxLength="1024" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentTitle" Text="Title is required" Display="Dynamic" />
            </div>

            <h3>Booking Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="TimePerQuantityUnit" Text="Hours required per 100KM *" />
                <div><asp:TextBox runat="server" ID="TimePerQuantityUnit" MaxLength="8" /><span>HRS/100KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per 100KM is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TimePerQuantityUnit" Text="Hours required per 100KM is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <h3>Photos</h3>
            <web:PhotoUpload runat="server" ID="Gallery" />

        </div>

        <div>

            <h3>Location</h3>

            <div class="form-row">
                <label>Location</label>
                <web:Map runat="server" Id="Location" />
            </div>

            <div class="form-row mobile-row">
                <asp:Label runat="server" AssociatedControlID="DistanceCharge" Text="Distance charge *" />
                <div><asp:TextBox runat="server" ID="DistanceCharge" MaxLength="8" /><span>$/KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="DistanceCharge" Text="Distance charge is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="DistanceCharge" Text="Distance charge is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row mobile-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance *" />
                <div><asp:TextBox runat="server" ID="MaximumDistance" MaxLength="8" /><span>KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

</asp:Content>
