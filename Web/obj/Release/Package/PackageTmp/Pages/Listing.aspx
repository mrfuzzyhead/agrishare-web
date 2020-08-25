<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Booking Details" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Listing.aspx.cs" Inherits="Agrishare.Web.Pages.Listing" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="ListingTitle" /></h1>
    <p style="margin-top: 0"><asp:HyperLink runat="server" ID="Reviews" /></p>

    <div class="cols">
        <div>
            
            <p><asp:Literal runat="server" ID="ListingDescription" /></p>            

        </div>
        <div>

            <asp:Repeater runat="server" ID="Gallery" OnItemDataBound="BindPhoto">
                <HeaderTemplate>
                    <ul class="gallery">
                </HeaderTemplate>
                <ItemTemplate>
                        <li>
                            <asp:Image runat="server" ID="Thumb" />
                        </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>

            <ul class="details">
                <li><strong>Brand</strong><span><asp:Literal runat="server" ID="Brand" /></span></li>
                <li><strong>Horse Power</strong><span><asp:Literal runat="server" ID="HorsePower" /></span></li>
                <li><strong>Year</strong><span><asp:Literal runat="server" ID="Year" /></span></li>
            </ul>

        </div>
    </div>

</asp:Content>
