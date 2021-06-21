<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Product" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Product" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Product</h1>

    <p>Please provide the following details for your Product.</p>

    <div class="form-cols">

        <div>

            <h3>Listing Details</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentTitle" Text="Title *" />
                <asp:TextBox runat="server" ID="EquipmentTitle" MaxLength="1024" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentTitle" Text="Title is required" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentDescription" Text="Description *" />
                <asp:TextBox runat="server" ID="EquipmentDescription" TextMode="MultiLine" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentDescription" Text="Description is required" Display="Dynamic" />
            </div>            

            <h3>Photo</h3>
            <web:PhotoUpload runat="server" ID="Gallery" Multiple="false" />

        </div>

        <div>

            <h3>Booking Details</h3>            

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="CostPerDay" Text="Cost/day *" />
                <asp:TextBox runat="server" ID="CostPerDay" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="CostPerDay" Text="Cost/day is required" Display="Dynamic" />
            </div>              

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Stock" Text="Stock *" />
                <asp:TextBox runat="server" ID="Stock" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Stock" Text="Stock is required" Display="Dynamic" />
            </div>    

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

</asp:Content>
