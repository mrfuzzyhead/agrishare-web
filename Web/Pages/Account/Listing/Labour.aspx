<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Tractor" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" CodeBehind="Labour.aspx.cs" Inherits="Agrishare.Web.Pages.Account.Listing.Labour" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    
    <h1>Labour</h1>

    <p>Enter the details for your labour services.</p>

    <div class="form-cols">

        <div>

            <h3>Listing</h3>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="EquipmentTitle" Text="Title *" />
                <asp:TextBox runat="server" ID="EquipmentTitle" MaxLength="1024" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="EquipmentTitle" Text="Title is required" Display="Dynamic" />
            </div>

            <div class="checkbox-row">
                <asp:CheckBox runat="server" ID="GroupHire" Text="Allow group hire" />
            </div>

            <h3>Services</h3>

            <asp:Repeater runat="server" ID="Services" OnItemDataBound="BindService">
                <ItemTemplate>

                    <div class="service">

                        <asp:HiddenField runat="server" ID="CategoryId" />
                        <asp:HiddenField runat="server" ID="ServiceId" />

                        <strong class="checkbox-row service-checkbox"><asp:CheckBox runat="server" ID="Title" /></strong>
                        
                        <div class="service-row">

                            <div class="form-row">
                                <asp:Label runat="server" AssociatedControlID="PricePerQuantityUnit" Text="Hire Cost *" />
                                <div><asp:TextBox runat="server" ID="PricePerQuantityUnit" MaxLength="8" /><span>$/DAY</span></div>
                                <asp:CustomValidator runat="server" ControlToValidate="PricePerQuantityUnit" ValidateEmptyText="true" ClientValidationFunction="validateServiceField" OnServerValidate="ValidateServiceField" Text="Hire cost is required" Display="Dynamic" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerQuantityUnit" Text="Hire cost  is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
                            </div>

                        </div>

                    </div>

                </ItemTemplate>
            </asp:Repeater>

        </div>
        <div>

            <h3>Location</h3>

            <div class="form-row">
                <label>Location</label>
                <web:Map runat="server" Id="Location" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="PricePerDistanceUnit" Text="Distance charge *" />
                <div><asp:TextBox runat="server" ID="PricePerDistanceUnit" MaxLength="8" /><span>$/KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PricePerDistanceUnit" Text="Distance charge is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PricePerDistanceUnit" Text="Distance charge is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <div class="form-row">
                <asp:Label runat="server" AssociatedControlID="MaximumDistance" Text="Maximum distance *" />
                <div><asp:TextBox runat="server" ID="MaximumDistance" MaxLength="8" /><span>KM</span></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is required" Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="MaximumDistance" Text="Maximum distance is invalid" ValidationExpression="^[\d]+(.[\d]+)?$" Display="Dynamic" />
            </div>

            <h3>Photos</h3>
            <web:PhotoUpload runat="server" Id="Gallery" />

        </div>

    </div>

    <p>
        <asp:Button runat="server" Text="Save" CssClass="button" OnClick="Save" />
    </p>

    <script>
        $('.service-checkbox input').change(function () {
            if ($(this).is(':checked'))
                $(this).parent().next('div').show();
            else                
                $(this).parent().next('div').hide();
        }).change();

        function validateServiceField(sender, args) {
            var checked = $(sender).closest('.service').find('input[type=checkbox]').is(':checked');
            var empty = $(sender).closest('div').find('input').val() === '';
            args.IsValid = !checked || (checked && !empty);
        }

    </script>

</asp:Content>
