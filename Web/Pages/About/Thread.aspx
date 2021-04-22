<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Thread.aspx.cs" Inherits="Agrishare.Web.Pages.About.Thread" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Message Thread</h1>

    <p>This is your conversation with AgriShare support.</p>

    <div class="cols">

        <div>

            <h2><asp:Literal runat="server" ID="MessageTitle" /></h2>

            <div class="original-message">
                <asp:Literal runat="server" ID="MessageContent" />
            </div>

            <asp:Repeater runat="server" Id="ThreadList" OnItemDataBound="BindMessage">
                <ItemTemplate>
                    <div class="thread-message">
                        <strong><asp:Literal runat="server" ID="Name" /></strong> <small><asp:Literal runat="server" ID="Date" /></small><br />
                        <asp:Literal runat="server" ID="Content" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="Message" Text="Reply *" />
                <asp:TextBox runat="server" ID="Message" TextMode="MultiLine" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Text="Message is required" Display="Dynamic" />
            </div>

            <p>
                <asp:Button runat="server" Text="Send" CssClass="button" OnClick="SendReply" />
            </p>

        </div>

        <div>

            <h3>FAQs</h3>
            <p>Do you have questions about AgriShare? Read through our Frequently Asked Questions.</p>
            <p><a href="/about/faqs" class="button">Read the FAQs</a></p>

        </div>

    </div>


</asp:Content>
