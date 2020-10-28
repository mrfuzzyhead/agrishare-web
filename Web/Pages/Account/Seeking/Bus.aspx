<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking a bus" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Bus.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Bus" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking a bus</h1>

    <div ng-controller="SearchController">

        <div class="search-form">

            <div id="StepFor" ng-show="search.step===1">
                <p>Who is this booking for?</p>
                <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
            </div>

            <div id="StepDate" ng-show="search.step===2">
                <p>When do you require the service to be performed?</p>
                <web:Date runat="server" ID="StartDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" ValidationGroup="Step3" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationGroup="Step3" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" />
            </div>

            <div id="StepBusPickUp" ng-show="search.step===3">
                <p>Where is the pick-up location?<br /><small>Drag the map to the desired location.</small></p>
                <web:Map runat="server" Id="PickupLocation" />
            </div>

            <div id="StepBusDropOfff" ng-show="search.step===4">
                <p>Where is the drop-off location?<br /><small>Drag the map to the desired location.</small></p>
                <web:Map runat="server" Id="DropoffLocation" />
            </div>

        </div>

        <div class="cols">
            <div>
                <a ng-click="search.previous()" class="button" ng-hide="search.step===1">Back</a>
            </div>
            <div style="text-align: right">
                <a ng-click="search.next()" class="button" ng-hide="search.step===4">Next</a>
                <asp:Button runat="server" OnClick="FindListings" Text="Search" ID="SearchButton" CssClass="button" ng-show="search.step===4" />
            </div>
        </div>

    </div>

</asp:Content>
