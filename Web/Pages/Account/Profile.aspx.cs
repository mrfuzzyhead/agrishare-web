using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.RequiresAuthentication = true;
            Master.Body.Attributes["class"] += " account ";

            DisplayName.Text = HttpUtility.HtmlEncode(Master.CurrentUser.FirstName + " " + Master.CurrentUser.LastName);
            DisplayCountry.Text = HttpUtility.HtmlEncode(Master.CurrentUser.Region.Title);
            DisplayGender.Text = HttpUtility.HtmlEncode(Master.CurrentUser.Gender);
            DisplayDateOfBirth.Text = Master.CurrentUser.DateOfBirth.HasValue ? Master.CurrentUser.DateOfBirth.Value.ToString("d MMMM yyyy") : "-";
            DisplayTelephone.Text = HttpUtility.HtmlEncode(Master.CurrentUser.Telephone);
            DisplayEmailAddress.Text = HttpUtility.HtmlEncode(Master.CurrentUser.EmailAddress);

            if (!Page.IsPostBack)
            {
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Male}", Value = ((int)Core.Entities.Gender.Male).ToString() });
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Female}", Value = ((int)Core.Entities.Gender.Female).ToString() });
            }

            switch (Request.QueryString["view"])
            {
                case "edit":
                    EditProfileForm.Visible = true;
                    EditProfileLink.CssClass = "active";
                    if (!Page.IsPostBack)
                    {
                        Region.DataSource = Core.Entities.Region.List();
                        Region.DataTextField = "Title";
                        Region.DataValueField = "Id";
                        Region.DataBind();
                        Region.Items.Insert(0, new ListItem("Select...", ""));

                        FirstName.Text = Master.CurrentUser.FirstName;
                        LastName.Text = Master.CurrentUser.LastName;
                        EmailAddress.Text = Master.CurrentUser.EmailAddress;
                        Telephone.Text = Master.CurrentUser.Telephone;
                        Gender.SelectedValue = ((int)Master.CurrentUser.GenderId).ToString();
                        DateOfBirth.SelectedDate = Master.CurrentUser.DateOfBirth;
                        Region.SelectedValue = Master.CurrentUser.Region.Id.ToString();
                    }
                    break;
                case "notifications":
                    NotificationPreferencesForm.Visible = true;
                    NotificationPrefsLink.CssClass = "active";
                    if (!Page.IsPostBack)
                    {
                        SMS.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.SMS) > 0;
                        PushNotifications.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.PushNotifications) > 0;
                        Email.Checked = (Master.CurrentUser.NotificationPreferences & (int)Core.Entities.NotificationPreferences.Email) > 0;
                    }
                    break;
                case "payments":
                    PaymentDetailsForm.Visible = true;
                    PaymentDetailsLink.CssClass = "active";
                    if (!Page.IsPostBack)
                    {
                        Cash.Checked = Master.CurrentUser.PaymentMethods.Contains(Core.Entities.PaymentMethod.Cash);
                        BankTransfer.Checked = Master.CurrentUser.PaymentMethods.Contains(Core.Entities.PaymentMethod.BankTransfer);
                        MobileMoney.Checked = Master.CurrentUser.PaymentMethods.Contains(Core.Entities.PaymentMethod.MobileMoney);
                        Bank.Text = Master.CurrentUser.BankAccount.Bank;
                        Branch.Text = Master.CurrentUser.BankAccount.Branch;
                        AccountName.Text = Master.CurrentUser.BankAccount.AccountName;
                        AccountNumber.Text = Master.CurrentUser.BankAccount.AccountNumber;
                    }
                    break;
                case "resetpin":
                    ResetForm.Visible = true;
                    ResetPinLink.CssClass = "active";
                    if (!Page.IsPostBack)
                        Master.CurrentUser.SendVerificationCode();
                    break;
                case "delete":
                    DeleteForm.Visible = true;
                    DeleteAccountLink.CssClass = "active";
                    var bookings = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
                    if (bookings > 0)
                        DeleteWarning.Visible = true;
                    var listings = Core.Entities.Listing.List(UserId: Master.CurrentUser.Id);
                    foreach (var listing in listings)
                    {
                        bookings = Core.Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                        if (bookings > 0)
                            DeleteWarning.Visible = true;
                    }
                    DeletePrompt.Visible = !DeleteWarning.Visible;
                    break;

                default:
                    Introduction.Visible = true;

                    if (Master.CurrentUser.PaymentMethods.Count == 0)
                        PaymentsPrompt.Visible = true;

                    AgentName.Text = Master.CurrentUser.Agent?.Title ?? "";
                    AgentDetails.Visible = Master.CurrentUser.AgentId.HasValue && Master.CurrentUser.AgentTypeId == Core.Entities.AgentUserType.Admin;
                    if (AgentDetails.Visible)
                    {
                        BookingCount.Text = Core.Entities.Booking.SeekingSummaryAgentAdminCount(Master.CurrentUser.AgentId ?? 0).ToString("N0");
                        CommissionAmount.Text = "$" + Core.Entities.Booking.SeekingSummaryAgentAdminCommission(Master.CurrentUser.AgentId ?? 0).ToString("N2");
                    }

                    break;
            }

        }

        public void UpdateUser(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Master.CurrentUser.Id);

            user.FirstName = FirstName.Text;
            user.LastName = LastName.Text;
            user.EmailAddress = EmailAddress.Text;
            user.Telephone = Telephone.Text;
            user.GenderId = (Core.Entities.Gender)Enum.ToObject(typeof(Core.Entities.Gender), int.Parse(Gender.SelectedValue));
            user.DateOfBirth = DateOfBirth.SelectedDate;
            user.Region = Core.Entities.Region.Find(Convert.ToInt32(Region.SelectedValue));

            if (!user.UniqueTelephone())
                Master.Feedback = "Telephone number has already been registered";
            else if (!user.UniqueEmailAddress())
                Master.Feedback = "Email address has already been registered";
            else if (user.Save())
            {
                Master.Feedback = "Your details have been updated";
                Response.Redirect("/account/profile");
            }
            else
                Master.Feedback = "An unknown error occured";
        }

        public void UpdateNotificationPreferences(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Master.CurrentUser.Id);
            user.NotificationPreferences = 0;
            if (SMS.Checked)
                user.NotificationPreferences += (int)Core.Entities.NotificationPreferences.SMS;
            if (PushNotifications.Checked)
                user.NotificationPreferences += (int)Core.Entities.NotificationPreferences.PushNotifications;
            if (Email.Checked)
                user.NotificationPreferences += (int)Core.Entities.NotificationPreferences.Email;

            if (user.Save())
            {
                Master.Feedback = "Your notification preferences have been updated";
                Response.Redirect("/account/profile");
            }
            else
                Master.Feedback = "An unknown error occured";
        }

        public void UpdatePaymentDetails(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Master.CurrentUser.Id);
            user.PaymentMethods = new List<Core.Entities.PaymentMethod>();
            if (Cash.Checked)
                user.PaymentMethods.Add(Core.Entities.PaymentMethod.Cash);
            if (BankTransfer.Checked)
                user.PaymentMethods.Add(Core.Entities.PaymentMethod.BankTransfer);
            if (MobileMoney.Checked)
                user.PaymentMethods.Add(Core.Entities.PaymentMethod.MobileMoney);
            user.BankAccount.Bank = Bank.Text;
            user.BankAccount.Branch = Branch.Text;
            user.BankAccount.AccountName = AccountName.Text;
            user.BankAccount.AccountNumber = AccountNumber.Text;

            if (user.Save())
            {
                Master.Feedback = "Your payment details have been updated";
                var redir = Request.QueryString["r"];
                if (string.IsNullOrEmpty(redir))
                    redir = "/account/profile/payments";
                Response.Redirect(redir);
            }
            else
                Master.Feedback = "An unknown error occured";
        }

        public void UpdatePassword(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ResetTelephone.Text);
                if (user.Id > 0 && !user.VerificationCode.IsEmpty() && user.VerificationCode == ResetCode.Text)
                {
                    user.ClearPassword = ResetPIN.Text;
                    user.FailedLoginAttempts = 0;
                    user.AuthToken = Guid.NewGuid().ToString();
                    user.VerificationCode = string.Empty;
                    if (user.Save())
                        Master.Feedback = "Your PIN has been updated";
                    else
                        Master.Feedback = "An unknown error occured";
                }
                else
                    Master.Feedback = "The verification code has expired - please reset your PIN again";

            }
        }

        public void Logout(object s, EventArgs e)
        {
            var user = Core.Entities.User.Find(Master.CurrentUser.Id);
            user.AuthToken = string.Empty;
            user.Save();

            Master.DropCookie(string.Empty);
            Response.Redirect("/");
        }

        public void DeleteAccount(object s, EventArgs e)
        {
            var bookings = Core.Entities.Booking.Count(UserId: Master.CurrentUser.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
            {
                Master.Feedback = "Can not delete account - you have upcoming bookings";
                return;
            }
            
            var listings = Core.Entities.Listing.List(UserId: Master.CurrentUser.Id);
            foreach (var listing in listings)
            {
                bookings = Core.Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                {
                    Master.Feedback = "Can not delete account - you have upcoming bookings";
                    return;
                }
            }

            foreach (var listing in listings)
                listing.Delete();

            var devices = Core.Entities.Device.List(UserId: Master.CurrentUser.Id);
            foreach (var device in devices)
                device.Delete();

            Master.CurrentUser.AuthToken = string.Empty;
            Master.CurrentUser.Delete();
            Master.DropCookie(string.Empty);
            Response.Redirect("/");
        }
    }
}