<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking land" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Land.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Land" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking land</h1>

    <div ng-controller="SearchController">

        <div class="search-form">

            <div id="StepFor" ng-show="search.step===1">
                <p>Who is this booking for?</p>
                <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
            </div>

            <div id="StepREgion" ng-show="search.step===2">
                <p>In which region do you want to hire land?</p>
                <asp:RadioButtonList runat="server" ID="LandRegion" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="LandRegion" Text="This is a required field" Display="Dynamic" ValidationGroup="Step2" />
            </div>

            <div id="StepDistance" ng-show="search.step===3">
                <p>How many acres do you want to hire?</p>
                <asp:TextBox runat="server" ID="Acres" placeholder="Acres" />      
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Acres" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step3"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="Acres" Text="Please enter a valid number" ValidationGroup="Step3" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                <p>&nbsp;</p>
                <p>How many years do you want to use the land for?</p>
                <asp:TextBox runat="server" ID="Years" placeholder="Years" />      
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Years" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step3"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="Years" Text="Please enter a valid number" ValidationGroup="Step3" ValidationExpression="^[\d]$" Display="Dynamic" />
            </div>

        </div>

        <div class="cols">
            <div>
                <a ng-click="search.previous()" class="button" ng-hide="search.step===1">Back</a>
            </div>
            <div style="text-align: right">
                <a ng-click="search.next()" class="button" ng-hide="search.step===3">Next</a>
                <asp:Button runat="server" OnClick="FindListings" Text="Search" ID="SearchButton" CssClass="button" ng-show="search.step===3" />
            </div>
        </div>

    </div>

</asp:Content>
