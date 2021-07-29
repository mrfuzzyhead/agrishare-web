﻿using Agrishare.Core;
using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Agrishare.Web.Pages.Account
{
    public partial class GetStarted : System.Web.UI.Page
    {
        private string DashboardUrl = "/account/seeking";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["view"]?.Equals("forgot") ?? false)
            {
                ForgotForm.Visible = true;
            }
            else if (!Page.IsPostBack)
            {
                RegisterForm.Visible = true;
                LoginForm.Visible = true;

                Region.DataSource = Core.Entities.Region.List();
                Region.DataTextField = "Title";
                Region.DataValueField = "Id";
                Region.DataBind();
                Region.Items.Insert(0, new ListItem("Select...", ""));

                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Male}", Value = Core.Entities.Gender.Male.ToString() });
                Gender.Items.Add(new ListItem { Text = $"{Core.Entities.Gender.Female}", Value = Core.Entities.Gender.Female.ToString() });
                Gender.Items.Insert(0, new ListItem("Select...", ""));
            }
        }

        public void RegisterUser(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                DateTime? dob = null;
                try { dob = DateTime.Parse(DateOfBirth.Text); }
                catch { }

                var user = new Core.Entities.User
                {
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    EmailAddress = EmailAddress.Text,
                    Telephone = Telephone.Text,
                    ClearPassword = PIN.Text,
                    GenderId = (Core.Entities.Gender)Enum.Parse(typeof(Core.Entities.Gender), Gender.SelectedValue),
                    DateOfBirth = dob,
                    Roles = new List<Core.Entities.Role> { Core.Entities.Role.User },
                    AuthToken = Guid.NewGuid().ToString(),
                    StatusId = Core.Entities.UserStatus.Verified,
                    LanguageId = Core.Entities.Language.English,
                    RegionId = Convert.ToInt32(Region.SelectedValue),
                    Region = Core.Entities.Region.Find(Convert.ToInt32(Region.SelectedValue))
                };

                if (!user.UniqueEmailAddress())
                {
                    Master.Feedback = "Your email address has already been registered";
                }
                else if (!user.UniqueTelephone())
                {
                    Master.Feedback = "Your telephone number has already been registered";
                }
                else
                {
                    if (!Core.Entities.User.VerificationRequired)
                    {
                        user.StatusId = UserStatus.Verified;
                        if (user.AuthToken.IsEmpty())
                            user.AuthToken = Guid.NewGuid().ToString();
                        user.Save();

                        Counter.Hit(user.Id, Counters.Register);

                        Master.DropCookie(user.AuthToken);
                        Master.Feedback = "Your account has been created and you are now logged in.";
                        Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                    }
                    else
                    {
                        user.StatusId = UserStatus.Pending;
                        user.AuthToken = string.Empty;
                        user.Save();

                        Counter.Hit(user.Id, Counters.Register);

                        user.SendVerificationCode();

                        Master.Feedback = "Please check your inbox for a verification code to complete your registration.";
                        RegisterForm.Visible = false;

                        VerifyUserId.Value = user.Id.ToString();
                        VerifyForm.Visible = true;
                    }
                }
            }
        }

        public void VerifyAccount(object s, EventArgs e)
        {
            var userId = Convert.ToInt32(VerifyUserId.Value);
            var user = Core.Entities.User.Find(userId);

            if (user?.FailedLoginAttempts > Core.Entities.User.MaxFailedLoginAttempts)
                Master.Feedback = "Your account has been locked - please reset your PIN.";

            else if (user?.VerificationCode == VerifyPin.Text)
            {
                if (user.VerificationCodeExpiry < DateTime.UtcNow)
                    Master.Feedback = "This code has expired";
                else
                {
                    user.VerificationCode = string.Empty;
                    user.StatusId = Core.Entities.UserStatus.Verified;
                    user.AuthToken = Guid.NewGuid().ToString();
                    user.Save();

                    Master.DropCookie(user.AuthToken);
                    Master.Feedback = "Your account has been created and you are now logged in.";
                    Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                }
            }

            if (user.Id > 0)
            {
                user.FailedLoginAttempts += 1;
                user.Save();
                Master.Feedback = "Invalid code - please try again";
            }
            else
            {
                Master.Feedback = "An unknown error occurred - please try again";
            }
        }

        public void AuthenticateUser(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: LoginTelephone.Text);
                if (user != null && user.Id > 0)
                {
                    if (user.FailedLoginAttempts > Core.Entities.User.MaxFailedLoginAttempts)
                    {
                        Master.Feedback = "Your account has been locked - please reset your password";
                    }
                    else if (user.StatusId != UserStatus.Verified)
                    {
                        Master.Feedback = "Your account has not been verified - please contact support";
                    }
                    else if (user.ValidatePassword(LoginPIN.Text))
                    {
                        user.FailedLoginAttempts = 0;
                        user.AuthToken = Guid.NewGuid().ToString();
                        user.Save();
                        Master.DropCookie(user.AuthToken);
                        Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                    }
                    else
                    {
                        user.FailedLoginAttempts += 1;
                        user.Save();
                        Master.Feedback = "Telephone or PIN not recognised";
                    }
                }
                else
                {
                    Master.Feedback = "Telephone or PIN not recognised";
                }
            }
        }

        public void SendResetCode(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ForgotTelephone.Text);
                if (user != null && user.Id > 0)
                    user.SendVerificationCode();
                ResetForm.Visible = true;
                ForgotForm.Visible = false;
            }
        }

        public void UpdatePassword(object s, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = Core.Entities.User.Find(Telephone: ResetTelephone.Text);
                if (user != null && user.Id > 0 && !user.VerificationCode.IsEmpty() && user.VerificationCode == ResetCode.Text)
                {
                    user.ClearPassword = ResetPIN.Text;
                    user.FailedLoginAttempts = 0;
                    user.AuthToken = Guid.NewGuid().ToString();
                    user.VerificationCode = string.Empty;
                    user.Save();
                    Master.DropCookie(user.AuthToken);
                    Response.Redirect(Request.QueryString["r"] ?? DashboardUrl);
                }
                else
                {
                    Master.Feedback = "The verification code has expired - please reset your PIN again";
                    ResetForm.Visible = false;
                    ForgotForm.Visible = true;
                }
            }
        }
    }
}