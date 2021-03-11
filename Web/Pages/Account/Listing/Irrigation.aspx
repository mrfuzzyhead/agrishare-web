<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Irrigation" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Irrigation.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Irrigation" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Irrigation</h1>

    <p>Please provide the following details for your irrigation service.</p>

    <div class="form-cols">

        <div>

            <h3>Listing Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentTitle" Text="Title *" />
                <asp:TextBox runat="server" ID="EquipmentTitle" MaxLength="1024" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentTitle" Text="Title is required" Display="Dynamic" />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="GroupHire" Text="Allow group hire" />
            </div>

            <h3>Booking Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
                <div><asp:TextBox runat="server" ID="PricePerQuantityUnit" MaxLength="8" /><span>$/DAY</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>
    
            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDistanceToWaterSource" Text="Maximum distance to water source *" />
                <div><asp:TextBox runat="server" ID="MaximumDistanceToWaterSource" MaxLength="8" /><span>KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistanceToWaterSource" Text="Maximum distance to water source is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MaximumDistanceToWaterSource" Text="Maximum distance to water source is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDepthOfWaterSource" Text="Maximum depth of water source *" />
                <div><asp:TextBox runat="server" ID="MaximumDepthOfWaterSource" MaxLength="8" /><span>METRES</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDepthOfWaterSource" Text="Maximum depth of water source is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MaximumDepthOfWaterSource" Text="Maximum depth of water source is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
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
                <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance to farm *" />
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
