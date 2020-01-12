﻿<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="FAQs" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="FAQs.aspx.cs" Inherits="Agrishare.Web.Pages.About.FAQs" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>FAQs</h1>

    <p>Do you have questions about AgriShare? Read through our Frequently Asked Questions.</p>

    <div class="cols">

        <div>

            <asp:Repeater runat="server" ID="List" OnItemDataBound="BindFaq">
                <HeaderTemplate>
                    <div class="faq-list">
                </HeaderTemplate>
                <ItemTemplate>
                        <div>
                            <p class="q"><asp:Literal runat="server" ID="Question" /></p>
                            <p class="a"><asp:Literal runat="server" ID="Answer" /></p>
                        </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

        </div>

        <div>

            <h3>Get in touch</h3>

            <p>We'd love to hear from you. Fill out this simple form and we'll get in touch with you.</p>

            <p><a href="/about/contact" class="button">Contact us</a></p>

        </div>

    </div>

</asp:Content>
