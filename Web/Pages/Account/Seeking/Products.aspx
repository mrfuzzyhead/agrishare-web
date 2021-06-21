<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Products" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Products" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1><asp:Literal runat="server" ID="ProductPageTitle" Text="Products" /></h1>

    <div class="cols">
        <div>

            <asp:Panel runat="server" ID="ProductSearch" CssClass="product-search" Visible="false">
                <asp:TextBox runat="server" ID="KeywordSearch" placeholder="Find products" ng-model="keywords" type="search" />
                <a ng-click="productSearch(keywords)">Search</a>
            </asp:Panel>

            <web:PagedRepeater runat="server" ID="ProductList" OnItemDataBound="BindProduct" PageSize="20" Visible="false">
                <HeaderTemplate>
                    
                    <div class="listings-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <asp:HyperLink runat="server" ID="Link">
                            <span runat="server" ID="Photo" />
                            <span>
                                <strong><asp:Literal runat="server" ID="Title" /></strong>
                                <small><asp:Literal runat="server" ID="Description" /></small>
                            </span>
                        </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </web:PagedRepeater>

            <asp:Panel runat="server" ID="ProductDetails" Visible="false" CssClass="product-details">

                <asp:Image runat="server" ID="HeroPhoto" CssClass="hero" /> 

                <div class="meta">
                    <div>
                        <asp:Literal runat="server" ID="DayRate" />
                    </div>
                    <div>
                        <asp:LinkButton runat="server" ID="AddToCartButton" CssClass="button" Text="Add to cart" OnClick="AddToCart" />
                    </div>
                </div>

                <div class="description">
                    <asp:Literal runat="server" ID="ProductDescription" />
                </div>

                <p>
                    <asp:HyperLink runat="server" ID="BackButton" CssClass="button" Width="100%" Text="Browse Products" NavigateUrl="/account/seeking/products" />
                </p>

            </asp:Panel>

            <asp:Panel runat="server" ID="BookNowPanel" Visible="false" ng-controller="SearchController">

                <div class="search-form">

                    <div id="StepFor" ng-show="search.step===1">
                        <p>Who is this booking for?</p>
                        <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
                    </div>

                    <div id="StepDays" ng-show="search.step===2">
                        <p>How many days do you need the products for?</p>
                        <asp:TextBox runat="server" ID="DayCount" placeholder="Days" />      
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="DayCount" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step2"/>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="DayCount" Text="Please enter a valid number" ValidationGroup="Step2" ValidationExpression="^[\d]+$" Display="Dynamic" />
                    </div>

                    <div id="StepDate" ng-show="search.step===3">
                        <p>When do you require the products?</p>
                        <web:Date runat="server" ID="StartDate" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" ValidationGroup="Step3" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationGroup="Step3" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" />
                    </div>

                    <div id="StepLocation" ng-show="search.step===4">
                        <p>Where do you require the products?<br />
                            <small>Click and drag the map so the marker is at the required location.</small></p>
                        <web:Map runat="server" Id="Location" />
                    </div>

                </div>

                <div class="cols">
                    <div>
                        <a ng-click="search.previous()" class="button" ng-hide="search.step===1">Back</a>
                    </div>
                    <div style="text-align: right">
                        <a ng-click="search.next()" class="button" ng-hide="search.step===4">Next</a>
                        <asp:Button runat="server" OnClick="CreateBooking" Text="Book Now" ID="SearchButton" CssClass="button" ng-show="search.step===4" />
                    </div>
                </div>

            </asp:Panel>

        </div>
        <div>
            
            <h3>Your Cart</h3>
            <web:PagedRepeater runat="server" ID="CartList" OnItemDataBound="BindCartItem" EmptyMessage="Your cart is empty">
                <HeaderTemplate>
                    <div class="cart-list">
                </HeaderTemplate>
                <ItemTemplate>      
                    <div>
                        <span runat="server" ID="Photo" />
                        <span>
                            <strong><asp:Literal runat="server" ID="Title" /></strong>
                            <small><asp:Literal runat="server" ID="Description" /></small>
                        </span>                            
                        <asp:LinkButton runat="server" ID="RemoveFromCartButton" CssClass="material-icons-round md-16" OnClick="RemoveFromCart" Text="delete" />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                    <p style="margin-top: 30px">
                        <asp:HyperLink runat="server" ID="BookButton" CssClass="button" Width="100%" Text="Book Now" NavigateUrl="/account/seeking/products?book=now" />
                    </p>
                </FooterTemplate>
            </web:PagedRepeater>
            
        </div>
    </div>

</asp:Content>
