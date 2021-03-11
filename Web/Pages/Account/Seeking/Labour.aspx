<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking labour" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Labour.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Labour" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking labour</h1>

    <div ng-controller="SearchController">

        <div class="search-form">

            <div id="StepFor" ng-show="search.step===1">
                <p>Who is this booking for?</p>
                <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
            </div>

            <div id="StepService" ng-show="search.step===2">
                <p>What type of service do you require?</p>                            
                <asp:CheckBoxList runat="server" ID="Services" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
            </div>

            <div id="StepTractorDetails" ng-show="search.step===3">
                <p>How many days do you need the person for:</p>
                <asp:TextBox runat="server" ID="DayCount" placeholder="Days" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="DayCount" Text="This is a required field" Display="Dynamic" ValidationGroup="Step3"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="DayCount" Text="Please enter a valid number" ValidationGroup="Step2" ValidationExpression="^[\d]+$" Display="Dynamic" />
            </div>

            <div id="StepDate" ng-show="search.step===4">
                <p>When do you require the service to be performed?</p>
                <web:Date runat="server" ID="StartDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" ValidationGroup="Step4"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" ValidationGroup="Step4"/>
            </div>

            <div id="StepLocation" ng-show="search.step===5">
                <p>Where do you need the service to be performed?<br />
                    <small>Click and drag the map so the marker is at the required location.</small></p>
                <web:Map runat="server" Id="Location" />
            </div>

        </div>

        <div class="cols">
            <div>
                <a ng-click="search.previous()" class="button" ng-hide="search.step===1">Back</a>
            </div>
            <div style="text-align: right">
                <a ng-click="search.next()" class="button" ng-hide="search.step===5">Next</a>
                <asp:Button runat="server" OnClick="FindListings" Text="Search" ID="SearchButton" CssClass="button" ng-show="search.step===5" />
            </div>
        </div>

    </div>

</asp:Content>
