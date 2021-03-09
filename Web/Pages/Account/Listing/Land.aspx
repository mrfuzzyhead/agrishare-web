<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Lorry" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Land.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Land" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Land</h1>

    <p>Please provide the following details for your land to rent.</p>

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
            
            <h3>Land Details</h3>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="UnclearedLand" Text="Uncleared land"  />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="ClearedLand" Text="Cleared land"  />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="NearWaterSource" Text="Water Source Nearby"  />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="FertileSoil" Text="Fertile soil"  />
            </div>


            <h3>Booking Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Rent *" />
                <div><asp:TextBox runat="server" ID="PricePerQuantityUnit" MaxLength="8" /><span>$/YEAR</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Rent is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Rent is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>
    
            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MaxRentalYears" Text="Maximum rental period *" />
                <div><asp:TextBox runat="server" ID="MaxRentalYears" MaxLength="8" /><span>YEARS</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaxRentalYears" Text="Maximum rental is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MaxRentalYears" Text="Maximum rental is invalid" ValidationExpression="^[\d]+$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="AvailableAcres" Text="Available land *" />
                <div><asp:TextBox runat="server" ID="AvailableAcres" MaxLength="8" /><span>ACRES</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="AvailableAcres" Text="Available land is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="AvailableAcres" Text="Available land is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MinimumAcres" Text="Minimum land rental *" />
                <div><asp:TextBox runat="server" ID="MinimumAcres" MaxLength="8" /><span>ACRES</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MinimumAcres" Text="Minimum land rental is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MinimumAcres" Text="Minimum land rental is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

        </div>

        <div>

            <h3>Location</h3>

            <div class="form-row">
                <label>Location</label>
                <web:Map runat="server" Id="Location" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="LandRegion" Text="Region *" />
                <div><asp:DropDownList runat="server" ID="LandRegion" /></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="LandRegion" Text="Region is required" Display="Dynamic" />
            </div>

            <h3>Photos</h3>
            <web:PhotoUpload runat="server" ID="Gallery" />

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

</asp:Content>
