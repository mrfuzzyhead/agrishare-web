﻿<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Listings" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Listings.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listings" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="ListTitle" /></h1>

    <div class="cols">
        <div>
            
            <web:PagedRepeater runat="server" ID="ListingsList" OnItemDataBound="BindListing" PageSize="5" Visible="false">
                <HeaderTemplate>
                    <div class="listings-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <span runat="server" ID="Photo" />
                            <span>
                                <strong><asp:Literal runat="server" ID="Title" /></strong>
                                <span><asp:Literal runat="server" ID="Description" /></span>
                            </span>
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

            <web:PagedRepeater runat="server" ID="ProductList" OnItemDataBound="BindProduct" PageSize="5" Visible="false">
                <HeaderTemplate>
                    <div class="listings-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <span runat="server" ID="Photo" />
                            <span>
                                <strong><asp:Literal runat="server" ID="Title" /></strong>
                                <span><asp:Literal runat="server" ID="Description" /></span>
                            </span>
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

        </div>
        <div>

            <p style="margin-top: 30px">
                <asp:HyperLink runat="server" ID="AddButton" CssClass="button" Width="100%" />
            </p>

            <ul class="menu">
                <li><a href="/account/listings?cid=1">Tractors</a></li>
                <li><a href="/account/listings?cid=2">Lorries</a></li>
                <li><a href="/account/listings?cid=3">Processing</a></li>
                <li><a href="/account/listings?cid=17">Buses</a></li>                
                <li><a href="/account/listings?cid=60">Irrigation</a></li>
                <li><a href="/account/listings?cid=50">Labour</a></li>
                <li><a href="/account/listings?cid=70">Land</a></li>
                <li runat="server" id="EquipmentMenuItem"><a href="/account/listings?cid=0">Equipment</a></li>
            </ul>
        </div>
    </div>

    

</asp:Content>
