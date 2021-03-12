<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Profile" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>My Profile</h1>

    <div class="cols">

        <div>

            <asp:PlaceHolder runat="server" ID="Introduction" Visible="false">

                <asp:Panel runat="server" ID="PaymentsPrompt" Visible="false">
                    <p style="padding: 5px 10px; border-radius: 5px; background: #eee">
                        If you would like to offer equipment, please <a href="/account/profile/payments">set your payment preferences</a>.
                    </p>
                </asp:Panel>

                <p>
                    Welcome <strong><asp:Literal runat="server" ID="DisplayName" /></strong><br />
                    <small>Please review your profile detalis below and use the menu to edit your details.</small>
                </p>

                <ul class="profile-details">
                    <li runat="server" id="SupplierRow"><small>SUPPLIER:</small> <span><asp:Literal runat="server" ID="DisplaySupplier" /></span></li>
                    <li><small>COUNTRY:</small> <span><asp:Literal runat="server" ID="DisplayCountry" /></span></li>
                    <li><small>EMAIL:</small> <span><asp:Literal runat="server" ID="DisplayEmailAddress" /></span></li>
                    <li><small>TELEPHONE:</small> <span><asp:Literal runat="server" ID="DisplayTelephone" /></span></li>
                    <li><small>GENDER:</small> <span><asp:Literal runat="server" ID="DisplayGender" /></span></li>
                    <li><small>DATE OF BIRTH:</small> <span><asp:Literal runat="server" ID="DisplayDateOfBirth" /></span></li>
                </ul>

                <asp:Panel runat="server" ID="AgentDetails">
                    <h3>Commissions: <asp:Literal runat="server" Id="AgentName" /></h3>
                    <asp:Panel runat="server" ID="Summary" CssClass="summary">
                        <div>
                            <small>Bookings</small>
                            <span><asp:Literal runat="server" Id="BookingCount" /></span>
                        </div>
                        <div>
                            <small>Commission</small>
                            <span><asp:Literal runat="server" Id="CommissionAmount" /></span>
                        </div>
                    </asp:Panel>
                    <p>
                        <a href="/account/commission/report" class="button">Download report</a>
                    </p>
                </asp:Panel>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="EditProfileForm" Visible="false">

                <h2>Edit Profile</h2>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="Region" Text="Country *" />
                    <asp:DropDownList runat="server" ID="Region" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Region" Text="Country is required" Display="Dynamic" ValidationGroup="Edit" />
                </div>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="FirstName" Text="First Name *" />
                        <asp:TextBox runat="server" ID="FirstName" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="FirstName" Text="First name is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="LastName" Text="Last Name *" />
                        <asp:TextBox runat="server" ID="LastName" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="LastName" Text="Last name is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                </div>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="EmailAddress" Text="Email Address *" />
                        <asp:TextBox runat="server" ID="EmailAddress" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailAddress" Text="Email address is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Telephone" Text="Telephone *" />
                        <asp:TextBox runat="server" ID="Telephone" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Telephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                </div>

                <div class="form-cols">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Gender" Text="Gender *" />
                        <asp:DropDownList runat="server" ID="Gender" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Gender" Text="Gender is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="DateOfBirth" Text="Date of birth *" />
                        <web:Date runat="server" ID="DateOfBirth" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="DateOfBirth" Text="Date of birth is required" Display="Dynamic" ValidationGroup="Edit" />
                    </div>

                </div>

                <p>
                    <asp:Button runat="server" Text="Update" CssClass="button" OnClick="UpdateUser" ValidationGroup="Edit" />
                </p>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="PaymentDetailsForm" Visible="false">

                <h2>Update Payment Details</h2>
                <p>If you are supplying equipment, please select the payment methods you accept and enter your bank details if necessary.</p>
        
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="Cash" Text="Cash" />
                </div>
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="MobileMoney" Text="Mobile Money" />
                </div>  
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="BankTransfer" Text="Bank Transfer" />
                </div>  

                <div class="form-cols bank-details">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Bank" Text="Bank" />
                        <asp:TextBox runat="server" ID="Bank" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="Branch" Text="Branch" />
                        <asp:TextBox runat="server" ID="Branch" />
                    </div>

                </div>

                <div class="form-cols bank-details">

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="AccountName" Text="Account Name" />
                        <asp:TextBox runat="server" ID="AccountName" />
                    </div>

                    <div class="form-row">
                        <asp:Label runat="server" AssociatedControlID="AccountNumber" Text="Account Number" />
                        <asp:TextBox runat="server" ID="AccountNumber" />
                    </div>

                </div>     

                <p>
                    <asp:Button runat="server" Text="Update" CssClass="button" OnClick="UpdatePaymentDetails" ValidationGroup="Payments" />
                </p>

                <script type="text/javascript">
                    $('#Content_BankTransfer').change(function () {
                        if (this.checked)
                            $('.bank-details').show();
                        else
                            $('.bank-details').hide();
                    }).change();

                </script>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="NotificationPreferencesForm" Visible="false">

                <h2>Update Notification Preferences</h2>
        
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="SMS" Text="SMS" />
                </div>
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="PushNotifications" Text="Push Notifications" />
                </div>
                <div class="checkbox-row">
                    <asp:CheckBox runat="server" ID="Email" Text="Email" />
                </div>        

                <p>
                    <asp:Button runat="server" Text="Update" CssClass="button" OnClick="UpdateNotificationPreferences" ValidationGroup="Notifications" />
                </p>

            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="ResetForm" Visible="false">

                <h2>Reset Password</h2>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetTelephone" Text="Telephone *" />
                    <asp:TextBox runat="server" ID="ResetTelephone" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetTelephone" Text="Telephone is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetCode" Text="Verification Code *" />
                    <asp:TextBox runat="server" ID="ResetCode" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetCode" Text="Verification code is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="ResetPIN" Text="New PIN *" />
                    <asp:TextBox runat="server" ID="ResetPIN" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ResetPIN" Text="New PIN is required" Display="Dynamic" ValidationGroup="Reset" />
                </div>

                <p>
                    <asp:Button runat="server" Text="Reset" CssClass="button" OnClick="UpdatePassword" ValidationGroup="Reset" />
                </p>

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="DeleteForm" runat="server" Visible="false">

                <h2>Delete my account</h2>

                <asp:PlaceHolder ID="DeletePrompt" runat="server" Visible="true">
                    <p>Are you sure you would like to delete your account?</p>
                    <p>
                        <asp:LinkButton runat="server" OnClick="DeleteAccount">Yes, delete my account</asp:LinkButton>
                    </p>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="DeleteWarning" runat="server" Visible="true">
                    <p>You can not delete your account while you have active or pending bookings. Please cancel your bookings first.</p>
                </asp:PlaceHolder>

            </asp:PlaceHolder>

        </div>

        <div>

            <ul class="menu">
                <li><asp:HyperLink runat="server" ID="EditProfileLink" NavigateUrl="/account/profile/edit"><i class="material-icons-round">person</i><span>Edit Profile</span></asp:HyperLink></li>
                <li><asp:HyperLink runat="server" ID="PaymentDetailsLink" NavigateUrl="/account/profile/payments"><i class="material-icons-round">credit_card</i><span>Payment Details</span></asp:HyperLink></li>
                <li><asp:HyperLink runat="server" ID="NotificationPrefsLink" NavigateUrl="/account/profile/notifications"><i class="material-icons-round">notifications</i><span>Notification Preferences</span></asp:HyperLink></li>
                <li><asp:HyperLink runat="server" ID="ResetPinLink" NavigateUrl="/account/profile/resetpin"><i class="material-icons-round">vpn_key</i><span>Reset PIN</span></asp:HyperLink></li>
                <li class="logout"><asp:LinkButton runat="server" OnClick="Logout"><i class="material-icons-round">lock</i><span>Logout</span></asp:LinkButton></li>
                <li class="delete"><asp:HyperLink runat="server" ID="DeleteAccountLink" NavigateUrl="/account/profile/delete"><i class="material-icons-round md-16">delete</i><small>Delete my account</small></asp:HyperLink></li>
            </ul>

        </div>

    </div>

</asp:Content>
