<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Seeking irrigation" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Irrigation.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Seeking.Irrigation" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Seeking irrigation</h1>

    <div ng-controller="SearchController">

        <div class="search-form">

            <div id="StepFor" ng-show="search.step===1">
                <p>Who is this booking for?</p>
                <asp:RadioButtonList runat="server" ID="For" RepeatLayout="UnorderedList" CssClass="checkbox-list" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="For" Text="This is a required field" Display="Dynamic" ValidationGroup="Step1" />
            </div>

            <div id="StepDays" ng-show="search.step===2">
                <p>How many days do you need the equipment for?</p>
                <asp:TextBox runat="server" ID="DayCount" placeholder="Days" />      
                <asp:RequiredFieldValidator runat="server" ControlToValidate="DayCount" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step2"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="DayCount" Text="Please enter a valid number" ValidationGroup="Step2" ValidationExpression="^[\d]+$" Display="Dynamic" />
            </div>

            <div id="StepDistance" ng-show="search.step===3">
                <p>What is the distance to the water source from your garden?</p>
                <asp:TextBox runat="server" ID="WaterSourceDistance" placeholder="Distance (m)" />      
                <asp:RequiredFieldValidator runat="server" ControlToValidate="WaterSourceDistance" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step3"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="WaterSourceDistance" Text="Please enter a valid number" ValidationGroup="Step3" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                <p>&nbsp;</p>
                <p>How deep is the water source?</p>
                <asp:TextBox runat="server" ID="WaterSourceDepth" placeholder="Depth (m)" />      
                <asp:RequiredFieldValidator runat="server" ControlToValidate="WaterSourceDepth" Text="This is a required field" Display="Dynamic"  ValidationGroup="Step3"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="WaterSourceDepth" Text="Please enter a valid number" ValidationGroup="Step3" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div id="StepDate" ng-show="search.step===4">
                <p>When do you require the service to be performed?</p>
                <web:Date runat="server" ID="StartDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDate" Text="This is a required field" Display="Dynamic" ValidationGroup="Step4" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="StartDate" Text="Please enter a valid date in the format dd/MM/yyyy" ValidationGroup="Step4" ValidationExpression="^[\d]{2}/[\d]{2}/[\d]{4}?$" Display="Dynamic" />
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
